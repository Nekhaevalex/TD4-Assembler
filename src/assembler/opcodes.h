#ifndef OPCODES_H
#define OPCODES_H

//Opcodes
#define ADD 1
#define MOV 2
#define IN  3
#define SWI 4
#define OUT 5
#define SWM 6
#define LD  7
#define ST  8
#define JNC 9
#define JMP 10

// Arguments
#define A -1
#define B -2


typedef struct
{
    int op;
    int arg1;
    int arg2;
    int fa;
    int promised;
    char bin_instr;
    char arg;
} opcode;

#endif