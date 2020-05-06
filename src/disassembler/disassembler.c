#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <stdbool.h>

unsigned char program[16][16];
unsigned int program8[256][256];

void usage() {
	printf("\n\n-=-=TD4 Processor Developer Kit=-=-\n© 2020 JL Computer Inc. All rights reserved\nAuthor: Alexander Nekhaev\n\n-=-=TD4 Family Disassembler=-=-\n\nUsage:\n\t-h -- print usage\n\t-d [file] -- disassembly of 4-bit binary\n\t-D [file] -- disassembly of 8-bit binary\n");
}

static void disassembly(FILE **fileToCompile) {
	*fileToCompile = fopen(optarg, "rb");
	if (*fileToCompile != NULL) {
		fread(program, 256, 1, *fileToCompile);
	}
	printf(";Disassembly of %s:\n.main:\n", optarg);
	for (int j = 0; j<16; j++) {
		for (int i = 0; i<16; i++) {
			if (program[j][i] != 0b00000000) {
				unsigned char opcode = program[j][i] & 0b11110000;
				unsigned char arg = program[j][i] & 0b00001111;
				switch (opcode) {
					case 0b00000000:
						if (arg != 0) {
							printf("add\ta, %d\t\t;%d\n", arg, i % 15);
						} else {
							printf("nop\t\t\t;%d\n", i);
						}
						break;
					case 0b00010000:
						if (arg == 0) {
							printf("mov\ta, b\t\t;%d\n", i);
						} else {
							printf("mov\ta, b +%d\t;%d\n", arg, i);
						}
						break;
					case 0b00100000:
						if (arg == 0) {
							printf("in\ta\t\t;%d\n", i);
						} else {
							printf("in\ta +%d\t\t;%d\n", arg, i);
						}
						break;
					case 0b00110000:
						printf("mov\ta, %d\t\t;%d\n", arg, i);
						break;
					case 0b01000000:
						if (arg == 0) {
							printf("mov\tb, a\t\t;%d\n", i);
						} else {
							printf("mov\tb, a +%d\t;%d\n", arg, i);
						}
						break;
					case 0b01010000:
						printf("add\tb, %d\t\t;%d\n", arg, i);
						break;
					case 0b01100000:
						if (arg == 0) {
							printf("in\tb\t\t;%d\n", i);
						} else {
							printf("in\tb +%d\t;%d\n", arg, i);
						}
						break;
					case 0b01110000:
						printf("mov\tb, %d\t\t;%d\n", arg, i);
						break;
					case 0b10010000:
						if (arg == 0) {
							printf("out\tb\t\t;%d\n", i);
						} else {
							printf("out\tb +%d\t;%d\n", arg, i);
						}
						break;
					case 0b10110000:
						printf("out\t%d\t\t;%d\n", arg, i);
						break;
					case 0b11100000:
						printf("jnc\t%d\t\t;%d\n", arg, i);
						break;
					case 0b11000000:
						printf("ld\t%d\t\t;%d\n", arg, i);
						break;
					case 0b11010000:
						printf("st\t%d\t\t;%d\n", arg, i);
						break;
					case 0b10100000:
						printf("swm\t%d\t\t;%d\n", arg, i);
						break;
					case 0b10000000:
						printf("swi\t%d\t\t;%d\n;******************\n;     Next page\n;******************\n", arg, i);
						break;
					case 0b11110000:
						if (arg == i) {
							printf("hlt\t\t\t;%d\n", i);
						} else {
							printf("jmp\t%d\t\t;%d\n", arg, i);
						}
						break;
					default:
						printf("nop\n");
						break;
				}
			}
		}
	}
}

static void disassembly8(FILE *fileToCompile) {
	fileToCompile = fopen(optarg, "rb");
	if (fileToCompile != NULL) {
		fseek(fileToCompile, 0, SEEK_END);
		long filelen = ftell(fileToCompile);
		fseek(fileToCompile, 0, SEEK_SET);
		char *buffer = malloc((filelen+1)*sizeof(char));
		fread(buffer, filelen, 1, fileToCompile);
		fclose(fileToCompile);
		int currentByte = 0;
		bool startOfByte = true;
		int i = 0, j = 0;
		while (i*255+j < 255*255) {
			if (startOfByte) {
				char firstByte = buffer[currentByte];
				char secondByte = buffer[currentByte+1];
				int part1 = (firstByte & 0b11110000)<<4;
				part1 = part1 | (firstByte & 0b1111) << 4;
				int part2 = (secondByte & 0b11110000)>>4;
				program8[i][j] = part1 | part2;
				startOfByte = !startOfByte;
				currentByte++;
			} else {
				char firstByte = buffer[currentByte];
				char secondByte = buffer[currentByte+1];
				int part1 = (firstByte & 0b00001111) << 8;
				int part2 = secondByte & 0b11111111;
				program8[i][j] = part1 | part2;
				currentByte = currentByte + 2;
				startOfByte = !startOfByte;
			}
			j++;
			if (j >= 256) {
				i++;
				j = 0;
			}
			if (i >= 256) {
				i = 0;
			}
		}
	}
	printf(";Disassembly of %s:\n.main:\n", optarg);
	for (int j = 0; j<256; j++) {
		for (int i = 0; i<256; i++) {
			if (program8[j][i] != 0b000000000000) {
				unsigned int opcode = program8[j][i] & 0b111100000000;
				unsigned char arg = program8[j][i] & 0b000011111111;
				switch (opcode) {
					case 0b000000000000:
						if (arg != 0) {
							printf("add\ta, %d\t\t;%d\n", arg, i % 15);
						} else {
							printf("nop\t\t\t;%d\n", i);
						}
						break;
					case 0b000100000000:
						if (arg == 0) {
							printf("mov\ta, b\t\t;%d\n", i);
						} else {
							printf("mov\ta, b +%d\t;%d\n", arg, i);
						}
						break;
					case 0b001000000000:
						if (arg == 0) {
							printf("in\ta\t\t;%d\n", i);
						} else {
							printf("in\ta +%d\t\t;%d\n", arg, i);
						}
						break;
					case 0b001100000000:
						printf("mov\ta, %d\t\t;%d\n", arg, i);
						break;
					case 0b010000000000:
						if (arg == 0) {
							printf("mov\tb, a\t\t;%d\n", i);
						} else {
							printf("mov\tb, a +%d\t;%d\n", arg, i);
						}
						break;
					case 0b010100000000:
						printf("add\tb, %d\t\t;%d\n", arg, i);
						break;
					case 0b011000000000:
						if (arg == 0) {
							printf("in\tb\t\t;%d\n", i);
						} else {
							printf("in\tb +%d\t;%d\n", arg, i);
						}
						break;
					case 0b011100000000:
						printf("mov\tb, %d\t\t;%d\n", arg, i);
						break;
					case 0b100100000000:
						if (arg == 0) {
							printf("out\tb\t\t;%d\n", i);
						} else {
							printf("out\tb +%d\t;%d\n", arg, i);
						}
						break;
					case 0b101100000000:
						printf("out\t%d\t\t;%d\n", arg, i);
						break;
					case 0b111000000000:
						printf("jnc\t%d\t\t;%d\n", arg, i);
						break;
					case 0b110000000000:
						printf("ld\t%d\t\t;%d\n", arg, i);
						break;
					case 0b110100000000:
						printf("st\t%d\t\t;%d\n", arg, i);
						break;
					case 0b101000000000:
						printf("swm\t%d\t\t;%d\n", arg, i);
						break;
					case 0b100000000000:
						printf("swi\t%d\t\t;%d\n;******************\n;     Next page\n;******************\n", arg, i);
						break;
					case 0b111100000000:
						if (arg == i) {
							printf("hlt\t\t\t;%d\n", i);
						} else {
							printf("jmp\t%d\t\t;%d\n", arg, i);
						}
						break;
					default:
						printf("nop\n");
						break;
				}
			}
		}
	}
}


int main(int argc, char * argv[]) {
    int opt;
    FILE* source;
    while ((opt = getopt(argc, argv, "hd:D:")))
    {
        switch (opt)
        {
        case 'h':
            usage();
            break;
        case 'd':
            disassembly(&source);
            break;
        case 'D':
            disassembly8(&source);
            break;
        default:
            break;
        }
    }
    return 0;
}