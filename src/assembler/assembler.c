#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>
#include <pcre.h>
#include <unistd.h>
#include "patterns.h"
#include "opcodes.h"

#define TD4_MODE 1
#define TD8_MODE 2

#define OVECTORLEN 30

typedef struct
{
    char* label_name;
    int string_num;
}label_rec;

typedef struct
{
    int op;
    char arg1;
    char arg2;
    int fa;
}opcode;

void print_help() {
    printf("OK");
}

void print_inf_error() {
    printf("Error: Input file missing\n");
}

opcode make_opcode(int id, char* op, char* arg1, char* arg2, char* fa, char* lab, int sn) {
    switch (id)
    {
    case 1:
        printf("%d::TYPE:\tLABEL\n\tname:\t%s\nline:\t%d\n", sn, lab, sn);
        break;
    case 2:
        printf("%d::TYPE:\tOPERATION\n\tname:\t%s\n\targ1:\t%s\n\targ2:\t%s\n\tfa:\t%s\n", sn, op, arg1, arg2, fa);
        break;
    case 3:
        printf("%d::TYPE:\tJUMP\n\top:\t%s\n\tlabel:\t%s\n", sn, op, lab);
    default:
        break;
    }
}

void build(FILE* in, FILE* out, int mode) {
    const char *label_error;
    int label_error_offset;
    pcre* pat_label = pcre_compile(PAT_LABEl, PCRE_CASELESS, &label_error, &label_error_offset, NULL);
    
    const char *main_error;
    int main_error_offset;
    pcre* pat_main = pcre_compile(PAT_MAIN, PCRE_CASELESS, &main_error, &main_error_offset, NULL);

    const char *jump_error;
    int jump_error_offset;
    pcre* pat_jump = pcre_compile(PAT_JUMP, PCRE_CASELESS, &jump_error, &jump_error_offset, NULL);
    
    if (pat_label == NULL || pat_main == NULL || pat_jump == NULL) {
        printf("PCRE compilation failed");
    }

    int label_ovector[OVECTORLEN];
    int main_ovector[OVECTORLEN];
    int jump_ovector[OVECTORLEN];

    char buffer[100];
    int string_counter = 0;
    while (!feof(in))
    {
        fgets(buffer, 100, in);
        int label_rc = pcre_exec(pat_label, NULL, buffer, strlen(buffer), 0, 0, label_ovector, OVECTORLEN);
        int main_rc  = pcre_exec(pat_main, NULL, buffer, strlen(buffer), 0, 0, main_ovector, OVECTORLEN);
        int jump_rc  = pcre_exec(pat_jump, NULL, buffer, strlen(buffer), 0, 0, jump_ovector, OVECTORLEN);

        int id = 0;
        char op [5];
        char arg1 [5];
        char arg2 [5];
        char fa [5];
        char lab [15];

        string_counter++;

        if (label_rc == PCRE_ERROR_NOMATCH) {
            if (main_rc == PCRE_ERROR_NOMATCH) {
                if (jump_rc == PCRE_ERROR_NOMATCH) {
                    printf("Unknown opcode: %s\n", buffer);
                }
                else if (jump_rc > 0){
                    id = 3;
                    for (int i = 1; i < jump_rc; i++) {
                        char* substring = buffer + main_ovector[2*i];
                        int substring_len = main_ovector[2*i+1]-main_ovector[2*i];
                        switch (i)
                        {
                        case 1:
                            sprintf(op, "%.*s", substring_len, substring);
                            break;
                        case 2:
                            sprintf(lab, "%.*s", substring_len, substring);
                            break;
                        default:
                            break;
                        }
                    }
                }
            }
            else if (main_rc > 0) {
                id = 2;
                for (int i = 1; i < main_rc; i++) {
                    char* substring = buffer + main_ovector[2*i];
                    int substring_len = main_ovector[2*i+1]-main_ovector[2*i];
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
            }
        }
        else if (label_rc > 0)
        {
            id = 1;
            for (int i = 1; i < label_rc; i++) {
                char* substring = buffer + main_ovector[2*i];
                int substring_len = main_ovector[2*i+1]-main_ovector[2*i];
                printf("%2d: %.*s\n", i, substring_len, substring);
            }
        }
        make_opcode(id, op, arg1, arg2, fa, lab, string_counter);
    }
}

int main(int argc, char ** argv) {
    int opt;
    char* input_file_name = NULL;
    char* output_file_name = "a.out";
    int mode = TD4_MODE;
    bool verbose = false;
    while (optind < argc)
    {
        if ((opt = getopt(argc, argv, "o:vm:h")) != -1) {
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
        else {
            input_file_name = argv[optind];
            optind++;
        }
    }
    
    if (input_file_name == NULL) {
        print_inf_error();
        return -1;
    }

    FILE* in = fopen(input_file_name, "rt");
    if (in == NULL) {
        print_inf_error();
        return -1;
    }
    FILE* out = fopen(output_file_name, "wb");
    build(in, out, mode);
    return 0;
}