#ifndef ITERAM_H
#define ITERAM_H
#macro init_ram slot
    #ifdef 8_BIT
        #pext iteram8.pext slot
    #else
        #pext iteram.pext slot
    #endif
#endmacro
#endif