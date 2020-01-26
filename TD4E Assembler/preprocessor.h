#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include <string.h>
#include "pext_processor.h"

#ifndef PREPROCESSOR_H
#define PREPROCESSOR_H

#define JUNK_DELIM_SET " \"\t\r\n"
#define NEW_LINE_DELIM_SET "\r\n"
#define NULL_STRING ""

#pragma region System

bool _8_bit = false;
bool _opt = false;
bool _trace = false;
FILE* _config = NULL;

#pragma endregion System

#pragma region Constructions

typedef struct definition
{
    char* def_name;
    char* def_value;
}definition;


typedef struct macros
{
    char* macro_name;
    char** macro_vars;
    int var_amount;
    FILE* macro_file;
    fpos_t* macro_start;
}macros;

typedef enum token{
    _EMPTY,
    _IMPORT,
    _DEFINE,
    _PEXT,
    _ERROR,
    _PRAGMA,
    _LINE,
    _MESSAGE,
    _IFDEF,
    _IFNDEF,
    _ELSE,
    _ENDIF,
    _SUMDEF,
    _RESDEF,
    _UNDEF,
    _MAP,
    _FORDEF,
    _MACRO,
    _ENDMACRO,
    _COMMENTARY,
    _UNSTATED,
    _INSERTING_MACRO
} token;

#pragma endregion Constructions
#pragma region Globals

FILE* gl_input = NULL;
FILE* gl_ouput = NULL;
definition* definitions = NULL;
macros* macroses = NULL;
int def_amount = 0;
int macro_amount = 0;
char preproc_args[100][100];
int preproc_args_amount = 0;


#pragma endregion Globals
#pragma region Flags

bool preproc_command_flag = false;
token preproc_command = _EMPTY;
bool region_end = false;
bool endl = false;
bool in_string = false;
int ignore_section = 0;
bool last_branch = true;

#pragma endregion Flags
#pragma region Preprocessor_commands
const char * const preprocessor_commands[] = {
    "#import",
    "#define",
    "#pext",
    "#error",
    "#pragma",
    "#line",
    "#message",
    "#ifdef",
    "#ifndef",
    "#else",
    "#endif",
    "#sumdef",
    "#resdef",
    "#undef",
    "#map",
    "#fordef",
    "#macro",
    "#endmacro",
    "//"
};

#define IMPORT      preprocessor_commands[0]
#define DEFINE      preprocessor_commands[1]
#define PEXT        preprocessor_commands[2]
#define ERROR       preprocessor_commands[3]
#define PRAGMA      preprocessor_commands[4]
#define LINE        preprocessor_commands[5]
#define MESSAGE     preprocessor_commands[6]
#define IFDEF       preprocessor_commands[7]
#define IFNDEF      preprocessor_commands[8]
#define ELSE        preprocessor_commands[9]
#define ENDIF       preprocessor_commands[10]
#define SUMDEF      preprocessor_commands[11]
#define RESDEF      preprocessor_commands[12]
#define UNDEF       preprocessor_commands[13]
#define MAP         preprocessor_commands[14]
#define FORDEF      preprocessor_commands[15]
#define MACRO       preprocessor_commands[16]
#define ENDMACRO    preprocessor_commands[17]
#define COMMENTARY  preprocessor_commands[18]

#pragma endregion Preprocessor_commands

#pragma region Prototypes
void add_definition(char* name, char* value);
void add_macro(char* name, char** vars, int var_amount, FILE* input);
void add_prepoc_argument(char* name);
int char_is_in_set(char c, char* set);
char* get_definition(char* name);
int get_macro_args_amount(char* name);
void get_macro(char* name, char** vars, int var_amount);
char* get_state_name();
char* identify_command_or_definition(bool full_word);
char* identify_command(char* name);
char* identify_definition(char* name);
bool is_definition(char* name);
bool is_macro(char* name);
bool is_number(char* string);
int make_program(FILE* input, FILE* output);
char* prepoc_impl_ifdef();
char* prepoc_impl_ifndef();
char* prepoc_impl_map();
char* prepoc_impl_message();
char* preproc_call();
char* preproc_impl_define();
char* preproc_impl_else();
char* preproc_impl_end();
char* preproc_impl_error();
char* preproc_impl_line();
char* preproc_impl_pragma();
char* preproc_impl_resdef();
char* preproc_impl_sumdef();
char* preproc_impl_undef();
char* preproc_import();
int reference_preproc_argument_amount(token state);
void remove_all_definitions();
void remove_definition(char* name);
void remove_spaces();
void stack_empty();
char stack_get();
char stack_pop();
int stack_push(char c);
#pragma endregion Prototypes

#pragma region Stack_operations

char stack[1024];
int stack_pointer = 0;

int stack_push(char c) {
    if (stack_pointer >= 0 && stack_pointer < 1024) {
        stack[stack_pointer] = c;
        stack_pointer++;
        stack[stack_pointer] = 0;
        return 0;
    }
    fprintf(stderr, "Stack overflow\n");
    exit(-1);
    return -1;
}

char stack_pop() {
    if (stack_pointer > 0 && stack_pointer <= 1024) {
        char toPop = stack[stack_pointer];
        stack[stack_pointer] = 0;
        stack_pointer--;
        return toPop;
    }
    fprintf(stderr, "Stack underflow\n");
    exit(-1);
    return -1;
}

void stack_empty() {
    int endline = stack_pointer < 1024 ? stack_pointer : 1023;
    for (int i = 0; i<=endline; i++) {
        stack[i] = 0;
    }
    stack_pointer = 0;
}

char stack_get() {
    if (stack_pointer > 0 && stack_pointer <= 1024) {
        return stack[stack_pointer - 1];
    }
    fprintf(stderr, "Stack underflow");
    exit(-1);
    return -1;
}

#pragma endregion Stack_operations
#pragma region Characters_operations
int char_is_in_set(char c, char* set) {
    int res = -1;
    for (int i = 0; i<strlen(set); i++) {
        if (set[i] == c)
            res = i;
    }
}

void remove_spaces() {
    if (char_is_in_set(stack_get(), JUNK_DELIM_SET))
        stack_pop();
}

#pragma endregion Characters_operations 
#pragma region Preproc_implementations

char* preproc_impl_define() {
    add_definition (preproc_args[0], preproc_args[1]);
    return NULL_STRING;
}

char* preproc_impl_error() {
    return strcat("#error ", preproc_args[0]);
}

char* prepoc_impl_message() {
    return strcat("#message ", preproc_args[0]);
}

char* preproc_impl_pragma() {
    char* code = preproc_args[0];
    FILE* cconfig = fopen("c_config.config", "a");
    if (strcmp(code, "8_BIT") == 0) {
        add_definition("8_BIT", "TRUE");
        fprintf(cconfig, "8_BIT\n");
    } else if(strcmp(code, "NO_OPT") == 0)
    {
        add_definition("NO_OPT", "TRUE");
        fprintf(cconfig, "NO_OPT\n");
    } else if (strcmp(code, "4_BIT") == 0)
    {
        add_definition("4_BIT", "TRUE");
        fprintf(cconfig, "4_BIT\n");
    } else if (strcmp(code, "USE_TRACE") == 0)
    {
        add_definition("USE_TRACE", "TRUE");
        fprintf(cconfig, "USE_TRACE\n");
    } else if (strcmp(code, "NO_TRACE") == 0) {
        add_definition("NO_TRACE", "TRUE");
        fprintf(cconfig, "NO_TRACE\n");
    } else if (strcmp(code, "CONFIG") == 0) {
        if (_config != NULL) {
            while (!feof(_config))
            {
                //TODO: import config realisation from config_processor
            }
            
        }
    } else if (strcmp(code, "DEF_SYS") == 0)
    {
         if (_8_bit) {
            add_definition("8_BIT", "TRUE");
            fprintf(cconfig, "8_BIT\n");
         } else
         {
            add_definition("4_BIT", "TRUE");
            fprintf(cconfig, "4_BIT\n");
         }
         if (_opt) {
            add_definition("OPTIMIZE", "TRUE");
            fprintf(cconfig, "OPTIMIZE\n");
         } else {
            add_definition("NO_OPT", "TRUE");
            fprintf(cconfig, "NO_OPT\n");
         }
         if (_trace) {
            add_definition("USE_TRACE", "TRUE");
            fprintf(cconfig, "USE_TRACE\n");
         } else
         {
            add_definition("NO_TRACE", "TRUE");
            fprintf(cconfig, "NO_TRACE\n");
         }
         
    }
    
    
    return NULL_STRING;
}

char* preproc_import() {
    FILE* import = fopen(preproc_args[0], "rt");
    if (import == NULL) {
        fprintf(stderr, "Error during import: %s\n", preproc_args[0]);
        exit(-1);
    }
    make_program(import, gl_ouput);
    fclose(import);
    return NULL_STRING;
}

char* preproc_impl_end() {
    region_end = false;
    if (ignore_section > 0) 
        ignore_section--;
    else
    {
        fprintf(stderr, "No subsection entrance\n");
        exit(-1);
    }
    
    return NULL_STRING;
}

char* preproc_impl_line() {
    int line = atoi(preproc_args[0]);
    FILE* line_source = fopen(preproc_args[1], "rt");
    if (line_source == NULL) {
        fprintf(stderr, "Error during file opening: %s\n", preproc_args[1]);
        exit(-1);
    }
    char string[250];
    int line_counter = 0;
    while (!feof(line_source) && line_counter < line)
    {
        if (fgetc(line_source) == "\n")
            line_counter++;
    }
    if (line_counter < line-1) {
        fprintf(stderr, "Line %d does'n exists in %s\n", line, preproc_args[1]);
        exit(-1);
    }
    line_counter = 0;
    char c = 0;
    while (!feof(line_source) && c != "\n")
    {
        c = fgetc(line_source);
        string[line_counter] = c;
        line_counter++;
    }
    string[line_counter] = 0;
    return string;
}

bool is_number(char* string) {
    char ina[3];
    sprintf(ina, "%d", atoi(string));
    if (strcmp(ina, string) == 0) {
        return true;
    }
    return false;
}

char* preproc_impl_sumdef() {
    char* original = preproc_args[0];
    char* add = preproc_args[1];
    char res[3];
    if (is_number(add) && is_number(original)) {
        sprintf(res, "%d", atoi(original) + atoi(add));
        add_definition(original, res);
    } else
    {
        add_definition(original, strcat(original, res));
    }
    return NULL_STRING;
}

char* preproc_impl_resdef() {
    char* original = preproc_args[0];
    char* add = preproc_args[1];
    char res[3];
    if (is_number(add) && is_number(original)) {
        sprintf(res, "%d", atoi(original) - atoi(add));
        add_definition(original, res);
    } else
    {
        add_definition(original, strcat(original, res));
    }
    return NULL_STRING;
}

char* preproc_impl_undef() {
    remove_definition(preproc_args[0]);
    return NULL_STRING;
}

char* prepoc_impl_ifdef() {
    if (is_definition(preproc_args[0])) {
        last_branch = true;
        make_program(gl_input, gl_ouput);
    } else
    {
        last_branch = false;
        make_program(gl_input, gl_ouput);
        ignore_section++;
    }
    return NULL_STRING;
}

char* prepoc_impl_ifndef() {
    if (!is_definition(preproc_args[0])) {
        last_branch = true;
        make_program(gl_input, gl_ouput);
    } else
    {
        last_branch = false;
        make_program(gl_input, gl_ouput);
        ignore_section++;
    }
    return NULL_STRING;
}

char* preproc_impl_else() {
    if (ignore_section >= 0) {
        if (last_branch == true) {
            last_branch = false;
            ignore_section++;
        } else
        {
            last_branch = true;
            ignore_section--;
        }
        make_program(gl_input, gl_ouput);
    } else {
        fprintf(stderr, "No condition test for else branch\n");
        exit(-1);
    }
}

char* prepoc_impl_map() {
    char* var_name = preproc_args[0];
    char* to_delim = preproc_args[1];
    char* str_amount = preproc_args[2];
    int amount = atoi(str_amount);
    char* delim_ready;
    int to_full = strlen(to_delim) % amount;
    if (to_full == 0) {
        delim_ready = to_delim;
    } else
    {
        delim_ready = malloc(sizeof(char) * (to_full + 1));
        for (int i = 0; i < to_full; i++)
        {
            delim_ready[i] = '0';
        }
        delim_ready = strcat(delim_ready, to_delim);
    }
    int step = strlen(delim_ready) % amount;
    for (int i = 0; i<strlen(delim_ready); i += step) {
        //TODO: Finish map
    }
}

char* preproc_call() {
    preproc_args_amount = 0;
}

#pragma endregion Preproc_implementations
#pragma region Macro_and_definitions

bool is_definition(char* name) {
    for (int i = 0; i < def_amount; i++) {
        if (strcmp(definitions[i].def_name, name) == 0) {
            return true;
        }
    }
    return false;
}

void add_definition(char* name, char* value) {
    if (definitions == NULL) {
        definitions = malloc(1 * sizeof(definition));
    } else
    {
        if (!is_definition(name))
            realloc(definitions, def_amount+1);
    }
    if (!is_definition(name)) {
        definitions[def_amount].def_name = malloc(strlen(name)*sizeof(char));
        strcmp(definitions[def_amount].def_name, name);
        definitions[def_amount].def_value = malloc(strlen(value)*sizeof(char));
        strcmp(definitions[def_amount].def_value, value);
        def_amount++;
    } else {
        for (int i = 0; i < def_amount; i++) {
            if (strcmp(definitions[i].def_name, name) == 0) {
                free(definitions[i].def_value);
                definitions[def_amount].def_value = malloc(strlen(value)*sizeof(char));
                strcmp(definitions[def_amount].def_value, value);
            }
        }
    }
}

char* get_definition(char* name) {
    for (int i = 0; i < def_amount; i++) {
        if (strcmp(definitions[i].def_name, name) == 0) {
            if (is_number(definitions[i].def_value)) {
                int num = atoi(definitions[i].def_value);
                if (num < 0) {
                    num = ~(-num) + 1;
                }
                sprintf(definitions[i].def_value, "%d", num);
            }
            return definitions[i].def_value;
        }
    }
    fprintf(stderr, "Unknown value: %s\n", name);
    return NULL;
}

void remove_definition(char* name) {
    for (int i =  0; i < def_amount; i++) {
        if (strcmp(definitions[i].def_name, name) == 0) {
            free(definitions[i].def_name);
            free(definitions[i].def_value);
            for (int j = i; j < def_amount; j++) {
                definitions[j - 1] = definitions[j];
            }
            def_amount--;
            break;
        }
    }
}

void remove_all_definitions() {
    for (int i = 0; i < def_amount; i++) {
        free(definitions[i].def_name);
        free(definitions[i].def_value);
    }
    free(definitions);
}

void add_macro(char* name, char** vars, int var_amount, FILE* input) {
    if (macroses == NULL) {
        macroses = malloc(1 * sizeof(macroses));
    } else {
        realloc(macroses, macro_amount + 1);
    }
    macroses[macro_amount].macro_name = name;
    macroses[macro_amount].macro_vars = vars;
    macroses[macro_amount].macro_file = input;
    macroses[macro_amount].var_amount = var_amount;
    fgetpos(input, macroses[macro_amount].macro_start);
}

void get_macro(char* name, char** vars, int var_amount) {
    int macro = 0;
    bool found = false;
    for(int i = 0; i < macro_amount; i++) {
        if (strcmp(macroses[i].macro_name, name) == 0) {
            macro = i;
            found = true;
            break;
        }
    }
    if (found) {
        if (var_amount != macroses[macro].var_amount) {
            fprintf(stderr, "Parameter mismatch in macro $s", name);
            exit(-1);
        }
        for(int j = 0; j < macroses[macro].var_amount; j++) {
            add_definition(macroses[macro].macro_vars, vars[j]);
        }
        FILE* macro_file = macroses[macro].macro_file;
        fsetpos(macro_file, macroses[macro].macro_start);
        make_program(macro_file, gl_ouput);
        for(int j = 0; j < macroses[macro].var_amount; j++) {
            remove_definition(macroses[macro].macro_vars);
        }
    }
}

int get_macro_args_amount(char* name) {
    for (int i = 0; i < def_amount; i++) {
        if (strcmp(macroses[i].macro_name, name) == 0) {
            return macroses[i].var_amount;
        }
    }
}

bool is_macro(char* name) {
    for (int i = 0; i < def_amount; i++) {
        if (strcmp(macroses[i].macro_name, name) == 0) {
            return true;
        }
    }
    return false;
}

#pragma endregion Macro_and_definitions
#pragma region Identification

char* identify_command(char* name) {
    char* result = NULL;
    if (strncmp(name, IMPORT, strlen(IMPORT)) == 0)
    {
        preproc_command_flag = true;
        preproc_command = _IMPORT;
    } else if (strncmp(name, DEFINE, strlen(DEFINE)) == 0)
    {
        preproc_command_flag = true;
        preproc_command = _DEFINE;
    } else if (strncmp(name, PEXT, strlen(PEXT)) == 0)
    {
        preproc_command_flag = true;
        preproc_command = _PEXT;
    } else if (strncmp(name, ERROR, strlen(ERROR)) == 0)
    {
        preproc_command_flag = true;
        preproc_command = _ERROR;
    } else if (strncmp(name, PRAGMA, strlen(PRAGMA)) == 0)
    {
        preproc_command_flag = true;
        preproc_command = _PRAGMA;
    } else if (strncmp(name, LINE, strlen(LINE)) == 0)
    {
        preproc_command_flag = true;
        preproc_command = _LINE;
    } else if (strncmp(name, MESSAGE, strlen(MESSAGE)) == 0)
    {
        preproc_command_flag = true;
        preproc_command = _MESSAGE;
    } else if (strncmp(name, IFDEF, strlen(IFDEF)) == 0)
    {
        preproc_command_flag = true;
        preproc_command = _IFDEF;
    } else if (strncmp(name, IFNDEF, strlen(IFNDEF)) == 0)
    {
        preproc_command_flag = true;
        preproc_command = _IFNDEF;
    } else if (strncmp(name, ELSE, strlen(ELSE)) == 0)
    {
        preproc_command_flag = true;
        preproc_command = _ELSE;
    } else if (strncmp(name, ENDIF, strlen(ENDIF)) == 0)
    {
        preproc_command_flag = true;
        preproc_command = _ENDIF;
    } else if (strncmp(name, SUMDEF, strlen(SUMDEF)) == 0)
    {
        preproc_command_flag = true;
        preproc_command = _SUMDEF;
    } else if (strncmp(name, RESDEF, strlen(RESDEF)) == 0)
    {
        preproc_command_flag = true;
        preproc_command = _RESDEF;
    } else if (strncmp(name, UNDEF, strlen(UNDEF)) == 0)
    {
        preproc_command_flag = true;
        preproc_command = _UNDEF;
    } else if (strncmp(name, MAP, strlen(MAP)) == 0)
    {
        preproc_command_flag = true;
        preproc_command = _MAP;
    } else if (strncmp(name, FORDEF, strlen(FORDEF)) == 0)
    {
        preproc_command_flag = true;
        preproc_command = _FORDEF;
    } else if (strncmp(name, MACRO, strlen(MACRO)) == 0)
    {
        preproc_command_flag = true;
        preproc_command = _MACRO;
    } else if (strncmp(name, ENDMACRO, strlen(COMMENTARY)) == 0)
    {
        preproc_command_flag = true;
        preproc_command = _ENDMACRO;
    } else if (strncmp(name, COMMENTARY, strlen(COMMENTARY)) == 0)
    {
        preproc_command_flag = true;
        preproc_command = _COMMENTARY;
    } else
    {
        fprintf(stderr, "Unknown token: %s\n", name);
        exit(-1);
    }
    return result;
}

void add_prepoc_argument(char* name) {
    strcpy(preproc_args[preproc_args_amount], name);
    preproc_args_amount++;
}

int reference_preproc_argument_amount(token state) {
    switch (state)
    {
    case _DEFINE:
        return 2;
        break;
    case _ELSE:
        return 0;
        break;
    case _ENDIF:
        return 0;
        break;
    case _ENDMACRO:
        return 0;
        break;
    case _ERROR:
        return 1;
        break;
    case _FORDEF:
        return 3;
        break;
    case _IFDEF:
        return 1;
        break;
    case _IMPORT:
        return 1;
        break;
    case _LINE:
        return 2;
        break;
    case _MACRO:
        return _UNSTATED;
        break;
    case _MAP:
        return 3;
        break;
    case _MESSAGE:
        return 1;
        break;
    case _PEXT:
        return 2;
        break;
    case _PRAGMA:
        return 1;
        break;
    case _RESDEF:
        return 2;
        break;
    case _SUMDEF:
        return 2;
        break;
    case _UNDEF:
        return 1;
        break;
    default:
        fprintf(stderr, "Mode error\n");
        return -1;
        break;
    }
}

char* get_state_name() {
    switch (preproc_command)
    {
    case _COMMENTARY:
        return COMMENTARY;
    case _DEFINE:
        return DEFINE;
    case _ELSE:
        return ELSE;
    case _ENDIF:
        return ENDIF;
    case _ENDMACRO:
        return ENDMACRO;
    case _ERROR:
        return ERROR;
    case _FORDEF:
        return FORDEF;
    case _IFDEF:
        return IFDEF;
    case _IFNDEF:
        return IFNDEF;
    case _IMPORT:
        return IMPORT;
    case _LINE:
        return LINE;
    case _MACRO:
        return MACRO;
    case _MAP:
        return MAP;
    case _MESSAGE:
        return MESSAGE;
    case _PEXT:
        return PEXT;
    case _PRAGMA:
        return PRAGMA;
    case _RESDEF:
        return RESDEF;
    case _SUMDEF:
        return SUMDEF;
    case _UNDEF:
        return UNDEF;
    default:
        return NULL_STRING;
        break;
    }
}

char* identify_definition(char* name) {
    if (preproc_command_flag) {
        if (is_definition(name)) {
            add_prepoc_argument(get_definition(name));
        } else {
            add_prepoc_argument(name);
        }
        if (!_INSERTING_MACRO && endl) {
            preproc_command_flag = false;
            if (reference_preproc_argument_amount(preproc_command) == preproc_args_amount) {
                return preproc_call();
            } else
            {
                fprintf(stderr, "Parameters mismatch in: %s\n", get_state_name());
            }
        }
        return NULL_STRING;
    } else
    {
        if (is_definition(name)) {
            return get_definition(name);
        }
        if (is_macro(name)) {
            preproc_command_flag = true;
            if (endl) {
                preproc_command_flag = false;
                get_macro(name, preproc_args, preproc_args_amount);
            }
            return NULL_STRING;
        }
        //TODO: Add pext insertion
        return NULL;
    }
}

char* identify_command_or_definition(bool full_word) {
    char* result;
    if (stack[0] == '#') {
        result = identify_command(stack);
    } else {
        result = identify_definition(stack);
    }
    if (full_word == true) {
        if (result == NULL) {
            result = stack;
        } else
        {
            stack_empty();
        }
        return result;
    }
    return NULL_STRING;
}

#pragma endregion Identification

int make_program(FILE* input, FILE* output) {
    while (!feof(input) && !region_end)
    {
        char c = fgetc(input);
        int push_result = stack_push(c);
        if (push_result != 0)
            break;
        endl = (c == "\n") ? true : false;
        if (c == "\"") {
            in_string = !in_string;
        }
        remove_spaces();
        if (stack_pointer > 0) {
            bool ctrl_chr = (char_is_in_set(c, JUNK_DELIM_SET) != -1) ? true : false;
            bool finished_word = ctrl_chr & (!in_string);
            char* to_out = identify_command_or_definition(finished_word);
            if (ignore_section == 0) {
                for (int i = 0; i<strlen(to_out); i++) {
                    fputc(to_out[i], output);
                }
                if (ctrl_chr){
                    fputc(c, output);
                }
            }
        }
    }
    region_end = false;
}

#endif