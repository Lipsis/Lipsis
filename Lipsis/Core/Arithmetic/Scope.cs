using System;
using System.Text;
using System.Collections.Generic;

namespace Lipsis.Core {
    public unsafe class ArithmeticScope {
        private LinkedList<ArithmeticOperator> p_Operators;
        private LinkedList<ArithmeticOperand> p_Operands;

        public ArithmeticScope() {
            p_Operators = new LinkedList<ArithmeticOperator>();
            p_Operands = new LinkedList<ArithmeticOperand>();

            //add 0 to the operand so future AddOperation calls
            //will just work from a base value of 0
            p_Operands.AddLast(0);
        }
        private ArithmeticScope(LinkedList<ArithmeticOperator> operators, LinkedList<ArithmeticOperand> operands) {
            p_Operators = operators;
            p_Operands = operands;
        }

        public void AddOperation(ArithmeticOperator op, ArithmeticOperand operand) {
            p_Operators.AddLast(op);
            p_Operands.AddLast(operand);
        }

        #region Calculate function
        public static double Calculate(string data) {
            return Calculate(data, new LinkedList<ArithmeticSubstitute>());
        }
        public static double Calculate(byte[] data) {
            return Calculate(data, new LinkedList<ArithmeticSubstitute>());
        }
        public static double Calculate(ref byte* data, int length) {
            return Calculate(ref data, length, new LinkedList<ArithmeticSubstitute>());
        }
        public static double Calculate(ref byte* data, byte* dataEnd) {
            return Calculate(ref data, dataEnd, new LinkedList<ArithmeticSubstitute>());
        }

        public static double Calculate(string data, LinkedList<ArithmeticSubstitute> substitutes) {
            return Calculate(Encoding.ASCII.GetBytes(data), substitutes);
        }
        public static double Calculate(byte[] data, LinkedList<ArithmeticSubstitute> substitutes) {
            fixed (byte* fixedPtr = data) {
                byte* ptr = fixedPtr;
                return Calculate(ref ptr, ptr + data.Length, substitutes);
            }
        }
        public static double Calculate(ref byte* data, int length, LinkedList<ArithmeticSubstitute> substitutes) {
            return Calculate(ref data, data + length, substitutes);
        }
        public static double Calculate(ref byte* data, byte* dataEnd, LinkedList<ArithmeticSubstitute> substitutes) {
            ArithmeticScope scope = ArithmeticScope.Parse(ref data, dataEnd);
            return scope.Calculate(substitutes);
        }

        public double Calculate() {
            return Calculate(new LinkedList<ArithmeticSubstitute>());
        }
        public double Calculate(LinkedList<ArithmeticSubstitute> substitutes) {
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
            double buffer = getOperandValue(firstOpAnd);

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
        public static ArithmeticScope Parse(string data) {
            return Parse(Encoding.ASCII.GetBytes(data));
        }
        public static ArithmeticScope Parse(byte[] data) {
            fixed (byte* fixedPtr = data) {
                byte* ptr = fixedPtr;
                return Parse(ref ptr, fixedPtr + data.Length);
            }
        }
        public static ArithmeticScope Parse(ref byte* data, int length) {
            return Parse(ref data, data + length);
        }
        public static ArithmeticScope Parse(ref byte* data, byte* dataEnd) {
            //define the list of scopes, operators and operands
            LinkedList<ArithmeticOperator> operators = new LinkedList<ArithmeticOperator>();
            LinkedList<ArithmeticOperand> operands = new LinkedList<ArithmeticOperand>();
            
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
                    ArithmeticScope scope = Parse(ref data, dataEnd);
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

                    //add the number as an operand
                    object opValue;
                    string numStr = Helpers.ReadString(numPtr, numPtrEnd);
                    if (isDecimal) { opValue = Convert.ToDouble(numStr); }
                    else { opValue = Convert.ToInt64(numStr); }
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

            return new ArithmeticScope(operators, operands);
        }
        #endregion

        public override string ToString() {
            LinkedListNode<ArithmeticOperator> currentOperator = p_Operators.First;
            LinkedListNode<ArithmeticOperand> currentOperand = p_Operands.First;

            //just a scope/number
            if (currentOperator == null) {
                if (currentOperand != null) {
                    string ret = currentOperand.Value.Value.ToString();
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
            string buffer = firstOperand.Value.ToString();
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
                string opAndString = opAnd.Value.ToString();
                if (opAnd.IsScope) {
                    opAndString = "(" + opAndString + ")";
                }
                if (opAnd.IsNegative) {
                    opAndString = "-" + opAndString;
                }

                //add to the buffer
                buffer += (opChar == '\0' ? "" : opChar.ToString()) + opAndString;
                currentOperator = currentOperator.Next;
            }

            return buffer;
        }

        private double getOperandValue(ArithmeticOperand operand) {
            double val = Convert.ToDouble(operand.Value);
            if (operand.IsNegative) { val = -val; }
            return val;
        }
        private void processOperand(ref ArithmeticOperand operand,LinkedList<ArithmeticSubstitute> subs) {
            if (operand.IsSubstitution) { 
                operand = processSubstitute(operand, subs);                
            }
            if (operand.IsScope) { 
                double ret = ((ArithmeticScope)operand.Value).Calculate(subs);
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
        private void performCalc(ref double current, ArithmeticOperator op, ArithmeticOperand opAnd) {
            //get the operand as decimal
            double opAndDecimal = getOperandValue(opAnd);

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