using System;
using System.Collections.Generic;

using Lipsis.Core;
using Lipsis.Languages.Markup;
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
        private LinkedList<ArithmeticSubstitute> p_Substitutes;
        private ArithmeticSubstitute p_SubstituteN;
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
           
            //does the expression contain any substitutes? 
            //if so, we add "n" as a substitute (IF it is n, if
            //it isn't then it is an invalid nth* class.
            LinkedListNode<ArithmeticOperand> currentOperand = p_Expression.Operands.First;
            while (currentOperand != null) {
                ArithmeticOperand current = currentOperand.Value;
                if (!current.IsSubstitution) {
                    currentOperand = currentOperand.Next;
                    continue; 
                }
                ArithmeticSubstitute sub = current.Value as ArithmeticSubstitute;

                //invalid
                if (sub.Name != 'n') {
                    success = false;
                    break;
                }

                //create a new counter substitute
                p_SubstituteN = new ArithmeticSubstitute(
                    new ArithmeticOperand(0),
                    'n');
                p_Substitutes = new LinkedList<ArithmeticSubstitute>();
                p_Substitutes.AddLast(p_SubstituteN);
                break;
            }


        }

        

        private byte toLower(byte b) {
            if (b >= 'A' && b <= 'Z') {
                return (byte)((b - 'A') + 'a');
            }
            return b;
        }
    }
}