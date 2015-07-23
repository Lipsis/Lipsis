using System;

namespace Lipsis.Languages.CSS {
    /*
        Pseudo classes that have arguments are at 0x10000000000 and beyond
    */

    [Flags]
    public enum CSSPseudoClass : long {
        None            = 0x0,
        
        Default         = 0x1,
        Active          = 0x2,
        Checked         = 0x4,
        Disabled        = 0x8,
        Empty           = 0x10,
        Enabled         = 0x20,
        First           = 0x40,
        FirstChild      = 0x80,
        FirstOfType     = 0x100,
        FullScreen      = 0x200,
        Focus           = 0x400,
        Hover           = 0x800,
        Indeterminate   = 0x1000,
        InRange         = 0x2000,
        Invalid         = 0x4000,
        LastChild       = 0x8000,
        LastOfType      = 0x10000,
        Left            = 0x20000,
        Link            = 0x40000,
        OnlyChild       = 0x80000,
        OnlyOfType      = 0x100000,
        Optional        = 0x200000,
        OutOfRange      = 0x400000,
        ReadOnly        = 0x800000,
        ReadWrite       = 0x1000000,
        Required        = 0x2000000,
        Right           = 0x4000000,
        Root            = 0x8000000,
        Scope           = 0x10000000,
        Target          = 0x20000000,
        Valid           = 0x40000000,
        Visited         = 0x80000000,

        /*Classes that have arguments*/
        Dir             = 0x10000000000,
        Lang            = 0x20000000000,
        Not             = 0x40000000000,
        NthChild        = 0x80000000000,
        NthLastChild    = 0x100000000000,
        NthLastOfType   = 0x200000000000,
        NthOfType       = 0x400000000000,

        /*
            this is so we know where exactly in the enum value 
            we should look for pseudo classes which have arguments
        */

        _LIP_ARG_MIN_VALUE = Dir,
        _LIP_ARG_MAX_VALUE = NthOfType,

        _LIP_NTH_MIN = NthChild,
        _LIP_NTH_MAX = NthOfType,

        _LIP_MIN = Default,
        _LIP_MAX = _LIP_NTH_MAX,
    }   
}