namespace Lipsis.Core {
    public struct CSSRule {
        private string p_Name;
        private string p_Value;

        internal CSSRule(string name, string value) {
            p_Name = name;
            p_Value = value;
        }

        public string Name { get { return p_Name; } }
        public string Value { 
            get { return p_Value; }
            set { p_Value = value; }
        }

        public override string ToString() {
            return
                p_Name + ": " +
                p_Value + ";";
        }
    }
}