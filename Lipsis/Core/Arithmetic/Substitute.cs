using System;

namespace Lipsis.Core {
    public class ArithmeticSubstitute {
        private ArithmeticOperand p_Operand;
        private char p_Name;

        public ArithmeticSubstitute(ArithmeticOperand operand, char name) {
            //operand cannot be another substitute
            if (operand.IsSubstitution) {
                throw new Exception("Invalid operand");
            }

            p_Operand = operand;
            p_Name = name;
        }

        public char Name { get { return p_Name; } }
        public ArithmeticOperand Operand { 
            get { return p_Operand; }
            set { p_Operand = value; }
        }

        public override string ToString() {
            return p_Name + "=" + p_Operand.ToString();
        }
    }
}