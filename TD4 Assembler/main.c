//
//  main.c
//  TD4 Assembler
//
//  Created by Alexander Nekhaev on 06.10.2018.
//  Copyright © 2018 Alexander Nekhaev. All rights reserved.
//

#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <string.h>
#include <ctype.h>

typedef enum opcodes {
	add,
	mov,
	in,
	out,
	jnc,
	jmp,
	a,
	b,
	na
} opcodes;

void usage() {
	printf("\n\n-=-=TD4 Processor Developer Kit=-=-\n© 2018 JL Computer Inc. All rights reserved\n\nUsage:\n\t-h -- print usage\n\t-c [file] -- compile file\n\t-r [file] -- run emulator\n\t-d [file] -- disassembly of binary\n\t-o [file] -- output to file\n");
}

char* toLowerCase(char* str) {
	char* strN = malloc(strlen(str));
	for (int i = 0; i<strlen(str); i++) {
		strN[i] = tolower(str[i]);
	}
	return strN;
}

char compileLine (char* line) {
	char command = '\0';
	int i = 0;
	char parsed[3][4];
	opcodes tokenized[3];
	char* found = strtok(line, " ,\t");
	while (found != NULL) {
		if (strlen(found)<4) {
			memcpy(parsed[i], found, strlen(found));
		} else {
			memcpy(parsed[i], found, 3);
		}
		i++;
		if (parsed[0][0] == '#' || parsed[0][0] == ';' || ((parsed[0][0] == '/') && (parsed[0][1]) == '/')) {
			return 0;
		}
		found = strtok(NULL, " ,");
	}
	
	for (int i = 0; i<3; i++) {
		for (int j = 0; j<4; j++) {
			if (parsed[i][j] == '\n') {
				parsed[i][j] = 0;
			}
		}
	}
	
	for (int i = 0; i<3; i++) {
		if (strcmp(toLowerCase(parsed[i]), "add") == 0) {
			tokenized[i] = add;
		} else if (strcmp(toLowerCase(parsed[i]), "mov") == 0) {
			tokenized[i] = mov;
		} else if (strcmp(toLowerCase(parsed[i]), "in") == 0) {
			tokenized[i] = in;
		} else if (strcmp(toLowerCase(parsed[i]), "out") == 0) {
			tokenized[i] = out;
		} else if (strcmp(toLowerCase(parsed[i]), "jnc") == 0) {
			tokenized[i] = jnc;
		} else if (strcmp(toLowerCase(parsed[i]), "jmp") == 0) {
			tokenized[i] = jmp;
		} else if (strcmp(toLowerCase(parsed[i]), "a") == 0) {
			tokenized[i] = a;
		} else if (strcmp(toLowerCase(parsed[i]), "b") == 0) {
			tokenized[i] = b;
		} else {
			if (i == 0) {
				tokenized[i] = na;
			} else {
				tokenized[i] = atoi(parsed[i]);
			}
		}
	}
	switch (tokenized[0]) {
		case add:
			switch (tokenized[1]) {
				case a:
					return (0b00000000 | tokenized[2]);
					break;
				case b:
					return (0b01010000 | tokenized[2]);
					break;
			}
			break;
		case mov:
			switch (tokenized[1]) {
				case a:
					switch (tokenized[2]) {
						case b:
							return (0b00010000);
							break;
						default:
							return (0b00110000 | tokenized[2]);
							break;
					}
					break;
				case b:
					switch (tokenized[2]) {
						case a:
							return (0b01000000);
							break;
						default:
							return (0b01110000 | tokenized[2]);
							break;
					}
			}
			break;
		case in:
			switch (tokenized[1]) {
				case a:
					return (0b00100000);
					break;
				case b:
					return (0b01100000);
			}
			break;
		case out:
			switch (tokenized[1]) {
				case b:
					return (0b10010000);
					break;
				default:
					return (0b10110000 | tokenized[2]);
			}
			break;
		case jnc:
			return (0b11100000 | tokenized[2]);
			break;
		case jmp:
			return (0b11110000 | tokenized[2]);
			break;
		default:
			return (0b10000000);
			break;
	}
	return command;
}

char program[16];

int main(int argc, const char ** argv) {
	int opt;
	FILE* fileToCompile;
	FILE* output;
	while ((opt=getopt(argc, argv, "hc:r:d:o:")) != -1) {
		switch (opt) {
			case 'h':
				usage();
			case 'c':
				fileToCompile = fopen(optarg, "rt");
				if (fileToCompile != NULL) {
					fseek(fileToCompile, 0, SEEK_SET);
					int i = 0;
					while (!feof(fileToCompile)) {
						char* line;
						size_t len = 0;
						size_t gotLen = getline(&line, &len, fileToCompile);
						if (gotLen != 0) {
							char t = compileLine(line);
							if (t != 0) {
								program[i] = t;
								i++;
							}
						} else {
							return -2;
						}
					}
					fclose(fileToCompile);
				} else {
					return -1;
				}
				break;
			case 'r':
				
				break;
			case 'd':
				fileToCompile = fopen(optarg, "rb");
				if (fileToCompile != NULL) {
					fread(program, 16, 1, fileToCompile);
				}
				printf("//Disassembly of %s:\n", optarg);
				for (int i = 0; i<16; i++) {
					if (program[i] != 0b00000000) {
						unsigned char opcode = program[i] & 0b11110000;
						unsigned char arg = program[i] & 0b00001111;
						switch (opcode) {
							case 0b00000000:
								printf("add\ta, %d\n", arg);
								break;
							case 0b00010000:
								printf("mov\ta, b\n");
								break;
							case 0b00100000:
								printf("in\t,a\n");
								break;
							case 0b00110000:
								printf("mov\tb, %d\n", arg);
								break;
							case 0b01000000:
								printf("mov\tb, a\n");
								break;
							case 0b01100000:
								printf("add\tb, %d\n", arg);
								break;
							case 0b01110000:
								printf("mov\tb, %d\n", arg);
								break;
							case 0b10010000:
								printf("out\tb\n");
								break;
							case 0b10110000:
								printf("out\t%d\n", arg);
								break;
							case 0b11100000:
								printf("jnc\t%d\n", arg);
								break;
							case 0b11110000:
								printf("jmp\t%d\n", arg);
								break;
							default:
								printf("nop\n");
								break;
						}
					}
				}
				break;
			case 'o':
				output = fopen(optarg, "wb");
				if (output != NULL) {
					fwrite(program, sizeof(program[0]), sizeof(program)/sizeof(program[0]), output);
					fclose(output);
				}
				break;
			default:
				usage();
				break;
		}
	}
}
