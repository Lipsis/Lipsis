using System;
using System.Collections.Generic;

using Lipsis.Core;
using Lipsis.Languages.Markup;
namespace Lipsis.Languages.CSS {
    internal sealed class CSSPseudoClass_Nth : ICSSPseudoClassArgument {
        private static ArithmeticFunction p_SqrtFunction;
        static CSSPseudoClass_Nth() {
            p_SqrtFunction = ArithmeticFunction.FromDelegate(
                "sqrt",
                delegate(ArithmeticNumeric arg) {
                    return Math.Sqrt(Convert.ToDouble(arg.RAWObject));
                });
        }

        private ArithmeticQueue p_Expression;
        private int p_ExpressionResult;
        private bool p_Odd, p_Even;
        private bool p_HasNSubstitute = false;

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
            ArithmeticQueue flatten = p_Expression.Flatten();

            //does the expression contain any substitutes? 
            //if so, we add "n" as a substitute (IF it is n, if
            //it isn't then it is an invalid nth* class.
            LinkedListNode<ArithmeticOperand> currentOperand = flatten.Operands.First;
            while (currentOperand != null) {
                //is the operand a substitute?
                ArithmeticOperand current = currentOperand.Value;
                if (!current.IsSubstitution) {
                    currentOperand = currentOperand.Next;
                    continue; 
                }

                
                //invalid? (the name has to be "n")
                if ((char)current.Value != 'n') {
                    success = false;
                    break;
                }

                p_HasNSubstitute = true;
                break;
            }

            //if there is no "n" substitute, pre-calculate the expression
            //so we don't have to constantly calculate everytime we check.
            if (success && !p_HasNSubstitute) {
                p_ExpressionResult = (int)p_Expression.Calculate();
            }
        }


        public bool AppliesTo(MarkupElement originalElement, MarkupElement pipeElement) { 
            //get the index of the element
            int index = pipeElement.Index;

            //odd/even?
            if (p_Odd) { return index % 2 != 0; }
            if (p_Even) { return index % 2 == 0; }

            //substitute for n?
            if (!p_HasNSubstitute) { 
                //index has to match the result of the calculation
                return index == p_ExpressionResult;
            }

            //use the n substitute as an accumulator and
            //see if the result of the expression
            //matches the elements index.
            ArithmeticSubstitute n = new ArithmeticSubstitute(
                new ArithmeticOperand(0),
                'n');
            LinkedList<ArithmeticSubstitute> subs = new LinkedList<ArithmeticSubstitute>();
            subs.AddLast(n);
            for (int c = 0; c <= index; c++) {
                n.Operand.setValue(c, false, true);

                ArithmeticNumeric res = p_Expression.Calculate(subs);
                if (res == index) { return true; }
            }

            //no match
            return false;
        }


        private byte toLower(byte b) {
            if (b >= 'A' && b <= 'Z') {
                return (byte)((b - 'A') + 'a');
            }
            return b;
        }

        public override string ToString() {
            return p_Expression.Flatten().ToString();
        }
    }
}