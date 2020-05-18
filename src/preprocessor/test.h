/*
    Test header
    Not ok if nothing is printed
*/

#include <test2.h>
#include <std.h>
#include <malloc.h>
#include <string.h>
#include "calc.h"

#macro HW
    #message "Hello, World!" //Print Hello World message
    #message "If you see this string, it's probably working"
#endmacro

#macro test_Params one, two, three
    HW
    #message "One <=> $one"
    #message "Two <=> $two"
    #message "Three <=> $three"
    GB
#endmacro

#define TD4E8

#ifdef TD4E
    init_allocator
    main
    stop_allocator
#else
    #message "Preprocessor test and OS assembly"
    #define var1 1
    #message "var1 = $var1"
    #sumdef var1 2
    #resdef var1 3
    #message "var1 = $var1"
    #sumdef var1 1
    mov a, var1 /*Ink*/
    test_Params 1, 2, 3
    GB
    test_Params a, b, c
    init_allocator
    hlt
#endif