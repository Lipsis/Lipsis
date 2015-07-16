using System;
using System.Text;
using System.Collections.Generic;

namespace Lipsis.Core {
    public unsafe class ArithmeticQueue {
        private LinkedList<ArithmeticOperator> p_Operators;
        private LinkedList<ArithmeticOperand> p_Operands;
        private sbyte p_ResultSize;
        private bool p_HasDecimal, p_BIDMASSorted;
        
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
                throw new Exception("Cannot add on to chain when there is no operands to chain off. Consider using AddOperation(op,opAnd1,opAnd2).");
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
            p_BIDMASSorted = false;

            //division
            if (op == ArithmeticOperator.Divide) { HasDecimal = true; }
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
            p_BIDMASSorted = false;

            //division?
            if (op == ArithmeticOperator.Divide) { HasDecimal = true; }
            return this;
        }

        public void SortBIDMAS() {
            //already sorted?/nothing to sort?
            if (p_BIDMASSorted || p_Operators.Count == 0) { return; }
            
            //process each operator in order of BIDMAS
            processBIDMAS(ArithmeticOperator.Power);
            processBIDMAS(ArithmeticOperator.Divide);
            processBIDMAS(ArithmeticOperator.Modulus);
            processBIDMAS(ArithmeticOperator.Multiply);
            processBIDMAS(ArithmeticOperator.Addition);
            processBIDMAS(ArithmeticOperator.Subtract);

            p_BIDMASSorted = true;
        }
        private void processBIDMAS(ArithmeticOperator op) { 

            //iterate through the operators
            LinkedListNode<ArithmeticOperator> currentOperator = p_Operators.First;
            LinkedListNode<ArithmeticOperand> currentOperand = p_Operands.First;
            while (currentOperator != null) {
                //operator we want to process?
                ArithmeticOperator currentOp = currentOperator.Value;
                if (op != currentOp) {
                    currentOperator = currentOperator.Next;
                    currentOperand = currentOperand.Next;
                    continue;
                }
                
                //get the 2 operands for the operator
                ArithmeticOperand opAnd1 = currentOperand.Value;
                ArithmeticOperand opAnd2 = currentOperand.Next.Value;

                //group this operator into a scope
                ArithmeticQueue queue = new ArithmeticQueue();
                queue.AddOperation(currentOp, opAnd1, opAnd2);
                
                //replace the operand with the queue
                currentOperand.Value = new ArithmeticOperand(queue);
                                
                //since we have processed the second operand and
                //the operator, remove it
                LinkedListNode<ArithmeticOperator> nextOp = currentOperator.Next;
                p_Operands.Remove(currentOperand.Next);
                p_Operators.Remove(currentOperator);
                currentOperator = nextOp;
            }
            

        }

        public void Clear() {
            p_Operands.Clear();
            p_Operators.Clear();

            p_ResultSize = 1;
            p_HasDecimal = false;
        }

        public sbyte ResultSize {
            get { return p_ResultSize; }
            set { p_ResultSize = value; }
        }
        public bool HasDecimal {
            get { return p_HasDecimal; }
            set { 
                //value changed?
                if (p_HasDecimal == value) { return; }

                p_HasDecimal = value; 
                
                //set every queue operand to have decimal
                IEnumerator<ArithmeticOperand> e = p_Operands.GetEnumerator();
                while (e.MoveNext()) {
                    ArithmeticOperand current = e.Current;
                    if (!current.IsQueue) { continue; }
                    ((ArithmeticQueue)current.Value).HasDecimal = value;
                }
                e.Dispose();
            }
        }

        public int QueueCount {
            get { 
                //count how many queue operands we have
                int buffer = 0;
                IEnumerator<ArithmeticOperand> e = p_Operands.GetEnumerator();
                while (e.MoveNext()) {
                    if (e.Current.IsQueue) {
                        buffer++;
                    }
                }
                e.Dispose();
                return buffer;
            }
        }

        #region Calculate function

        public static ArithmeticNumeric Calculate(string data) {
            return Calculate(data, true);
        }
        public static ArithmeticNumeric Calculate(byte[] data) {
            return Calculate(data, true);
        }
        public static ArithmeticNumeric Calculate(ref byte* data, int length) {
            return Calculate(ref data, data + length, true);
        }
        public static ArithmeticNumeric Calculate(ref byte* data, byte* dataEnd) {
            return Calculate(ref data, dataEnd, true);
        }
        
        public static ArithmeticNumeric Calculate(string data, bool useBIDMAS) {
            return Calculate(data, new LinkedList<ArithmeticSubstitute>(), useBIDMAS);
        }
        public static ArithmeticNumeric Calculate(byte[] data, bool useBIDMAS) {
            return Calculate(data, new LinkedList<ArithmeticSubstitute>(), useBIDMAS);
        }
        public static ArithmeticNumeric Calculate(ref byte* data, int length, bool useBIDMAS) {
            return Calculate(ref data, data + length, new LinkedList<ArithmeticSubstitute>(), useBIDMAS);
        }
        public static ArithmeticNumeric Calculate(ref byte* data, byte* dataEnd, bool useBIDMAS) {
            return Calculate(ref data, dataEnd, new LinkedList<ArithmeticSubstitute>(), useBIDMAS);
        }

        public static ArithmeticNumeric Calculate(string data, LinkedList<ArithmeticSubstitute> substitutes) {
            return Calculate(data, substitutes, true);
        }
        public static ArithmeticNumeric Calculate(byte[] data, LinkedList<ArithmeticSubstitute> substitutes) {
            return Calculate(data, substitutes, true);
        }
        public static ArithmeticNumeric Calculate(ref byte* data, int length, LinkedList<ArithmeticSubstitute> substitutes) {
            return Calculate(ref data, data + length, substitutes, true);
        }
        public static ArithmeticNumeric Calculate(ref byte* data, byte* dataEnd, LinkedList<ArithmeticSubstitute> substitutes) {
            return Calculate(ref data, dataEnd, substitutes, true);
        }

        public static ArithmeticNumeric Calculate(string data, LinkedList<ArithmeticSubstitute> substitutes, bool useBIDMAS) {
            return Calculate(Encoding.ASCII.GetBytes(data), substitutes, useBIDMAS);
        }
        public static ArithmeticNumeric Calculate(byte[] data, LinkedList<ArithmeticSubstitute> substitutes, bool useBIDMAS) {
            fixed (byte* fixedPtr = data) {
                byte* ptr = fixedPtr;
                return Calculate(ref ptr, ptr + data.Length, substitutes, useBIDMAS);
            }
        }
        public static ArithmeticNumeric Calculate(ref byte* data, int length, LinkedList<ArithmeticSubstitute> substitutes, bool useBIDMAS) {
            return Calculate(ref data, data + length, substitutes, useBIDMAS);
        }
        public static ArithmeticNumeric Calculate(ref byte* data, byte* dataEnd, LinkedList<ArithmeticSubstitute> substitutes, bool useBIDMAS) {
            ArithmeticQueue scope = ArithmeticQueue.Parse(ref data, dataEnd, useBIDMAS);
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
                    ArithmeticNumeric ret = ArithmeticNumeric.Zero;
                    processOperand(ref opAnd, false, ref ret, substitutes);
                    return getOperandValue(opAnd);
                }
                return 0;
            }

            //define the return buffer (start as the first operand)
            ArithmeticNumeric buffer = ArithmeticNumeric.CreateOfSize(p_ResultSize, p_HasDecimal);
            ArithmeticOperand firstOpAnd = currentOperand.Value;
            currentOperand = currentOperand.Next;
            processOperand(ref firstOpAnd, true, ref buffer, substitutes);
            buffer += getOperandValue(firstOpAnd);

            //iterate through the operators
            while (currentOperator != null) {
                //get the operator/operand
                ArithmeticOperator op = currentOperator.Value;
                ArithmeticOperand opAnd = currentOperand.Value;

                //process the operand for scope/substitution
                processOperand(ref opAnd, true, ref buffer, substitutes);
                
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
            return Parse(data, new LinkedList<ArithmeticFunction>());
        }
        public static ArithmeticQueue Parse(byte[] data) {
            return Parse(data, new LinkedList<ArithmeticFunction>());
        }
        public static ArithmeticQueue Parse(ref byte* data, int length) {
            return Parse(ref data, length, new LinkedList<ArithmeticFunction>());
        }
        public static ArithmeticQueue Parse(ref byte* data, byte* dataEnd) {
            return Parse(ref data, dataEnd, new LinkedList<ArithmeticFunction>());
        }

        public static ArithmeticQueue Parse(string data, LinkedList<ArithmeticFunction> functions) {
            return Parse(data, true, functions);
        }
        public static ArithmeticQueue Parse(byte[] data, LinkedList<ArithmeticFunction> functions) {
            return Parse(data, true, functions);
        }
        public static ArithmeticQueue Parse(ref byte* data, int length, LinkedList<ArithmeticFunction> functions) {
            return Parse(ref data, length, true, functions);
        }
        public static ArithmeticQueue Parse(ref byte* data, byte* dataEnd, LinkedList<ArithmeticFunction> functions) {
            return Parse(ref data, dataEnd, true, functions);
        }

        public static ArithmeticQueue Parse(string data, bool useBIDMAS) {
            return Parse(data, useBIDMAS, new LinkedList<ArithmeticFunction>());
        }
        public static ArithmeticQueue Parse(byte[] data, bool useBIDMAS) {
            return Parse(data, useBIDMAS, new LinkedList<ArithmeticFunction>());
        }
        public static ArithmeticQueue Parse(ref byte* data, int length, bool useBIDMAS) {
            return Parse(ref data, length, useBIDMAS, new LinkedList<ArithmeticFunction>());
        }
        public static ArithmeticQueue Parse(ref byte* data, byte* dataEnd, bool useBIDMAS) {
            return Parse(ref data, dataEnd, useBIDMAS, new LinkedList<ArithmeticFunction>());
        }

        public static ArithmeticQueue Parse(string data, bool useBIDMAS, LinkedList<ArithmeticFunction> functions) {
            return Parse(Encoding.ASCII.GetBytes(data), useBIDMAS, functions);
        }
        public static ArithmeticQueue Parse(byte[] data, bool useBIDMAS, LinkedList<ArithmeticFunction> functions) {
            fixed (byte* fixedPtr = data) {
                byte* ptr = fixedPtr;
                return Parse(ref ptr, fixedPtr + data.Length, useBIDMAS, functions);
            }
        }
        public static ArithmeticQueue Parse(ref byte* data, int length, bool useBIDMAS, LinkedList<ArithmeticFunction> functions) {
            return Parse(ref data, data + length, useBIDMAS, functions);
        }
        public static ArithmeticQueue Parse(ref byte* data, byte* dataEnd, bool useBIDMAS, LinkedList<ArithmeticFunction> functions) {
            //define the list of scopes, operators and operands
            LinkedList<ArithmeticOperator> operators = new LinkedList<ArithmeticOperator>();
            LinkedList<ArithmeticOperand> operands = new LinkedList<ArithmeticOperand>();
        
            //define the size of the numerical object the queue will return when
            //the calculate function is called (default of 1 (byte))
            sbyte resultSize = 1;
            bool containsDecimal = false;

            //any functions in the list?
            bool functionsListed = functions.Count != 0;

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
                    //if it's division, we make sure we don't lose 
                    //precision when dividing operators (e.g 1/3)
                    if (op == ArithmeticOperator.Divide) { containsDecimal = true; }
                    
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

                #region function?
                if (functionsListed && isAlphabetic(*data)) {         
                    //read until an open queue
                    byte* functionNameStart = data;
                    byte* functionNameEnd = (byte*)0;
                    byte* dataCopy = data;
                    bool foundScope = false;
                    while (dataCopy < dataEnd) {
                        //scope?
                        if (*dataCopy == '(') {
                            foundScope = true;
                            functionNameEnd = dataCopy - 1;
                            dataCopy++;
                            break;
                        }

                        //not a whitespace?
                        if (*dataCopy != ' ' &&
                            *dataCopy != '\n' &&
                            *dataCopy != '\r' &&
                            *dataCopy != '\t' &&
                            !isAlphabetic(*dataCopy)) {
                            break;
                        }
                        dataCopy++;
                    }

                    //did we find a scope?
                    if (foundScope) {                         
                        //trim the function name to remove whitespace
                        while (functionNameEnd > data &&
                               !isAlphabetic(*functionNameEnd)) { functionNameEnd--; }

                        //name must be at least 2 characters wide
                        if (functionNameStart != functionNameEnd) {                            
                            //it's a function, lookup its name
                            data = dataCopy;
                            string functionName = Helpers.ReadString(functionNameStart, functionNameEnd);

                            //find the function from the functions list
                            IEnumerator<ArithmeticFunction> e = functions.GetEnumerator();
                            ArithmeticFunction functionBase = null;
                            while (e.MoveNext()) {
                                ArithmeticFunction current = e.Current;
                                if (current.Name == functionName) {
                                    functionBase = current;
                                    break;
                                }
                            }

                            //found the function?
                            if (functionBase == null) {
                                throw new Exception("Function \"" + functionName + "\" does not exist!");
                            }

                            //was there an operand behind the substitute?
                            //if not, (e.g 5(5)) we multiple the previous operand with the scope
                            if (operands.Count != operators.Count) {
                                operators.AddLast(ArithmeticOperator.Multiply);
                            }

                            //parse the function call arguments
                            ArithmeticQueue args = ArithmeticQueue.Parse(ref data, dataEnd, useBIDMAS, functions);

                            //add the function as an operand
                            ArithmeticFunction function = functionBase.Create(args);
                            operands.AddLast(new ArithmeticOperand(function, negativeFlag));
                            negativeFlag = false;
                            continue;
                        }
                    }
                }                

                #endregion

                #region queue
                if (*data == '(') {
                    //was there an operand behind the substitute?
                    //if not, (e.g 5(5)) we multiple the previous operand with the scope
                    if (operands.Count != operators.Count) {
                        operators.AddLast(ArithmeticOperator.Multiply);
                    }

                    data++;
                    ArithmeticQueue scope = Parse(ref data, dataEnd, functions);
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

            ArithmeticQueue buffer = new ArithmeticQueue(resultSize, containsDecimal, operators, operands);

            //bidmas?
            if (useBIDMAS) {
                buffer.SortBIDMAS();
            }

            return buffer;
        }
        #endregion

        public override string ToString() {
            LinkedListNode<ArithmeticOperator> currentOperator = p_Operators.First;
            LinkedListNode<ArithmeticOperand> currentOperand = p_Operands.First;

            //just a scope/number
            if (currentOperator == null) {
                if (currentOperand != null) {
                    string ret = currentOperand.Value.ToString();
                    if (currentOperand.Value.IsQueue) {
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
                    (opAnd.IsSubstitution || opAnd.IsQueue)) {
                        opChar = '\0';
                }

                //define what the operand string is
                //if the operand is a scope, add the scope parentheses
                string opAndString = opAnd.ToString();
                if (opAnd.IsQueue) {
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

        private static bool isAlphabetic(byte b) {

            return (b >= 'A' && b <= 'Z') ||
                   (b >= 'a' && b <= 'z');

        }
        private ArithmeticNumeric getOperandValue(ArithmeticOperand operand) {
            ArithmeticNumeric val = (ArithmeticNumeric)operand.Value;
            if (operand.IsNegative) { val = -val; }
            return val;
        }
        private void processOperand(ref ArithmeticOperand operand, bool canResize, ref ArithmeticNumeric buffer, LinkedList<ArithmeticSubstitute> subs) {
            if (operand.IsSubstitution) { 
                ArithmeticOperand newOpAnd = processSubstitute(operand, subs);

                //do we have to resize the buffer? to store the substitution?
                if (canResize) {
                    ArithmeticNumeric val = (ArithmeticNumeric)newOpAnd.Value;

                    //decimal?
                    if (!buffer.IsDecimal && newOpAnd.IsDecimal) {
                        ArithmeticNumeric bufferCopy = buffer;
                        
                        buffer = ArithmeticNumeric.CreateOfSize(val.Size, true);
                        buffer += bufferCopy;                        
                    }

                    //need more memory?
                    else if (buffer.Size < val.Size) {
                        ArithmeticNumeric bufferCopy = buffer;
                        buffer = ArithmeticNumeric.CreateOfSize(val.Size, newOpAnd.IsDecimal);
                        buffer += bufferCopy;
                    }
                }


                if (operand.IsNegative) { newOpAnd.IsNegative = true; }
                operand = newOpAnd;
                return;
            }
            if (operand.IsQueue) { 
                ArithmeticNumeric ret = ((ArithmeticQueue)operand.Value).Calculate(subs);
                operand = new ArithmeticOperand(ret, operand.IsNegative);
            }
            if (operand.IsFunction) {
                ArithmeticNumeric ret = ((ArithmeticFunction)operand.Value).Call(subs);
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
            char name = (char)operand.Value;
            ArithmeticSubstitute ret;
            bool found = substituteExist(name, sub, out ret);
            if (found) { return ret.Operand; }
            throw new Exception("Substitute '" + name + "' was not found");
        }
        private bool substituteExist(char name, LinkedList<ArithmeticSubstitute> subs, out ArithmeticSubstitute found) {
            found = default(ArithmeticSubstitute);

            //look for the name
            IEnumerator<ArithmeticSubstitute> e = subs.GetEnumerator();
            while (e.MoveNext()) {
                ArithmeticSubstitute current = e.Current;
                if (current.Name == name) {
                    e.Dispose();
                    found = current;
                    return true;
                }
            }

            //not found
            e.Dispose();
            return false;
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
                case ArithmeticOperator.Power:
                    current = Math.Pow(
                        Convert.ToDouble(current.RAWObject), 
                        Convert.ToDouble(opAndDecimal.RAWObject)); 
                    break;
            }
        }


        #region default functions
        private static LinkedList<ArithmeticFunction> p_DefaultFunctions;
        private static Random p_Random = new Random();

        static ArithmeticQueue() {
            p_DefaultFunctions = new LinkedList<ArithmeticFunction>();

            p_DefaultFunctions.AddLast(ArithmeticFunction.FromDelegate("sin", func_sin));
            p_DefaultFunctions.AddLast(ArithmeticFunction.FromDelegate("cos", func_cos));
            p_DefaultFunctions.AddLast(ArithmeticFunction.FromDelegate("tan", func_tan));
            p_DefaultFunctions.AddLast(ArithmeticFunction.FromDelegate("sinh", func_sinh));
            p_DefaultFunctions.AddLast(ArithmeticFunction.FromDelegate("cosh", func_cosh));
            p_DefaultFunctions.AddLast(ArithmeticFunction.FromDelegate("tanh", func_tanh));
            p_DefaultFunctions.AddLast(ArithmeticFunction.FromDelegate("asin", func_asin));
            p_DefaultFunctions.AddLast(ArithmeticFunction.FromDelegate("acos", func_acos));
            p_DefaultFunctions.AddLast(ArithmeticFunction.FromDelegate("atan", func_atan));
            p_DefaultFunctions.AddLast(ArithmeticFunction.FromDelegate("trunc", func_trunc));
            p_DefaultFunctions.AddLast(ArithmeticFunction.FromDelegate("round", func_round));
            p_DefaultFunctions.AddLast(ArithmeticFunction.FromDelegate("ceil", func_ceil));
            p_DefaultFunctions.AddLast(ArithmeticFunction.FromDelegate("floor", func_floor));
            p_DefaultFunctions.AddLast(ArithmeticFunction.FromDelegate("abs", func_abs));
            p_DefaultFunctions.AddLast(ArithmeticFunction.FromDelegate("sqrt", func_sqrt));
            p_DefaultFunctions.AddLast(ArithmeticFunction.FromDelegate("rand", func_rand));
        }

        public static LinkedList<ArithmeticFunction> DefaultFunctions { get { return p_DefaultFunctions; } }

        private static ArithmeticNumeric func_sin(ArithmeticNumeric arg) {
            return Math.Sin(Convert.ToDouble(arg.RAWObject));
        }
        private static ArithmeticNumeric func_cos(ArithmeticNumeric arg) {
            return Math.Cos(Convert.ToDouble(arg.RAWObject));
        }
        private static ArithmeticNumeric func_tan(ArithmeticNumeric arg) {
            return Math.Tan(Convert.ToDouble(arg.RAWObject));
        }
        private static ArithmeticNumeric func_asin(ArithmeticNumeric arg) {
            return Math.Asin(Convert.ToDouble(arg.RAWObject));
        }
        private static ArithmeticNumeric func_acos(ArithmeticNumeric arg) {
            return Math.Acos(Convert.ToDouble(arg.RAWObject));
        }
        private static ArithmeticNumeric func_atan(ArithmeticNumeric arg) {
            return Math.Atan(Convert.ToDouble(arg.RAWObject));
        }
        private static ArithmeticNumeric func_abs(ArithmeticNumeric arg) {
            return Math.Abs(Convert.ToDouble(arg.RAWObject));
        }
        private static ArithmeticNumeric func_sinh(ArithmeticNumeric arg) {
            return Math.Sinh(Convert.ToDouble(arg.RAWObject));
        }
        private static ArithmeticNumeric func_cosh(ArithmeticNumeric arg) {
            return Math.Cosh(Convert.ToDouble(arg.RAWObject));
        }
        private static ArithmeticNumeric func_tanh(ArithmeticNumeric arg) {
            return Math.Tanh(Convert.ToDouble(arg.RAWObject));
        }
        private static ArithmeticNumeric func_trunc(ArithmeticNumeric arg) {
            return Math.Truncate(Convert.ToDouble(arg.RAWObject));
        }
        private static ArithmeticNumeric func_sqrt(ArithmeticNumeric arg) {
            return Math.Sqrt(Convert.ToDouble(arg.RAWObject));
        }
        private static ArithmeticNumeric func_floor(ArithmeticNumeric arg) {
            return Math.Floor(Convert.ToDouble(arg.RAWObject));
        }
        private static ArithmeticNumeric func_ceil(ArithmeticNumeric arg) {
            return Math.Ceiling(Convert.ToDouble(arg.RAWObject));
        }
        private static ArithmeticNumeric func_round(ArithmeticNumeric arg) {
            return Math.Round(Convert.ToDouble(arg.RAWObject));
        }
        private static ArithmeticNumeric func_rand(ArithmeticNumeric arg) {
            return p_Random.NextDouble();
        }
        
        #endregion
    }
}