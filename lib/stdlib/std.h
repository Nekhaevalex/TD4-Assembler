#ifndef STD_H
#define STD_H

//missing NOP opcode
#macro nop 
    add a 0
#endmacro

//missing HLT opcode
#macro hlt 
#ifndef _HLT_
    nop
    #define _HLT_ 0
hlt_label:
#endif
    jmp hlt_label
#endmacro

#macro gqt operand, label 
    #ifdef 8_BIT
    #define addit 255
    #else
    #define addit 15
    #endif
    #resdef addit operand
    add a, addit
    jnc label
    #undef full
    #undef addit
#endmacro

//define string
#macro db_s value, address, len_name 
    //mapping value into str array
    #map value str
    #define store_to address
    #fordef iterator1 0 str.length 1
    mov b, str[iterator1]
    #sumdef store_to 1
    st store_to
    #undef str[iterator1]
    #endfor
    #define len_name str.length
    #undef str.length
    #undef store_to
#endmacro

//define byte
#macro dbx value, address 
    mov b, value
    st address
#endmacro

#macro out_addr address, length 
    #define leng_out length
    #sumdef leng_out address
    #fordef iterator1 address leng_out 1
    ld iterator1
    out b
    #endfor
    #undef leng_out
#endmacro

#macro in_addr address, length 
    #define leng_in length
    #sumdef leng_in address
    #fordef iterator2 address leng_in 1
    mov b, 0
iterator2:
    in b
    mov a, b
    add a, 255
    jnc iterator2
    st iterator2
    #endfor
    #undef leng_in
#endmacro

#endif