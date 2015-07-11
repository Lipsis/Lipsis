namespace Lipsis.Languages.CSS {
    public enum CSSSelectorPreSelectorRelationship { 
        None = 0,

        GeneralChild = 1,           //a b
        GeneralSibling = 2,         //a~b
        ImmidiateChild = 3,         //a>b
        AdjacentSibling = 4,        //a+b
    }
}