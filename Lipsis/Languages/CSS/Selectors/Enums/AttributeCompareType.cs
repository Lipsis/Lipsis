namespace Lipsis.Languages.CSS {
    public enum CSSSelectorAttributeCompareType { 
        Match = 1,                          //attr=value
        WhitespaceSplitMatch = 2,           //attr~=value
        DashSplitPrefixMatch = 3,           //attr|=value
        PrefixMatch = 4,                    //attr^=value
        SuffixMatch = 5,                    //attr$=value
        Contains = 6,                       //attr*=value
        HasAttribute = 7                    //attr
    }
}