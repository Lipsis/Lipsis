using System;
using System.Collections.Generic;

using Lipsis.Core;

namespace Lipsis.Languages.CSS {
    public sealed class CSSPseudoClass_Nth : ICSSPseudoClassArgument {
        private static ArithmeticFunction p_SqrtFunction;
        static CSSPseudoClass_Nth() {
            p_SqrtFunction = ArithmeticFunction.FromDelegate(
                "sqrt",
                delegate(ArithmeticNumeric arg) {
                    return Math.Sqrt(Convert.ToDouble(arg.RAWObject));
                });
        }

        private ArithmeticQueue p_Reverse;
        private ArithmeticQueue p_Expression;
        private bool p_Odd, p_Even;

        internal unsafe CSSPseudoClass_Nth(byte* data, byte* dataEnd, CSSPseudoClass pseudoClass, out bool success) {
            success = true;
            #region odd/even?
            if (toLower(*data) == 'o' &&
                toLower(*(data + 1)) == 'd' &&
                toLower(*(data + 2)) == 'd') {
                    p_Odd = true;
                    return;
            }
            if (toLower(*data) == 'e' &&
                toLower(*(data + 1)) == 'v' &&
                toLower(*(data + 2)) == 'e' &&
                toLower(*(data + 3)) == 'n') {
                    p_Even = true;
                    return;
            }

            #endregion

            //parse the data as an arithmetic expression
            p_Expression = ArithmeticQueue.Parse(ref data, dataEnd);
            LinkedList<ArithmeticOperator> operators = p_Expression.Operators;
            LinkedList<ArithmeticOperand> operands = p_Expression.Operands;
            ArithmeticOperand firstOperand = operands.First.Value;
            int operatorsCount = operators.Count;
            int operandsCount = operands.Count;

            /*
                We find the reverse of the expression by checking 
                if the expression has a known pattern and we know
                how to reverse it.
                
                If we don't know how to reverse it (meaning the CSS is NON-STANDARD, 
                then when the element is checked for selection, we manually iterate 
                n (n MUST be present)
             
                Note: if ANY of the operators is modulus/power, we CANNOT reverse 
                      (since modulus cannot be reversed).
                      (it is mathmatically possible to reverse powers, but
                       we can't here)
                                 
                Due to how the BIDMAS sort works on the queue, it will put any expression
                with operators into a nested queue.
             
                Here are the known patterns and their reverse
                (note: x = index, op = operator, opop = opposite operator (appropriately matching))
                n                       =       n
                (a [op] n)              =       x [opop] a
                (a [op] n [op] b)       =       (x [opop] b) [opop] n
             
             
                Note: in the reverse expression "n" is used as the result of the original expression
                where the result of the reverse is the value of the original n (we make n the subject)
            */

            #region "n"
            if (operatorsCount == 0 && operandsCount == 1 &&
                firstOperand.IsSubstitution) { 
                    //must be "n", otherwise it's invalid.
                    if ((char)firstOperand.Value != 'n') {
                        success = false;
                        return;
                    }

                    p_Reverse = p_Expression;
                    return;
            }
            #endregion

            //is the first operand a queue and the only operand
            //if so, we select that queue as the expression.
            //if it isn't then we know we don't know how to 
            //reverse it because the upcoming statements are ONLY
            //in one nested queue.
            bool isAloneQueue = firstOperand.IsQueue && 
                           operandsCount == 1 &&
                           operatorsCount == 0;
            if (isAloneQueue) {
                p_Expression = (ArithmeticQueue)firstOperand.Value;
                operators = p_Expression.Operators;
                operands = p_Expression.Operands;
                operatorsCount = operators.Count;
                operandsCount = operands.Count;
            }
            else {
                return;
            }

            //get the first node from the operators and operands
            //list so we can easily cycle through them
            LinkedListNode<ArithmeticOperator> currentOperator = operators.First;
            LinkedListNode<ArithmeticOperand> currentOperand = operands.First;


            #region "(a [op] n)"
            if (operatorsCount == 1 &&
                currentOperand.Value.IsNumeric &&
                currentOperand.Next.Value.IsSubstitution) {
                   ArithmeticOperator op = currentOperator.Value;

                   //we cannot reverse modulus/powers
                   if (op == ArithmeticOperator.Modulus ||
                       op == ArithmeticOperator.Power) { return; }

                   //add the reversed (where we just swap round the operands)
                   op = oppositeOperator(op);
                   p_Reverse = new ArithmeticQueue();
                   p_Reverse.AddOperation(op,
                       currentOperand.Next.Value,
                       currentOperand.Value);
                   return;

            }
            #endregion

            #region "(a [op] n [op] b)"
            if (operatorsCount == 2 &&
                currentOperand.Value.IsNumeric &&
                currentOperand.Next.Value.IsSubstitution && 
                currentOperand.Next.Next.Value.IsNumeric) {
                
                    //get the operands/operators
                    ArithmeticOperator op1 = currentOperator.Value;
                    currentOperator = currentOperator.Next;
                    ArithmeticOperator op2 = currentOperator.Value;

                    ArithmeticOperand a = currentOperand.Value;
                    currentOperand = currentOperand.Next;
                    ArithmeticOperand n = currentOperand.Value;
                    currentOperand = currentOperand.Next;
                    ArithmeticOperand b = currentOperand.Value;
                    

                    //verify it's not a power/modulus which we cannot reverse
                    if (op1 == ArithmeticOperator.Power ||
                       op2 == ArithmeticOperator.Power ||
                       op1 == ArithmeticOperator.Modulus ||
                       op2 == ArithmeticOperator.Modulus) { return; }
                    
                    

            }
            #endregion

            return;
        }

        private ArithmeticOperator oppositeOperator(ArithmeticOperator op) {
            switch (op) {
                case ArithmeticOperator.Addition: return ArithmeticOperator.Subtract;
                case ArithmeticOperator.Subtract: return ArithmeticOperator.Addition;
                case ArithmeticOperator.Multiply: return ArithmeticOperator.Divide;
                case ArithmeticOperator.Divide: return ArithmeticOperator.Multiply;
            }
            return ArithmeticOperator.None;
        }
        private byte toLower(byte b) {
            if (b >= 'A' && b <= 'Z') {
                return (byte)((b - 'A') + 'a');
            }
            return b;
        }
    }
}