namespace Lipsis.Languages.CSS {
    public struct CSSRule {
        private string p_Name;
        private string p_Value;
        private bool p_Important;

        internal CSSRule(string name, string value, bool important) {
            p_Name = name;
            p_Value = value;
            p_Important = important;
        }

        public string Name { get { return p_Name; } }
        public string Value { 
            get { return p_Value; }
            set { p_Value = value; }
        }
        public bool Important { get { return p_Important; } }

        public override string ToString() {
            return
                p_Name + ": " +
                p_Value + ";";
        }
    }
}