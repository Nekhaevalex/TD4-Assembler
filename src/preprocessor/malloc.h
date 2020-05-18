#include <std.h>
#ifndef MALLOC_H
#define MALLOC_H

#macro init_allocator
    #define malloc_system_pointer 1
    mov b, 1
    st 0
#endmacro

init_allocator

#macro stop_allocator
    #undef malloc_system_pointer
#endmacro

/*
    pointer <- n
    if (val(pointer) is in defs) {
        val(pointer) 
    }
*/
#macro malloc pointer, size
    #define pointer malloc_system_pointer
    #sumdef malloc_system_pointer size
    mov b, malloc_system_pointer
    st 0
#endmacro

#macro db val, ret_ptr
    malloc ptr, 1
    dbx val, ptr
    #define ret_ptr ptr
    #undef ptr
#endmacro

#macro free pointer
    #define temp malloc_system_pointer
    #resdef temp pointer
    #resdef malloc_system_pointer temp
    mov b, malloc_system_pointer
    st 0
    #undef temp
#endmacro

#macro memcpy src dst length
    #define start src
    #define end src
    #sumdef end length
    #define toset dst
    #fordef iter start end 1
    ld iter
    st toset
    #sumdef toset 1
    #endfor
    #undef start
    #undef end
    #undef toset
#endmacro

#endif