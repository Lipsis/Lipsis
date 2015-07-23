using System;

namespace Lipsis.Languages.CSS {

    [Flags]
    public enum CSSPseudoElement { 
        None = 0,
        After = 0x1,
        Before = 0x2,
        FirstLetter = 0x4,
        FirstLine = 0x8,
        Selection = 0x10,
        Backdrop = 0x20,


        _LIP_MIN = After,
        _LIP_MAX = Backdrop
    }
}