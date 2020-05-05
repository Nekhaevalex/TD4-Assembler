#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>
#include <pcre.h>
#include <unistd.h>
#include <ctype.h>
#include "patterns.h"
#include "opcodes.h"
#include "dictionary.h"

#define TD4_MODE 15
#define TD8_MODE 255

#define OVECTORLEN 30

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

void print_help()
{
    printf("OK");
}

void print_inf_error()
{
    printf("Error: Input file missing\n");
}

void parse_error(opcode code) {
    printf("Impossible word combination:");
}

opcode make_opcode(char *op, char *arg1, char *arg2, char *fa)
{
    opcode code;
    for (int i = 0; i < strlen(op); i++)
    {
        op[i] = tolower(op[i]);
    }
    if (strlen(arg1) < 2)
    {
        for (int i = 0; i < strlen(arg1); i++)
        {
            arg1[i] = tolower(arg1[i]);
        }
    }
    for (int i = 0; i < strlen(arg2); i++)
    {
        arg2[i] = tolower(arg2[i]);
    }
    if (strcmp(op, "add") == 0)
    {
        code.op = ADD;
    }
    else if (strcmp(op, "mov") == 0)
    {
        code.op = MOV;
    }
    else if (strcmp(op, "in") == 0)
    {
        code.op = IN;
    }
    else if (strcmp(op, "swi") == 0)
    {
        code.op = SWI;
    }
    else if (strcmp(op, "out") == 0)
    {
        code.op = OUT;
    }
    else if (strcmp(op, "swm") == 0)
    {
        code.op = SWM;
    }
    else if (strcmp(op, "ld") == 0)
    {
        code.op = LD;
    }
    else if (strcmp(op, "st") == 0)
    {
        code.op = ST;
    }
    else if (strcmp(op, "jnc") == 0)
    {
        code.op = JNC;
    }
    else if (strcmp(op, "jmp") == 0)
    {
        code.op = JMP;
    }

    if (strcmp(arg1, "a") == 0)
    {
        code.arg1 = A;
    }
    else if (strcmp(arg1, "b") == 0)
    {
        code.arg1 = B;
    }
    else
    {
        if (code.op == JNC || code.op == JMP)
        {
            int tgt = find_label(arg1);
            if (tgt >= 0)
            {
                code.arg1 = tgt;
            }
            else
            {
                code.arg1 = compute_hash(arg1);
                code.promised = 1;
            }
        }
        else
        {
            code.arg1 = atoi(arg1);
        }
    }
    if (strcmp(arg2, "a") == 0)
    {
        code.arg2 = A;
    }
    else if (strcmp(arg2, "b") == 0)
    {
        code.arg2 = B;
    }
    else
    {
        code.arg2 = atoi(arg2);
    }
    code.fa = atoi(fa);

    op = NULL;
    arg1 = NULL;
    arg2 = NULL;
    fa = NULL;
    return code;
}

int convert(int num)
{
    if (num < 0)
    {
        num = ~(-num) + 1;
    }
    return num;
}

opcode finish_promise(opcode code, int mode)
{
    if (code.promised == 1)
    {
        int promise = find_label_by_hash(code.arg1);
        if (promise > 0)
        {
            code.arg1 = promise;
            code.promised = 0;
        }
        else
        {
            printf("Label not introduced: %d\n", code.arg1);
        }
    }
    int mask;
    if (mode == TD4_MODE)
    {
        mask = 0b1111;
    }
    else if (mode == TD8_MODE)
    {
        mask = 0b11111111;
    }
    code.fa = convert(code.fa) & mask;
}

opcode make_bin_parts(opcode code, int mode) {
    char mask = 0b1111;
    if (mode == TD8_MODE) {
        mask = 0b11111111;
    }
    switch (code.op)
    {
    case ADD:
        switch (code.arg1)
        {
        case A:
            code.bin_instr = 0b0000;
            code.arg = mask & code.arg2;
            break;
        case B:
            code.bin_instr = 0b0101;
            code.arg = mask & code.arg2;
            break;
        default:
            parse_error(code);
            break;
        }
        break;
    case MOV:
        switch (code.arg1)
        {
        case A:
            if (code.arg2 == B) {
                code.bin_instr = 0b0001;
                code.arg = mask & code.fa;
            }
            else
            {
                code.bin_instr = 0b0011;
                code.arg = mask & code.arg2;
            }
            break;
        case B:
            if (code.arg2 == A) {
                code.bin_instr = 0b0100;
                code.arg = mask & code.fa;
            }
            else
            {
                code.bin_instr = 0b0111;
                code.arg = mask & code.arg2;
            }
            break;
        default:
            parse_error(code);
            break;
        }
        break;
    case IN:
        switch (code.arg1)
        {
        case A:
            code.bin_instr = 0b0010;
            code.arg = mask & code.fa;
            break;
        case B:
            code.bin_instr = 0b0110;
            code.arg = mask & code.fa;
            break;
        default:
            parse_error(code);
            break;
        }
        break;
    case SWI:
        code.bin_instr = 0b1000;
        code.arg = mask & code.arg1;
        break;
    case OUT:
        break;
    case SWM:
        break;
    case LD:
        break;
    case ST:
        break;
    case JNC:
        break;
    case JMP:
        break;
    default:
        parse_error(code);
        break;
    }
}

void build(FILE *in, FILE *out, int mode)
{
    const char *label_error;
    int label_error_offset;
    pcre *pat_label = pcre_compile(PAT_LABEl, PCRE_CASELESS, &label_error, &label_error_offset, NULL);

    const char *main_error;
    int main_error_offset;
    pcre *pat_main = pcre_compile(PAT_MAIN, PCRE_CASELESS, &main_error, &main_error_offset, NULL);

    if (pat_label == NULL || pat_main == NULL)
    {
        printf("PCRE compilation failed");
        exit(-100);
    }

    int label_ovector[OVECTORLEN];
    int main_ovector[OVECTORLEN];

    char buffer[100];
    int string_counter = -1;
    int prog_limit = (mode + 1) * (mode + 1);
    opcode *code = malloc(sizeof(opcode) * prog_limit);
    int built_lines = 0;
    while (!feof(in))
    {
        fgets(buffer, 100, in);
        int label_rc = pcre_exec(pat_label, NULL, buffer, strlen(buffer), 0, 0, label_ovector, OVECTORLEN);
        int main_rc = pcre_exec(pat_main, NULL, buffer, strlen(buffer), 0, 0, main_ovector, OVECTORLEN);

        int id = 0;
        char op[5];
        char arg1[5];
        char arg2[5];
        char fa[5];
        char lab[15];

        op[0] = 0;
        arg1[0] = 0;
        arg2[0] = 0;
        fa[0] = 0;
        lab[0] = 0;

        if (label_rc > 0)
        {
            id = 1;
            for (int i = 1; i < label_rc; i++)
            {
                char *substring = buffer + label_ovector[2 * i];
                int substring_len = label_ovector[2 * i + 1] - label_ovector[2 * i];
                switch (i)
                {
                case 1:
                    sprintf(lab, "%.*s", substring_len, substring);
                    sprintf(arg1, "%d", string_counter + 1);
                    break;
                default:
                    break;
                }
                add_label(lab, arg1);
            }
        }
        if (main_rc > 0)
        {
            id = 2;
            string_counter++;
            if (string_counter > mode)
            {
                string_counter = 0;
            }
            for (int i = 1; i < main_rc; i++)
            {
                char *substring = buffer + main_ovector[2 * i];
                int substring_len = main_ovector[2 * i + 1] - main_ovector[2 * i];
                switch (i)
                {
                case 1:
                    sprintf(op, "%.*s", substring_len, substring);
                    break;
                case 2:
                    sprintf(arg1, "%.*s", substring_len, substring);
                    break;
                case 3:
                    sprintf(arg2, "%.*s", substring_len, substring);
                    break;
                case 4:
                    sprintf(fa, "%.*s", substring_len, substring);
                    break;
                default:
                    break;
                }
            }
            code[built_lines] = make_opcode(op, arg1, arg2, fa);
            built_lines++;
            if (built_lines >= (prog_limit - 1))
            {
                printf("Program size limit reached\n");
                exit(-3);
            }
        }
    }
    for (int i = 0; i < built_lines; i++)
    {
        code[i] = finish_promise(code[i], mode);

        printf("%d %d %d %d\n", code[i].op, code[i].arg1, code[i].arg2, code[i].fa);
    }
}

int main(int argc, char **argv)
{
    int opt;
    char *input_file_name = NULL;
    char *output_file_name = "a.out";
    int mode = TD4_MODE;
    bool verbose = false;
    init();
    while (optind < argc)
    {
        if ((opt = getopt(argc, argv, "o:vm:h")) != -1)
        {
            switch (opt)
            {
            case 'o':
                output_file_name = optarg;
                break;
            case 'h':
                print_help();
                break;
            case 'm':
                mode = TD8_MODE;
                break;
            case 'v':
                verbose = true;
                break;
            default:
                break;
            }
        }
        else
        {
            input_file_name = argv[optind];
            optind++;
        }
    }

    if (input_file_name == NULL)
    {
        print_inf_error();
        return -1;
    }

    FILE *in = fopen(input_file_name, "rt");
    if (in == NULL)
    {
        print_inf_error();
        return -1;
    }
    FILE *out = fopen(output_file_name, "wb");
    build(in, out, mode);
    return 0;
}