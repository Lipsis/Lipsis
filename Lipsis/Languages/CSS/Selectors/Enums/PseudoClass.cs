using System;

namespace Lipsis.Languages.CSS {
    /*
        Pseudo classes that have arguments are at 0x10000000000 and beyond
    */

    [Flags]
    public enum CSSSelectorPseudoClass : long {
        None            = 0x0,
        
        Default         = 0x1,
        Active          = 0x4,
        Checked         = 0x2,
        Disabled        = 0x4,
        Empty           = 0x8,
        Enabled         = 0x10,
        First           = 0x20,
        FirstChild      = 0x40,
        FirstOfType     = 0x80,
        FullScreen      = 0x100,
        Focus           = 0x200,
        Hover           = 0x400,
        Indeterminate   = 0x800,
        InRange         = 0x1000,
        Invalid         = 0x2000,
        LastChild       = 0x4000,
        LastOfType      = 0x8000,
        Left            = 0x10000,
        Link            = 0x20000,
        OnlyChild       = 0x40000,
        OnlyOfType      = 0x80000,
        Optional        = 0x100000,
        OutOfRange      = 0x200000,
        ReadOnly        = 0x400000,
        ReadWrite       = 0x800000,
        Required        = 0x1000000,
        Right           = 0x2000000,
        Root            = 0x4000000,
        Scope           = 0x8000000,
        Target          = 0x10000000,
        Valid           = 0x20000000,
        Visited         = 0x40000000,

        /*Classes that have arguments*/
        Dir             = 0x10000000000,
        Lang            = 0x20000000000,
        Not             = 0x40000000000,
        NthChild        = 0x80000000000,
        NthLastChild    = 0x100000000000,
        NthLastOfType   = 0x200000000000,
        NthOfType       = 0x400000000000,

    }   
}