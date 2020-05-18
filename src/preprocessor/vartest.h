//Tricking var.h:
#define ITERAM_H

#include <var.h>
#include <string.h>

/*
    Testing VAR.H library
*/

/*
in C:

int main() {
    int val = 1;
    int *ptr;
    ptr = &val;
    int pr = *ptr;
    printf ("%d\n", pr); //returns 1
}
*/

#macro main
    var n
    let n 1
#endmacro

main 