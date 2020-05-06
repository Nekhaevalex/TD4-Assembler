#include "opcodes.h"
#include <stdio.h>
#include <stdlib.h>

int opcodes[] = {
    ADD,
    MOV,
    IN,
    SWI,
    OUT,
    SWM,
    LD,
    ST,
    JNC,
    JMP};

char * opcodes_names[] = {
    "add",
    "mov",
    "in",
    "swi",
    "out",
    "swm",
    "ld",
    "st",
    "jnc",
    "jmp"
};

void parse_error(opcode code)
{
    char* op_name = opcodes_names[code.op - 1];
    char* op_arg1 = NULL;
    if (code.arg1 == A) {
        op_arg1 = "a";
    } else if (code.arg1 == B)
    {
        op_arg1 = "b";
    } else
    {
        sprintf(op_arg1, "%d", code.arg1);
    }
    
    printf("Impossible word combination: %s %s %d +%d\n", op_name, op_arg1, code.arg2, code.fa);
    exit(-4);
}

void debug_bin(opcode code) {
    printf("%d %d %d %d\n", code.op, code.arg1, code.arg2, code.fa);
}