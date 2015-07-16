using System;
using System.Text;
using System.Collections.Generic;

namespace Lipsis.Core {
    public unsafe class ArithmeticQueue {
        private LinkedList<ArithmeticOperator> p_Operators;
        private LinkedList<ArithmeticOperand> p_Operands;
        private sbyte p_ResultSize;
        private bool p_HasDecimal;
        
        public ArithmeticQueue() {
            p_Operators = new LinkedList<ArithmeticOperator>();
            p_Operands = new LinkedList<ArithmeticOperand>();
            p_ResultSize = 1;
        }
        private ArithmeticQueue(sbyte resultSize, bool hasDecimal, LinkedList<ArithmeticOperator> operators, LinkedList<ArithmeticOperand> operands) {
            p_Operators = operators;
            p_Operands = operands;
            p_ResultSize = resultSize;
            p_HasDecimal = hasDecimal;
        }

        public ArithmeticQueue AddOperation(ArithmeticOperator op, ArithmeticOperand operand) {
            //valid call? (there must be operands already) otherwise this call would
            //make the arithmatic queue invalid.
            if (p_Operands.Count == 0) {
                throw new Exception("Cannot add on to chain when their is no operands to chain off. Consider using AddOperation(op,opAnd1,opAnd2).");
            }

            //operand larger than the result size? if so, set the result size to that of the operand
            sbyte operandSize = 0;
            if (operand.IsNumeric) {
                if (operand.IsDecimal) { p_HasDecimal = true; }
                operandSize = ArithmeticNumeric.SizeOfNumericObj(operand.Value); 
            }
            if (operandSize > p_ResultSize) { p_ResultSize = operandSize; }
            
            //add it
            p_Operators.AddLast(op);
            p_Operands.AddLast(operand);

            return this;
        }
        public ArithmeticQueue AddOperation(ArithmeticOperator op, ArithmeticOperand opAnd1, ArithmeticOperand opAnd2) { 
            //there must be no operators/operands present since it would otherwise
            //create an invalid calculation queue
            if (p_Operands.Count != 0) {
                throw new Exception("This call must be used only when there are no operands to chain off. Consider AddOperation(op,operand)");
            }

            //operand larger than the result size? if so, set the result size to that of the operand
            sbyte opAnd1Size = 0;
            sbyte opAnd2Size = 0;
            if (opAnd1.IsNumeric) {
                if (opAnd1.IsDecimal) { p_HasDecimal = true; }
                opAnd1Size = ArithmeticNumeric.SizeOfNumericObj(opAnd1.Value); 
            }
            if (opAnd2.IsNumeric) {
                if (opAnd2.IsDecimal) { p_HasDecimal = true; }
                opAnd2Size = ArithmeticNumeric.SizeOfNumericObj(opAnd2.Value); 
            }
            if (opAnd1Size > p_ResultSize) { p_ResultSize = opAnd1Size; }
            if (opAnd2Size > p_ResultSize) { p_ResultSize = opAnd2Size; }
            
            //add it
            p_Operators.AddLast(op);
            p_Operands.AddLast(opAnd1);
            p_Operands.AddLast(opAnd2);

            return this;
        }

        public void Clear() {
            p_Operands.Clear();
            p_Operators.Clear();

            p_ResultSize = 1;
            p_HasDecimal = false;
        }

        #region Calculate function
        public static ArithmeticNumeric Calculate(string data) {
            return Calculate(data, new LinkedList<ArithmeticSubstitute>());
        }
        public static ArithmeticNumeric Calculate(byte[] data) {
            return Calculate(data, new LinkedList<ArithmeticSubstitute>());
        }
        public static ArithmeticNumeric Calculate(ref byte* data, int length) {
            return Calculate(ref data, length, new LinkedList<ArithmeticSubstitute>());
        }
        public static ArithmeticNumeric Calculate(ref byte* data, byte* dataEnd) {
            return Calculate(ref data, dataEnd, new LinkedList<ArithmeticSubstitute>());
        }

        public static ArithmeticNumeric Calculate(string data, LinkedList<ArithmeticSubstitute> substitutes) {
            return Calculate(Encoding.ASCII.GetBytes(data), substitutes);
        }
        public static ArithmeticNumeric Calculate(byte[] data, LinkedList<ArithmeticSubstitute> substitutes) {
            fixed (byte* fixedPtr = data) {
                byte* ptr = fixedPtr;
                return Calculate(ref ptr, ptr + data.Length, substitutes);
            }
        }
        public static ArithmeticNumeric Calculate(ref byte* data, int length, LinkedList<ArithmeticSubstitute> substitutes) {
            return Calculate(ref data, data + length, substitutes);
        }
        public static ArithmeticNumeric Calculate(ref byte* data, byte* dataEnd, LinkedList<ArithmeticSubstitute> substitutes) {
            ArithmeticQueue scope = ArithmeticQueue.Parse(ref data, dataEnd);
            return scope.Calculate(substitutes);
        }

        public ArithmeticNumeric Calculate() {
            return Calculate(new LinkedList<ArithmeticSubstitute>());
        }
        public ArithmeticNumeric Calculate(LinkedList<ArithmeticSubstitute> substitutes) {
            //grab the first operator, first operand and first scope so we can cycle through
            //them easily.
            LinkedListNode<ArithmeticOperand> currentOperand = p_Operands.First;
            LinkedListNode<ArithmeticOperator> currentOperator = p_Operators.First;

            //if there are no operators, just process every scope/operand
            if (currentOperator == null) {
                if (currentOperand != null) {
                    ArithmeticOperand opAnd = currentOperand.Value;
                    processOperand(ref opAnd, substitutes);
                    return getOperandValue(opAnd);
                }
                return 0;
            }

            //define the return buffer (start as the first operand)
            ArithmeticOperand firstOpAnd = currentOperand.Value;
            currentOperand = currentOperand.Next;
            processOperand(ref firstOpAnd, substitutes);
            ArithmeticNumeric buffer = ArithmeticNumeric.CreateOfSize(p_ResultSize, p_HasDecimal);
            buffer += getOperandValue(firstOpAnd);

            //iterate through the operators
            while (currentOperator != null) {
                //get the operator/operand
                ArithmeticOperator op = currentOperator.Value;
                ArithmeticOperand opAnd = currentOperand.Value;

                //process the operand for scope/substitution
                processOperand(ref opAnd, substitutes);
                
                //perform calculation
                performCalc(ref buffer, op, opAnd);

                //seek to the next operator/operand
                currentOperator = currentOperator.Next;
                currentOperand = currentOperand.Next;
            }
            return buffer;
        }
        #endregion

        #region Parse
        public static ArithmeticQueue Parse(string data) {
            return Parse(Encoding.ASCII.GetBytes(data));
        }
        public static ArithmeticQueue Parse(byte[] data) {
            fixed (byte* fixedPtr = data) {
                byte* ptr = fixedPtr;
                return Parse(ref ptr, fixedPtr + data.Length);
            }
        }
        public static ArithmeticQueue Parse(ref byte* data, int length) {
            return Parse(ref data, data + length);
        }
        public static ArithmeticQueue Parse(ref byte* data, byte* dataEnd) {
            //define the list of scopes, operators and operands
            LinkedList<ArithmeticOperator> operators = new LinkedList<ArithmeticOperator>();
            LinkedList<ArithmeticOperand> operands = new LinkedList<ArithmeticOperand>();
            
            //define the size of the numerical object the queue will return when
            //the calculate function is called (default of 1 (byte))
            sbyte resultSize = 1;
            bool containsDecimal = false;

            //iterate through the data
            bool negativeFlag = false;
            while (data < dataEnd) { 
                //skip whitespaces so we are on a character
                if (Helpers.SkipWhitespaces(ref data, dataEnd)) { break; }

                //end the scope?
                if (*data == ')') { data++; break; }

                #region operator?
                ArithmeticOperator op = parseOperator(*data);
                if (op != ArithmeticOperator.None) {
                    //make sure we do not process a negative number
                    //and treat it as an operator
                    bool isNegative = false;
                    if (op == ArithmeticOperator.Subtract) {
                        isNegative = operators.Count == operands.Count;
                        negativeFlag = isNegative;
                    }

                    if (!isNegative) {
                        operators.AddLast(op);
                    }
                    data++;
                    continue;
                }
                #endregion

                #region scope?
                if (*data == '(') {
                    
                    //was there an operand behind the substitute?
                    //if not, (e.g 5(5)) we multiple the previous operand with the scope
                    if (operands.Count != operators.Count) {
                        operators.AddLast(ArithmeticOperator.Multiply);
                    }

                    data++;
                    ArithmeticQueue scope = Parse(ref data, dataEnd);
                    operands.AddLast(new ArithmeticOperand(scope, negativeFlag));
                    negativeFlag = false;
                    continue;
                }
                #endregion

                #region numeric?
                if (*data >= '0' && *data <= '9') {
                    byte* numPtr = data;
                    byte* numPtrEnd = dataEnd - 1;
                    
                    //read the numeric
                    bool isDecimal = false;
                    while (data < dataEnd) {
                        //decimal?
                        if (*data == '.') {                             
                            isDecimal = true;
                            containsDecimal = true;
                            data++; 
                            continue; 
                        }

                        //end of numerical?
                        if (!((*data >= '0' && *data <= '9'))) {
                            numPtrEnd = data - 1;
                            break;
                        }

                        data++;
                    }

                    //calculate how much memory this number will take
                    //and see if the current result size can fit it, if 
                    //not, expand it
                    byte charLength = (byte)((numPtrEnd - numPtr) + 1);
                    sbyte memSize = ArithmeticNumeric.RequiredMemory(charLength, isDecimal);
                    if (memSize > resultSize) { resultSize = memSize; }

                    //add the number as an operand
                    string numStr = Helpers.ReadString(numPtr, numPtrEnd);
                    ArithmeticNumeric opValue = ArithmeticNumeric.FromString(numStr, isDecimal, charLength);
                    operands.AddLast(new ArithmeticOperand(opValue, negativeFlag));
                    negativeFlag = false;
                    continue;
                }
                #endregion

                #region substitute?
                else {
                    //was there an operand behind the substitute?
                    //if not, (e.g 5n) we multiple the previous operand with n
                    if (operands.Count != operators.Count) {
                        operators.AddLast(ArithmeticOperator.Multiply);
                    }

                    operands.AddLast(new ArithmeticOperand((char)*data, negativeFlag));
                    negativeFlag = false;
                    data++;


                }
                #endregion
            }

            return new ArithmeticQueue(resultSize, containsDecimal, operators, operands);
        }
        #endregion

        public override string ToString() {
            LinkedListNode<ArithmeticOperator> currentOperator = p_Operators.First;
            LinkedListNode<ArithmeticOperand> currentOperand = p_Operands.First;

            //just a scope/number
            if (currentOperator == null) {
                if (currentOperand != null) {
                    string ret = currentOperand.Value.ToString();
                    if (currentOperand.Value.IsScope) {
                        ret = "(" + ret + ")";
                    }
                    if (currentOperand.Value.IsNegative) {
                        ret = "-" + ret;
                    }
                    return ret;
                }
                return "0";
            }

            //define the return buffer and set it to the first operand
            ArithmeticOperand firstOperand = currentOperand.Value;
            currentOperand = currentOperand.Next;
            string buffer = firstOperand.ToString();
            if (firstOperand.IsNegative) { buffer = "-" + buffer; }

            //iterate over the operators
            while (currentOperator != null) {
                ArithmeticOperator op = currentOperator.Value;

                //get the operand
                ArithmeticOperand opAnd = currentOperand.Value;

                //define what character represents the operator
                char opChar = '\0';
                switch (op) {
                    case ArithmeticOperator.Addition: opChar = '+'; break;
                    case ArithmeticOperator.Subtract: opChar = '-'; break;
                    case ArithmeticOperator.Multiply: opChar = '*'; break;
                    case ArithmeticOperator.Divide: opChar = '/'; break;
                    case ArithmeticOperator.Modulus: opChar = '%'; break;
                    case ArithmeticOperator.Power: opChar = '^'; break;
                }
                
                //is it a multiple+scope/substitute? if so, we have it
                //presented without the * (e.g 5n or 5(5))
                if (op == ArithmeticOperator.Multiply && 
                    (opAnd.IsSubstitution || opAnd.IsScope)) {
                        opChar = '\0';
                }

                //define what the operand string is
                //if the operand is a scope, add the scope parentheses
                string opAndString = opAnd.ToString();
                if (opAnd.IsScope) {
                    opAndString = "(" + opAndString + ")";
                }
                if (opAnd.IsNegative) {
                    opAndString = "-" + opAndString;
                }

                //add to the buffer
                buffer += (opChar == '\0' ? "" : opChar.ToString()) + opAndString;
                currentOperator = currentOperator.Next;
                currentOperand = currentOperand.Next;
            }

            return buffer;
        }

        private ArithmeticNumeric getOperandValue(ArithmeticOperand operand) {
            ArithmeticNumeric val = (ArithmeticNumeric)operand.Value;
            if (operand.IsNegative) { val = -val; }
            return val;
        }
        private void processOperand(ref ArithmeticOperand operand,LinkedList<ArithmeticSubstitute> subs) {
            if (operand.IsSubstitution) { 
                ArithmeticOperand newOpAnd = processSubstitute(operand, subs);
                if (operand.IsNegative) { newOpAnd.IsNegative = true; }
                operand = newOpAnd;
                return;
            }
            if (operand.IsScope) { 
                ArithmeticNumeric ret = ((ArithmeticQueue)operand.Value).Calculate(subs);
                operand = new ArithmeticOperand(ret, operand.IsNegative);
            }
        }
        private static ArithmeticOperator parseOperator(byte b) {
            switch (b) {
                case (byte)'+': return ArithmeticOperator.Addition;
                case (byte)'-': return ArithmeticOperator.Subtract;
                case (byte)'*': return ArithmeticOperator.Multiply;
                case (byte)'/': return ArithmeticOperator.Divide;
                case (byte)'%': return ArithmeticOperator.Subtract;
                case (byte)'^': return ArithmeticOperator.Power;

                default: return ArithmeticOperator.None;
            }
        }
        private ArithmeticOperand processSubstitute(ArithmeticOperand operand, LinkedList<ArithmeticSubstitute> sub) {
            //get the name which we look for
            char name = (char)operand.Value;

            //look for the name
            IEnumerator<ArithmeticSubstitute> e = sub.GetEnumerator();
            while (e.MoveNext()) {
                ArithmeticSubstitute current = e.Current;
                if (current.Name == name) {
                    e.Dispose();
                    return current.Operand;
                }
            }

            //not found
            e.Dispose();
            throw new Exception("Substitute '" + name + "' was not found");
        }
        private void performCalc(ref ArithmeticNumeric current, ArithmeticOperator op, ArithmeticOperand opAnd) {
            //get the operand as decimal
            ArithmeticNumeric opAndDecimal = getOperandValue(opAnd);

            //perform the calculation
            switch (op) {
                case ArithmeticOperator.Addition: current += opAndDecimal; break;
                case ArithmeticOperator.Subtract: current -= opAndDecimal; break;
                case ArithmeticOperator.Multiply: current *= opAndDecimal; break;
                case ArithmeticOperator.Divide: current /= opAndDecimal; break;
                case ArithmeticOperator.Modulus: current %= opAndDecimal; break;
                case ArithmeticOperator.Power: current = Math.Pow(current, opAndDecimal); break;
            }
        }
    }
}