//Global patterns types
//Comment pattern
#define PAT_COM         "\\s*([;/#])+.+"
//Label pattern
#define PAT_LABEl       "\\s*(\\w+)\\s*:&"
#define PAT_NF_LABEL    "\\s*(\\w+)\\s*:"
//Main operations patterns
#define PAT_MAIN        "^\\s*(\\w+)\\s+(\\w+)\\s*,?\\s*(\\w+)?\\s*([+-]\\s*\\d+)?"
#define PAT_ADD         "\\s*add\\s+(\\w)\\s*,?\\s+(\\w+)"
#define PAT_MOV         "\\s*mov\\s+(\\w)\\s*,?\\s+(\\w+)\\s*([+-]\\s*\\d+)?"
#define PAT_IN          "\\s*in\\s+(\\w)\\s*([+-]\\s*\\d+)?"
#define PAT_SWI         "\\s*swi\\s+(\\d+)"
#define PAT_OUT         "\\s*out\\s+(\\w|\\d+)"
#define PAT_SWM         "\\s*swm\\s+(\\d+)"
#define PAT_LD          "\\s*ld\\s+(\\d+)"
#define PAT_ST          "\\s*st\\s+(\\d+)"
//Jump operations
#define PAT_JUMP        "^\\s*(\\w+)\\s+(\\w+)"
#define PAT_JNC         "\\s*jnc\\s+(\\w+)"
#define PAT_JMP         "\\s*jmp\\s+(\\w+)"