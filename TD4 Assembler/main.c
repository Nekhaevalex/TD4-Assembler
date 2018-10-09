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
	char* found = strtok(line, " ,");
	while (found != NULL) {
		memcpy(parsed[i], found, strlen(found));
		i++;
		if (strcmp(parsed[i],"#")==0 || strcmp(parsed[i],";")==0 || strcmp(parsed[i],"//")==0) {
			return 0;
		}
		printf("Parsed: %s\n", found);
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
		printf("token: %d\n", tokenized[i]);
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
					return (0b00100000);
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
						printf("Line: %s\n",line);
						if (gotLen != 0) {
							program[i] = compileLine(line);
							i++;
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
				fileToCompile = fopen(optarg, "rt");
				if (fileToCompile != NULL) {
					fread(program, 16, 1, fileToCompile);
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
				break;
		}
	}
}
