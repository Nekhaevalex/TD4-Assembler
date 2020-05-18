/*
    Attempt to make variables system
*/

#ifndef VAR_H
#define VAR_H

#ifndef ITERAM_H
#error "Iteram.pext not initialized"
#endif

#include <malloc.h>


/*
    initialize variable
*/
#macro var id
    db 0, idPtr
    #define id idPtr
#endmacro

/*
    assign value to variable
*/
#macro let id, value
    dbx value, id
#endmacro

/*
    initialize pointer
*/
#macro PTR ptr_name
    #define ptr_name 0
#endmacro


#endif