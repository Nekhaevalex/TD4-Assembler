#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include "preprocessor.h"

int main(int argc, char** argv) {
    if (argc > 1) {
        FILE* input = fopen(argv[1], "rt");
        FILE* output;
        bool file_out = false;
        if (argc > 2) {
            output = fopen(argv[2], "wt");
            file_out = true;
        } else
        {
             output = stdout;
        }
        gl_ouput = output;
        gl_input = input;
        int err_msg = make_program(input, output);
        if (file_out) {
            fclose(output);
        }
        return 0;
    }
    free(definitions);
    free(macroses);
    fprintf(stderr, "Not enough files");
    return -1;
}
