# TD4 Assembler v3.0
Third version of TD4 Assembler.
## Index
- [Index](#index)
- [Description](#description)
- [Installation](#installation)
    - [Windows](#windows)
    - [Linux/macOS](#linux/macos)
- [Usage](#usage)
    - [Instructions](#instructions)
    - [Preprocessor commands and special operators](#preprocessor-commands-and-special-operators)
    - [Pragma directives](#pragma-directives)
    - [Making new project](#making-new-project)
        - [Writing macroses](#writing-macroses)
        - [Writing .S file](#writing-.s-file)
    - [Using pExts](#using-pexts)
## Description
This repository hosts the 3rd version of TD4 Assembler. Here are some features:

- TD4/TD4E/TD4E8 support.
- Macroses
- pExts (processor extensions for TD4E/TD4E8)
- Link tracer
- Optimizer
- Sort of OS prototype
## Installation
### Windows
```cmd
git clone https://github.com/Nekhaevalex/TD4-Assembler-v3.0
cd TD4-Assembler-v3.0
.\Assebler\Setup\Release\setup.exe
```
### Linux/macOS
Program can be executed using ```dotnet```.
## Usage
### Instructions
| Binary | Instruction | Argument 1 | Argument 2 | Description             | FastAdd support | Commentary                         |
|--------|-------------|------------|------------|-------------------------|-----------------|------------------------------------|
| 0000   | add         | A          | Im         | A=A+Im                  | False           |                                    |
| 0001   | mov         | A          | B          | A=B + FastAdd           | True            |                                    |
| 0010   | in          | A          | -          | A=In + FastAdd          | True            |                                    |
| 0011   | mov         | A          | Im         | A=Im                    | False           |                                    |
| 0100   | mov         | B          | A          | B=A + FastAdd           | True            |                                    |
| 0101   | add         | B          | Im         | B=B + Im                | False           |                                    |
| 0110   | in          | B          | -          | B=In + FastAdd          | True            |                                    |
| 0111   | mov         | B          | Im         | B=Im                    | False           |                                    |
| 1000   | swi         | Im         | -          | Switch memory bank      | False           | Available only on TD4E/TD4E8       |
| 1001   | out         | B          | -          | Out=B + FastAdd         | True            |                                    |
| 1010   | swm         | Im         | -          | Switch RAM page         | False           | Available only on TD4E/TD4E8       |
| 1011   | out         | Im         | -          | Out=Im                  | False           |                                    |
| 1100   | ld          | Im         | -          | Load from RAM[Im] to B  | False           | Available only on TD4E/TD4E8       |
| 1101   | st          | Im         | -          | Store from B to RAM[Im] | False           | Available only on TD4E/TD4E8       |
| 1110   | jnc         | Im         | -          | PC=Im if C!=1           | False           | Supposed using label instead of Im |
| 1111   | jmp         | Im         | -          | PC=Im                   | False           | Supposed using label instead of Im |

### Preprocessor commands and special operators
| Command                 | Meaning                                                                                                 |
|-------------------------|---------------------------------------------------------------------------------------------------------|
| #import file.h          | Import file.h file with macroses                                                                        |
| #define a b             | Define constant which will be replaced by it's meaning during assembly. Here: a = b                     |
| #pext io.pext address   | Include pExt interface file. Here: include implementation for I/O pExt                                  |
| #error message          | Send error with message and stop assembly                                                               |
| #pragma                 | Change assembler settings. All pragma directives will be mentioned later/                               |
| #line n file.s          | Replace with n-th line from file.s                                                                      |
| #message greet          | Prints "greet" during assembly                                                                          |
| #ifdef A...#endif       | If A is defined, assembles lines inside "..."                                                           |
| #ifndef A...#endif      | If A is not defined, assembles lines inside "..."                                                       |
| #else...#endif          | The other branch of #ifdef/#ifndef construction                                                         |
| #sumdef A B             | Redefines A = A + B                                                                                     |
| #resdef A B             | Redefines A = A - B                                                                                     |
| #undef A                | Undefines A                                                                                             |
| #map string vals        | Transforms string into array with name vals. Accessed as vals[1]...vals[n].                             |
| #map bignumber vals n   | Transforms bignumber into array with n equal length elements and name vals.                             |
| #fordef a b c … #endfor | Repeats operation inside … as long as a < b where c is iterator (a = a + c). a can be used inside array |
| #macro name vals... {}  | Define macros "name" with arguments vals. 
### Pragma directives
| Directive | Meaning                                                                                                  |
|-----------|----------------------------------------------------------------------------------------------------------|
| 8_BIT     | Switch assembler into TD4E8 mode. Also executes #define 8_BIT TRUE                                       |
| NO_OPT    | Disable optimization. Also executes #define NO_OPT TRUE                                                  |
| 4_BIT     | Switch assembler into TD4E/TD4 mode. Also executes #define 4_BIT TRUE                                    |
| USE_TRACE | Enables tracer. Also executes #define USE_TRACE TRUE                                                     |
| NO_TRACE  | Disables tracer. Also executes #define NO_TRACE TRUE                                                     |
| CONFIG    | Loads target machine configuration file. Defines all specs in 'config' namespace.                        |
| UNDEF_ALL | Undefines all definitions.                                                                               |
| DEF_SYS   | Make #define for all known assembly specs: 8_BIT/4_BIT, BINARY/TEXT, OPTIMIZE/NO_OPT, USE_TRACE/NO_TRACE |
### Making new project
We strongly recommend you create project in it's own folder with two files: ```main.s``` and ```funcs.h```. 
#### Writing macroses
All user-defines macroses (which are equvivalents of fucntions) should be defined in ```funcs.h``` file. Simple example:
```c
#macro simple var1 var2
    out var1
    jmp var2
#endmacro
```
Don't forget that var1 can be both defined values and registers names, but not all operations on registers are available. Example:
```c
#macro error_macro var1
    add a var1
#endmacro

#macro caller
    error_macro b
#endmacro
```
Operation ```add``` can't sumarize two registers and program will not assemble.

We strongly recommend using ```main``` macros which will contain your entire program. It will simplify writing  ```main.s``` file which will be only calling ```main``` macros.

#### Writing .S file
Your .s file in project must contain all your imports as it's the only place, where ```#import``` command works. Typical ```main.s``` file example:
```c
#import std.h
#import malloc.h
#import string.h
#import calc.h
#import pexts.h
#import io.h

init_allocator
main
stop_allocator
```
### Using pExts
pExts (or "processor extentions") are main feature of TD4E family. 'E' stands for "extendable". Pexts are hardware co-processors which can be inserted instead of RAM pages. Each pext has it's own ID number and .pext interface file. During assembly all pseudo-instructions for selected pext will be translated into native instructions.
#### Writing .pext file
Any pext file consists of pext instruction descriptions:
```c
opcode : 1 | 2 ... | n-1 > n
```
- opcode - is the name of instruction
- 1 | 2 ... | n-1 - are the arguments addresses
- n - is the resulting argument address
