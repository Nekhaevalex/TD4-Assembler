#import std.h


#ifndef MALLOC_H
#define MALLOC_H
//Automated memory manager

#macro init_allocator 
    #define malloc_system_pointer 1
    //variable of pointer
    mov b, 1
    st 0
#endmacro

#macro stop_allocator 
    #undef malloc_system_pointer
#endmacro

#macro malloc pointer_ret, size 
    #define pointer_ret malloc_system_pointer
    #sumdef malloc_system_pointer size
    mov b, malloc_system_pointer
    st 0
#endmacro

#macro db value, ret_ptr 
    malloc ptr, 1
    dbx value, ptr
    #define ret_ptr ptr
#endmacro

#macro free pointer 
    #define temp malloc_system_pointer
    #resdef temp pointer
    #resdef malloc_system_pointer temp
    mov b, malloc_system_pointer
    st 0
    #undef temp
#endmacro

#macro memcpy src, dst, length 
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