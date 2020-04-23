#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <string.h>
#include <ctype.h>
#include <stdbool.h>

void usage() {
	printf("\n\n-=-=TD4 Processor Developer Kit=-=-\n© 2020 JL Computer Inc. All rights reserved\nAuthor: Alexander Nekhaev\n\n-=-=TD4 Family Emulator=-=-\n\nUsage:\n\t-h -- print usage\n\t-r [file] -- run emulator\n\t-R [file] -- run emulator of 8bit machine\n");
}

unsigned char program[16][16];

void emulator() {
	unsigned char a = 0;
	unsigned char b = 0;
	int pc = 0;
	unsigned char im;
	int c = 0;
	int inputCounter = 1;
	int outputCounter = 1;
	int cache;
	
	char ram[16][16];
	
	for (int i = 0; i<16; i++) {
		for (int j = 0; j<16; j++) {
			ram[i][j] = 0;
		}
	}
	
	int currentMemoryList = 0;
	int currentROMBank = 0;
	
	int w = 1;
	
	while (w) {
		unsigned char opcode = program[currentROMBank][pc] & 0b11110000;
		im = program[currentROMBank][pc] & 0b00001111;
		int loc;
		if (a>=16) {
			c = 1;
			a = a & 0b00001111;
		} else {
			c = 0;
		}
		b = b & 0b00001111;
		switch (opcode) {
			case 0b00000000:
				a += im;
				break;
			case 0b00010000:
				a = b+im;
				break;
			case 0b00100000:
				printf("In[%d]:=",inputCounter);
				cache = 17;
				while (cache > 16) {
					scanf("%d", &cache);
				}
				a = (cache & 0b00001111) + im;
				inputCounter++;
				break;
			case 0b00110000:
				a = im;
				break;
			case 0b01000000:
				b = a + im;
				break;
			case 0b01010000:
				b += im;
				break;
			case 0b01100000:
				printf("In[%d]:=",inputCounter);
				cache = 17;
				while (cache > 16) {
					scanf("%d", &cache);
				}
				b = (cache & 0b00001111) + im;
				inputCounter++;
				break;
			case 0b01110000:
				b = im;
				break;
			case 0b10010000:
				printf("Out[%d]:=%d\n", (outputCounter), b+im);
				outputCounter++;
				break;
			case 0b10110000:
				printf("Out[%d]:=%d\n", (outputCounter), im);
				outputCounter++;
				break;
			case 0b11100000:
				if (c != 1) {
					pc = im-1;
				}
				break;
			case 0b11110000:
				if (pc != im) {
					pc = im-1;
				} else {
					printf("HLT command recieved\n");
					w = 0;
				}
				break;
			case 0b11000000:
				loc = im;
				if (loc >15) {
					loc-=16;
				}
				b = ram[currentMemoryList][loc];
				break;
			case 0b11010000:
				loc = im;
				if (loc >15) {
					loc-=16;
				}
				ram[currentMemoryList][loc] = b;
				break;
			case 0b10100000:
				loc = im;
				if (loc >15) {
					loc-=16;
				}
				currentMemoryList = loc; //swm
				break;
			case 0b10000000:
				loc = im;
				if (loc >15) {
					loc-=16;
				}
				currentROMBank = loc; //swi
				break;
			default:
				break;
		}
		pc++;
		if (pc == 16) {
			pc = 0;
		}
	}
}

unsigned int program8[256][256];

void emulator8() {
	unsigned char a = 0;
	unsigned char b = 0;
	int pc = 0;
	unsigned char im;
	int c = 0;
	int cache;
	
	char ram[256][256];
	
	for (int i = 0; i<256; i++) {
		for (int j = 0; j<256; j++) {
			ram[i][j] = 0;
		}
	}
	
	int currentMemoryList = 0;
	int currentROMBank = 0;
	
	int w = 1;
	
	while (w) {
		unsigned int opcode = program8[currentROMBank][pc] & 0b111100000000;
		im = program8[currentROMBank][pc] & 0b000011111111;
		int loc;
		if (a>=16) {
			c = 1;
			a = a & 0b000011111111;
		} else {
			c = 0;
		}
		b = b & 0b000011111111;
		switch (opcode) {
			case 0b00000000:
				a += im;
				break;
			case 0b000100000000:
				a = b+im;
				break;
			case 0b001000000000:
				cache = fgetc(stdin);
				a = (cache & 0b000011111111) + im;
				break;
			case 0b001100000000:
				a = im;
				break;
			case 0b010000000000:
				b = a + im;
				break;
			case 0b010100000000:
				b += im;
				break;
			case 0b011000000000:
				cache = fgetc(stdin);
				b = (cache & 0b000011111111) + im;
				break;
			case 0b011100000000:
				b = im;
				break;
			case 0b100100000000:
				printf("%c", b+im);
				break;
			case 0b101100000000:
				printf("%c", im);
				break;
			case 0b111000000000:
				if (c != 1) {
					pc = im-1;
				}
				break;
			case 0b111100000000:
				if (pc != im) {
					pc = im-1;
				} else {
					printf("HLT command recieved\n");
					w = 0;
				}
				break;
			case 0b110000000000:
				loc = im;
				if (loc >255) {
					loc-=256;
				}
				b = ram[currentMemoryList][loc];
				break;
			case 0b110100000000:
				loc = im;
				if (loc >255) {
					loc-=256;
				}
				ram[currentMemoryList][loc] = b;
				break;
			case 0b101000000000:
				loc = im;
				if (loc >255) {
					loc-=256;
				}
				currentMemoryList = loc; //swm
				break;
			case 0b100000000000:
				loc = im;
				if (loc > 255) {
					loc-=256;
				}
				currentROMBank = loc; //swi
				break;
			default:
				break;
		}
		pc++;
		if (pc == 256) {
			pc = 0;
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
        case 'r':
				source = fopen(optarg, "rb");
				if (source != NULL) {
					fread(program, 256, 1, source);
					emulator();
				}
				fclose(source);
				break;
			case 'R':
				source = fopen(optarg, "rb");
				if (source != NULL) {
					fseek(source, 0, SEEK_END);
					long filelen = ftell(source);
					rewind(source);
					char *buffer = malloc((filelen+1)*sizeof(char));
					fread(buffer, filelen, 1, source);
					fclose(source);
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
					emulator8();
				}
				break;
        default:
            break;
        }
    }
    return 0;
}