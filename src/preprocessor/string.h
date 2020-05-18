#include <std.h>
#include <malloc.h>

#macro ds string, length_ret, pointer_ret
    #define pointer_ret malloc_system_pointer
    db_s string pointer_ret length_ret
    #sumdef pointer_ret length_ret
    #sumdef malloc_system_pointer length_ret
#endmacro

#macro prints pointer, length
    out_addr pointer length
#endmacro

#macro print string
    #map string str
    #ifndef config.IO
        #fordef strIterator 0 str.length 1
            out str[strIterator]
            #undef str[strIterator]
        #endfor
    #undef str.length
    #else
        mode_write
        enable_auto_inc
        bus stdout
        #fordef strIterator 0 str.length 1
            out str[strIterator]
            #undef str[strIterator]
        #endfor
        #undef str.length
    #endif
#endmacro

#macro getc ptr_ret
    malloc ptr_ret, 1
scan:
    mov b, 0
    in b
    mov a, b
    #ifdef 8_BIT
    add a, 255
    #else
    add a, 15
    #endif
    jnc scan
    st ptr
    mov a, 0
#endmacro

#macro gets ptr_ret
    #define ptr_ret malloc_system_pointer
#endmacro