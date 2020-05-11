/*
    Test header
*/

#macro HW
    #message "Hello, World!" //Print Hello World message
#endmacro

#define TD4

#ifdef TD4E8
    #error "Too complex system"
#else
    #message "It's ok"
    #define var1 HEL
    #message "var1"
    #sumdef var1 2
    #resdef var1 3
    #message "var1"
    #sumdef var1 1
    mov a, var1
    HW
#endif