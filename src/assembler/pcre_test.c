#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <pcre.h>
#include "patterns.h"

#define OVECTORLEN 30

int main(int argc, char** argv) {
    /*
    char *pattern = "\\s*(\\w+)\\s+(\\w+)\\s*,?\\s+(\\w+)\\s*([+-]\\s*\\d+)?";
    const char *error;
    int ovector[OVECTORLEN];
    int erroroffset;
    char *subject = "\tmov a, b +1";
    pcre* re = pcre_compile(pattern, 0, &error, &erroroffset, NULL);
    if(re == NULL) {
        printf("PCRE compilation failed at offset %d: %s\n", erroroffset, error);
        return 1;
    }
    int rc = pcre_exec(re, NULL, subject, strlen(subject), 0, 0, ovector, OVECTORLEN);

    //Errors
    if (rc < 0) {
        switch (rc)
        {
        case PCRE_ERROR_NOMATCH:
            printf("No match\n");
            break;
        
        default:
            printf("Matching error: %d\n", rc);
            break;
        }
        pcre_free(re);
        return 1;
    }
    
    if (rc == 0) {
        printf("OVECT to small\n");
    }

    for (int i = 0; i < rc; i++) {
        char* substring = subject + ovector[2*i];
        int substring_len = ovector[2*i+1]-ovector[2*i];
        printf("%2d: %.*s\n", i, substring_len, substring);
    }
    */
    int ovector[OVECTORLEN];
    const char* error;
    int error_offset;
    pcre* label_re = pcre_compile(PAT_LABEl, 0, &error, &error_offset, NULL);
    pcre* mov_re = pcre_compile(PAT_MOV, PCRE_CASELESS, &error, &error_offset, NULL);
    int rc = pcre_exec(label_re, NULL, argv[1], strlen(argv[1]), 0, 0, ovector, OVECTORLEN);
    if (rc == PCRE_ERROR_NOMATCH) {
        rc = pcre_exec(mov_re, NULL, argv[1], strlen(argv[1]), 0, 0, ovector, OVECTORLEN);
        if (rc == PCRE_ERROR_NOMATCH) {
            printf("Error\n");
        }
        else {
            printf("MOV: ");
        }
   } else {
        printf("LABEL:");
   }
   for (int i = 0; i < rc; i++) {
        char* substring = argv[1] + ovector[2*i];
        int substring_len = ovector[2*i+1]-ovector[2*i];
        printf("%2d: %.*s\n", i, substring_len, substring);
    }
}