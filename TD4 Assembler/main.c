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
#include <stdbool.h>

bool verbose = false;

typedef struct lexem{
	struct lexem* previous;
	char name[15];
	struct lexem* next;
}lexem;

typedef struct Node{
	struct Node* previous;
	lexem* line;
	struct Node* next;
	char num;
	int factNum;
}Node;

typedef struct argNode{
	struct argNode* previous;
	char addr;
	struct argNode* next;
}argNode;

typedef struct labelNode{
	struct labelNode* previous;
	char line [25];
	struct Node* addr;
	struct labelNode* next;
}labelNode;

typedef struct pextNode {
	struct pextNode* previous;
	char name[15];
	struct argNode* args;
	char result;
	struct pextNode* next;
	int mountPoint;
}pextNode;

typedef struct macrosNode {
	struct macrosNode* previous;
	char opcode[15];
	struct argNode* args;
	struct macrosNode* next;
}macrosNode;

void appendLabel(labelNode* root, Node* node, lexem* lexem) {
	labelNode* newLabel = malloc(sizeof(labelNode));
	labelNode* labelCatcher = root;
	while (labelCatcher->next != NULL) {
		labelCatcher = labelCatcher->next;
	}
	newLabel->previous = labelCatcher;
	newLabel->next = NULL;
	labelCatcher->next = newLabel;
	strcpy(newLabel->line, lexem->name);
	for (int i = 0; i<15; i++) {
		if (newLabel->line[i] == ':') {
			newLabel->line[i] = 0;
			break;
		}
	}
	if (lexem->next == NULL) {
		//last lexem => point on next string and remove this one
		newLabel->addr = node->next;
		(node->previous)->next = node->next;
		(node->next)->previous = node->previous;
		free(node);
	} else {
		//not last lexem => point on this one and remove this lexem;
		newLabel->addr = node;
		(lexem->next)->previous = lexem->previous;
		(lexem->previous)->next = lexem->next;
		free(lexem);
	}
}

void appendNode(Node* root, char* string) {
	Node* newNode = malloc(sizeof(Node));
	lexem* newLexem = malloc(sizeof(lexem));
	Node* catcher = root;
	lexem* lexemCatcher = NULL;
	while (catcher->next != NULL) {
		catcher = catcher->next;
	}
	for (int i = 0; i<1024; i++) {
		if (string[i] == '\n') {
			string[i] = 0;
			break;
		}
	}
	newNode->previous = catcher;
	newNode->next = NULL;
	newNode->line = newLexem;
	newLexem->next = NULL;
	newLexem->previous = NULL;
	lexemCatcher = newLexem;
	catcher->next = newNode;
	char* lexs = strtok(string, "\t, ");
	while (lexs != NULL) {
		newLexem = malloc(sizeof(lexem));
		newLexem->next = NULL;
		newLexem->previous = lexemCatcher;
		lexemCatcher->next = newLexem;
		lexemCatcher = lexemCatcher->next;
		int symb = 0;
		for (int i = 0; i<strlen(lexs); i++) {
			if (i<15) {
				newLexem->name[i] = lexs[i];
			}
			symb++;
		}
		for (int i = symb; i<15; i++) {
			newLexem->name[i] = 0;
		}
		lexs = strtok(NULL, "\t, ");
	}
}

char findReference(char* label, labelNode* referenceSource) {
	labelNode* catcher = referenceSource->next;
	char address = 0;
	while (catcher != NULL) {
		if (strcmp(label, catcher->line) == 0) {
			address = catcher->addr->num;
			break;
		}
		catcher = catcher->next;
	}
	if (catcher == NULL) {
		printf("Unknown reference %s\n", label);
	}
	return address;
}

void usage() {
	printf("\n\n-=-=TD4 Processor Developer Kit=-=-\n© 2018 JL Computer Inc. All rights reserved\nAuthor: Alexander Nekhaev\n\nUsage:\n\t-h -- print usage\n\t-c [file] -- compile file\n\t-r [file] -- run emulator\n\t-d [file] -- disassembly of binary\n\t-o [file] -- output binary to file\n\t-m [file] -- print hardware-ready representation of program\n\t-v -- verbose\n");
}

char* toLowerCase(char* str) {
	char* strN = malloc(strlen(str));
	for (int i = 0; i<strlen(str); i++) {
		strN[i] = tolower(str[i]);
	}
	return strN;
}

char firstSymbol(char* line) {
	char firstS = '\0';
	int len = (int)strlen(line);
	for (int i = 0; i<len; i++) {
		if ((line[i] != ' ') && (line[i] != '\t')) {
			firstS = line[i];
			break;
		}
	}
	return firstS;
}

void appendPext(pextNode* pexts, char* name, int mountPoint) {
	char location[150];
	getcwd(location, sizeof(location));
	strcat(location, "/");
	strcat(location, name);
	FILE* pext = fopen(location, "rt");
	if (pext == NULL) {
		printf("Error: Processor Extention \"%s\" not found.\n", location);
		exit(-2);
	}
	fseek(pext, 0, SEEK_SET);
	pextNode* catcher = pexts;
	while (catcher->next != NULL) {
		catcher = catcher->next;
	}
	argNode* argCatcher = NULL;
	if (pext != NULL) {
		char line[100];
		while (!feof(pext)) {
			fgets(line, 100, pext);
			for (int i = 0; i<100; i++) {
				if (line[i] == '\n') {
					line[i] = 0;
					break;
				}
			}
			if ((firstSymbol(line) != '#') && (firstSymbol(line) != ';') && (strlen(line)>0)) {
				char str[100];
				strcpy(str, line);
				char* lexs = strtok(str, " :|->");
				pextNode* newPextOp = malloc(sizeof(pextNode));
				newPextOp->args = malloc(sizeof(argNode));
				newPextOp->args->addr = 0;
				newPextOp->args->previous = NULL;
				newPextOp->args->next = NULL;
				newPextOp->next  = NULL;
				newPextOp->previous = catcher;
				newPextOp->mountPoint = mountPoint;
				catcher->next = newPextOp;
				strcpy(newPextOp->name, lexs);
				argCatcher = newPextOp->args;
				while (lexs != NULL) {
					lexs = strtok(NULL, " :|->");
					if (lexs != NULL) {
						if ((firstSymbol(lexs) != '#') && (firstSymbol(lexs) != ';')) {
							argNode* newArg = malloc(sizeof(argNode));
							newArg->addr = strtol(lexs, NULL, 16);
							newArg->next = NULL;
							argCatcher->next = newArg;
							newArg->previous = argCatcher;
							argCatcher = argCatcher->next;
						} else {
							break;
						}
					}
				}
				argCatcher->previous->next = NULL;
				newPextOp->result = argCatcher->addr;
				free(argCatcher);
				catcher = catcher->next;
			}
		}
	}
}

void appendMacros(macrosNode* macroses, char* name) {
	char location[150];
	getcwd(location, sizeof(location));
	strcat(location, "/");
	strcat(location, name);
	FILE* macros = fopen(location, "rt");
	if (macros == NULL) {
		printf("Error: macros \"%s\" not found.\n", location);
		exit(-2);
	}
	fseek(macros, 0, SEEK_SET);
	macrosNode* catcher = macroses;
	while (catcher->next != NULL) {
		catcher = catcher->next;
	}
	argNode* argCatcher = NULL;
	if (macros != NULL) {
		char line[100];
		fgets(line, 100, macros);
		for (int i = 0; i<100; i++) {
			if (line[i] == '\n') {
				line[i] = 0;
				break;
			}
		}
		if ((firstSymbol(line) != '#') && (firstSymbol(line) != ';') && (strlen(line)>0)) {
			char str[100];
			strcpy(str, line);
			bool isDeclaration = false;
		}
	}
}

typedef struct {
	char var[5];
	char addr;
}dictLine;

int insertPextMacros(Node* catcher, pextNode* pexts) {
	char opcode[10];
	strcpy(opcode, catcher->line->next->name);
	pextNode* pextCatcher = pexts;
	while (pextCatcher != NULL) {
		pextCatcher = pextCatcher->next;
		if (pextCatcher != NULL) {
			if (strcmp(pextCatcher->name, opcode) == 0) {
				break;
			}
		}
	}
	if (pextCatcher == NULL) {
		return 1;
	}
	int args = 0;
	lexem* lexCatcher = catcher->line->next;
	dictLine* replacers = malloc(sizeof(dictLine)*args);
	while (lexCatcher->next != NULL) {
		args++;
		lexCatcher = lexCatcher->next;
	}
	argNode* argsCatcher = pextCatcher->args;
	int argsNeeded = 0;
	while (argsCatcher->next != NULL) {
		argsNeeded++;
		argsCatcher = argsCatcher->next;
	}
	if (args < argsNeeded) {
		printf("Not enough arguments in function %s\n", pextCatcher->name);
		exit(-2);
	} else if (args > argsNeeded) {
		printf("Too many arguments in function %s\n", pextCatcher->name);
		exit(-2);
	}
	int argCounter = 0;
	lexCatcher = catcher->line->next;
	while (lexCatcher->next != NULL) {
		lexCatcher = lexCatcher->next;
		strcpy(replacers[argCounter].var, lexCatcher->name);
		argCounter++;
	}
	argsCatcher = pextCatcher->args;
	argCounter = 0;
	while (argsCatcher->next != NULL) {
		argsCatcher = argsCatcher->next;
		char addr = argsCatcher->addr;
		if (argCounter < args) {
			replacers[argCounter].addr = addr;
		}
		argCounter++;
	}
	for (int i = 0; i<args; i++) {
		for (int j = i; j<args; j++) {
			if (strcmp(replacers[j].var,"b") == 0) {
				dictLine cache;
				//save
				cache.addr = replacers[0].addr;
				strcpy(cache.var, replacers[0].var);
				//mov
				replacers[0].addr = replacers[j].addr;
				strcpy(replacers[0].var, replacers[j].var);
				//mov from cache
				replacers[j].addr = cache.addr;
				strcpy(replacers[j].var, cache.var);
				break;
			}
		}
	}
	Node* originalCode = catcher;
	Node* pextConnector = malloc(sizeof(Node));
	pextConnector->previous = NULL;
	pextConnector->next = NULL;
	pextConnector->line = NULL;
	char mountPointAdapter[10];
	sprintf(mountPointAdapter, "swm %d", pextCatcher->mountPoint);
	appendNode(pextConnector, mountPointAdapter);
	pextConnector = pextConnector->next;
	pextConnector->previous = originalCode;
	for (int i = 0; i<args; i++) {
		char operator[1024];
		if (strcmp(replacers[i].var, "a") == 0) {
			strcpy(operator, "mov b a");
			appendNode(pextConnector, operator);
			sprintf(operator, "st %d", replacers[i].addr);
			appendNode(pextConnector, operator);
		} else if (strcmp(replacers[i].var, "b") == 0) {
			sprintf(operator, "st %d", replacers[i].addr);
			appendNode(pextConnector, operator);
		} else {
			sprintf(operator, "mov b %d", (int)replacers[i].var);
			appendNode(pextConnector, operator);
			sprintf(operator, "st %d", replacers[i].addr);
			appendNode(pextConnector, operator);
		}
	}
	char operator[1024];
	for (int i = 0; i<1024; i++) {
		operator[i] = 0;
	}
	sprintf(operator, "ld %d", pextCatcher->result);
	appendNode(pextConnector, operator);
	Node* loaderCatcher = pextConnector;
	while (loaderCatcher->next != NULL) {
		loaderCatcher = loaderCatcher->next;
	}
	loaderCatcher->next = originalCode->next;
	originalCode->next->previous = loaderCatcher;
	originalCode->previous->next = pextConnector;
	return 0;
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
				b = ram[currentMemoryList][im];
				break;
			case 0b11010000:
				ram[currentMemoryList][im] = b;
				break;
			case 0b10100000:
				currentMemoryList = im; //swm
				break;
			case 0b10000000:
				currentROMBank = im; //swi
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

static void printROMMap() {
	for (int l = 0; l<16; l++) {
		printf("ROM Bank #%d\n", l+1);
		for (int j = 0; j<4; j++) {
			for (int k = 0; k<4; k++) {
				for (int i = 0; i < 8; i++) {
					printf("%d", !!((program[l][j*4+k] << i) & 0x80));
				}
				printf(" ");
			}
			printf("\n");
		}
	}
}

static void assembleFile(FILE **fileToCompile) {
	*fileToCompile = fopen(optarg, "rt");
	if (*fileToCompile != NULL) {
		fseek(*fileToCompile, 0, SEEK_SET);
		char line[1024];
		for (int i = 0; i<1024; i++) {
			line[i] = 0;
		}
		fseek(*fileToCompile, 0, SEEK_SET);
		Node* root = malloc(sizeof(Node));
		root->previous = NULL;
		root->next = NULL;
		root->line = malloc(sizeof(lexem));
		root->line->previous = NULL;
		root->line->next = NULL;
		while (!feof(*fileToCompile)) {
			fgets(line, 1024, *fileToCompile);
			appendNode(root, line);
		}
		Node* catcher = root->next;
		lexem* lexCatcher = catcher->line->next;
		bool comment = false;
		while (catcher->next != NULL) {
			while (lexCatcher != NULL) {
				if (firstSymbol(lexCatcher->name) == ';' || firstSymbol(lexCatcher->name) == '#') {
					comment = true;
				}
				if (comment) {
					lexCatcher->previous->next = lexCatcher->next;
					if (lexCatcher->next != NULL) {
						lexCatcher->next->previous = lexCatcher->previous;
					}
					free(lexCatcher);
				}
				lexCatcher = lexCatcher->next;
			}
			comment = false;
			catcher = catcher->next;
			lexCatcher = catcher->line->next;
		}
		catcher = root->next;
		while (catcher->next != NULL) {
			if (catcher->line->next == NULL) {
				catcher->previous->next = catcher->next;
				catcher->next->previous = catcher->previous;
				free(catcher);
			}
			catcher = catcher->next;
		}
		catcher = root->next;
		lexCatcher = catcher->line->next;
		pextNode* pexts = malloc(sizeof(pextNode));
		pexts->previous = NULL;
		pexts->next = NULL;
		pexts->args = malloc(sizeof(argNode));
		while (catcher->next != NULL) {
			while (lexCatcher != NULL) {
				if (firstSymbol(lexCatcher->name) == '%') {
					//catch "pext"
					if (strcmp(lexCatcher->name, "%pext") == 0) {
						lexem* name = lexCatcher->next;
						int mountPoint = atoi(lexCatcher->next->next->name);
						appendPext(pexts, name->name, mountPoint);
						catcher->previous->next = catcher->next;
						catcher->next->previous = catcher->previous;
						lexCatcher = catcher->next->line;
					} else if (strcmp(lexCatcher->name, "%include") == 0) {
						lexem* name = lexCatcher->next;
						
						lexCatcher = catcher->next->line;
					}
				}
				lexCatcher = lexCatcher->next;
			}
			catcher = catcher->next;
		}
		//Let's catch labels
		labelNode* labels = malloc(sizeof(labelNode));
		labels->previous = NULL;
		labels->next = NULL;
		labels->addr = NULL;
		catcher = root->next;
		lexCatcher = catcher->line->next;
		while (catcher->next != NULL) {
			while (lexCatcher != NULL) {
				for (int i = 0; i<15; i++) {
					if (lexCatcher->name[i] == ':') {
						//IT'S LABEL!!!
						appendLabel(labels, catcher, lexCatcher);
					}
				}
				lexCatcher = lexCatcher->next;
			}
			catcher = catcher->next;
			lexCatcher = catcher->line->next;
		}
		catcher = root->next;
		while (catcher->next != NULL) {
			lexCatcher = catcher->line->next;
			while (lexCatcher != NULL) {
				insertPextMacros(catcher, pexts);
				lexCatcher = lexCatcher->next;
			}
			catcher = catcher->next;
		}
		if (verbose) {
			pextNode* pextCatcher = pexts->next;
			printf("Imported pExt specified commands:\n");
			while (pextCatcher != NULL) {
				printf("%s\t(mount point %d)\n", pextCatcher->name, pextCatcher->mountPoint);
				pextCatcher = pextCatcher->next;
			}
			printf("Labels:\n");
			labelNode* labelCatcher = labels->next;
			while (labelCatcher != NULL) {
				printf("%s\n", labelCatcher->line);
				labelCatcher = labelCatcher->next;
			}
		}
		catcher = root->next;
		while (catcher->next != NULL) {
			lexCatcher = catcher->line->next;
			while (lexCatcher != NULL) {
				strcpy(lexCatcher->name, toLowerCase(lexCatcher->name));
				lexCatcher = lexCatcher->next;
			}
			catcher = catcher->next;
		}
		//printf was here
		int assemblyLine = 0;
		int factLine = 0;
		catcher = root->next;
		while (catcher->next != NULL) {
			catcher->num = assemblyLine;
			catcher->factNum = factLine;
			assemblyLine++;
			factLine++;
			if (assemblyLine == 15) {
				assemblyLine = 0;
			}
			catcher = catcher->next;
		}
		//Assembly itself
		catcher = root->next;
		assemblyLine = 0;
		int memPage = 0;
		unsigned char assembly = 0;
		while (catcher->next != NULL) {
			lexCatcher = catcher->line->next;
			while (lexCatcher != NULL) {
				if (strcmp(lexCatcher->name, "add") == 0) {
					lexem* arg1 = lexCatcher->next;
					lexem* arg2 = lexCatcher->next->next;
					if (strcmp(arg1->name, "a") == 0) {
						assembly = 0b00000000;
					} else if (strcmp(arg1->name, "b") == 0) {
						assembly = 0b01010000;
					}
					assembly = assembly | (atoi(arg2->name) & 0b00001111);
					lexCatcher = NULL;
				} else if (strcmp(lexCatcher->name, "mov") == 0) {
					lexem* arg1 = lexCatcher->next;
					lexem* arg2 = lexCatcher->next->next;
					if ((strcmp(arg1->name, "a") == 0) && (strcmp(arg2->name, "b") == 0)) {
						assembly = 0b00010000;
					} else if ((strcmp(arg1->name, "b") == 0) && (strcmp(arg2->name, "a") == 0)) {
						assembly = 0b01000000;
					} else if (strcmp(arg1->name, "a") == 0) {
						assembly = 0b00110000 | (atoi(arg2->name) & 0b00001111);
					} else if (strcmp(arg1->name, "b") == 0) {
						assembly = 0b01110000 | (atoi(arg2->name) & 0b00001111);
					} else {
						printf("Shit happens\n");
						exit(-3);
					}
					lexCatcher = NULL;
				} else if (strcmp(lexCatcher->name, "in") == 0) {
					lexem* arg1 = lexCatcher->next;
					if (strcmp(arg1->name, "a") == 0) {
						assembly = 0b00100000;
					} else if (strcmp(arg1->name, "b") == 0) {
						assembly = 0b01100000;
					}
					lexCatcher = NULL;
				} else if (strcmp(lexCatcher->name, "out") == 0) {
					lexem* arg1 = lexCatcher->next;
					if (strcmp(arg1->name, "b") == 0) {
						assembly = 0b10010000;
					} else {
						assembly = 0b10110000 | (atoi(arg1->name) & 0b00001111);
					}
					lexCatcher = NULL;
				} else if (strcmp(lexCatcher->name, "jmp") == 0) {
					lexem* arg1 = lexCatcher->next;
					assembly = 0b11110000 | (atoi(arg1->name) & 0b00001111);
					lexCatcher = NULL;
				} else if (strcmp(lexCatcher->name, "jnc") == 0) {
					lexem* arg1 = lexCatcher->next;
					assembly = 0b11100000 | (atoi(arg1->name) & 0b00001111);
					lexCatcher = NULL;
				} else if (strcmp(lexCatcher->name, "swi") == 0) {
					lexem* arg1 = lexCatcher->next;
					assembly = 0b10000000 | (atoi(arg1->name) & 0b00001111);
					lexCatcher = NULL;
				} else if (strcmp(lexCatcher->name, "swm") == 0) {
					lexem* arg1 = lexCatcher->next;
					assembly = 0b10100000 | (atoi(arg1->name) & 0b00001111);
					lexCatcher = NULL;
				} else if (strcmp(lexCatcher->name, "ld") == 0) {
					lexem* arg1 = lexCatcher->next;
					assembly = 0b11000000 | (atoi(arg1->name) & 0b00001111);
					lexCatcher = NULL;
				} else if (strcmp(lexCatcher->name, "st") == 0) {
					lexem* arg1 = lexCatcher->next;
					assembly = 0b11010000 | (atoi(arg1->name) & 0b00001111);
					lexCatcher = NULL;
				} else if (strcmp(lexCatcher->name, "jmpl") == 0) {
					lexem* arg1 = lexCatcher->next;
					assembly = 0b11110000 | (findReference(arg1->name, labels) & 0b00001111);
					lexCatcher = NULL;
				} else if (strcmp(lexCatcher->name, "jncl") == 0) {
					lexem* arg1 = lexCatcher->next;
					assembly = 0b11100000 | (findReference(arg1->name, labels) & 0b00001111);
					lexCatcher = NULL;
				} else if (strcmp(lexCatcher->name, "hlt") == 0) {
					assembly = 0b11110000 | (catcher->num & 0b00001111);
					lexCatcher = NULL;
				} else if (strcmp(lexCatcher->name, "nop") == 0) {
					assembly = 0b00000000;
					lexCatcher = NULL;
				}
				if ((lexCatcher != NULL) && (lexCatcher->next != NULL)) {
					lexCatcher = lexCatcher->next;
				}
			}
			if ((assemblyLine == 15) && (catcher->next != NULL) && (strcmp(catcher->line->next->name, "swi") != 0)) {
				program[memPage][assemblyLine] = 0b10000000 | (memPage+1 & 0b00001111);
				memPage++;
				assemblyLine = 0;
			}
			catcher = catcher->next;
			program[memPage][assemblyLine] = assembly;
			assemblyLine++;
		}
		if (verbose) {
			catcher = root->next;
			printf("Assembly-ready code:\n");
			while (catcher->next != NULL) {
				lexCatcher = catcher->line->next;
				while (lexCatcher != NULL) {
					strcpy(lexCatcher->name, toLowerCase(lexCatcher->name));
					printf("%s\t",lexCatcher->name);
					lexCatcher = lexCatcher->next;
				}
				printf("\n");
				catcher = catcher->next;
			}
		}
	}
	fclose(*fileToCompile);
}

static void disassembly(FILE **fileToCompile) {
	*fileToCompile = fopen(optarg, "rb");
	if (*fileToCompile != NULL) {
		fread(program, 256, 1, *fileToCompile);
	}
	printf(";Disassembly of %s:\n", optarg);
	for (int j = 0; j<16; j++) {
		for (int i = 0; i<16; i++) {
			if (program[j][i] != 0b00000000) {
				unsigned char opcode = program[j][i] & 0b11110000;
				unsigned char arg = program[j][i] & 0b00001111;
				switch (opcode) {
					case 0b00000000:
						if (arg != 0) {
							printf("add\ta, %d\t;%d\n", arg, i % 15);
						} else {
							printf("nop\t\t;%d\n", i);
						}
						break;
					case 0b00010000:
						if (arg == 0) {
							printf("mov\ta, b\t;%d\n", i);
						} else {
							printf("mov\ta, b + %d\t;%d\n", arg, i);
						}
						break;
					case 0b00100000:
						if (arg == 0) {
							printf("in\ta\t;%d\n", i);
						} else {
							printf("in\ta + %d\t;%d\n", arg, i);
						}
						break;
					case 0b00110000:
						printf("mov\ta, %d\t;%d\n", arg, i);
						break;
					case 0b01000000:
						if (arg == 0) {
							printf("mov\tb, a\t;%d\n", i);
						} else {
							printf("mov\tb, a + %d\t;%d\n", arg, i);
						}
						break;
					case 0b01010000:
						printf("add\tb, %d\t;%d\n", arg, i);
						break;
					case 0b01100000:
						if (arg == 0) {
							printf("in\tb\t;%d\n", i);
						} else {
							printf("in\tb + %d\t;%d\n", arg, i);
						}
						break;
					case 0b01110000:
						printf("mov\tb, %d\t;%d\n", arg, i);
						break;
					case 0b10010000:
						if (arg == 0) {
							printf("out\tb\t;%d\n", i);
						} else {
							printf("out\tb + %d\t;%d\n", arg, i);
						}
						break;
					case 0b10110000:
						printf("out\t%d\t;%d\n", arg, i);
						break;
					case 0b11100000:
						printf("jnc\t%d\t;%d\n", arg, i);
						break;
					case 0b11000000:
						printf("ld\t%d\t;%d\n", arg, i);
						break;
					case 0b11010000:
						printf("st\t%d\t;%d\n", arg, i);
						break;
					case 0b10100000:
						printf("swm\t%d\t;%d\n", arg, i);
						break;
					case 0b10000000:
						printf("swi\t%d\t;%d\n;******************\n;     Next page\n;******************\n", arg, i);
						break;
					case 0b11110000:
						if (arg == i) {
							printf("hlt\t\t;%d\n", i);
						} else {
							printf("jmp\t%d\t;%d\n", arg, i);
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

static void writeOutFile(FILE **output) {
	*output = fopen(optarg, "wb");
	if (*output != NULL) {
		fwrite(program, sizeof(program[0]), sizeof(program)/sizeof(program[0]), *output);
		fclose(*output);
	}
}

static void machineReady(FILE **fileToCompile) {
	*fileToCompile = fopen(optarg, "rb");
	if (*fileToCompile != NULL) {
		fread(program, 256, 1, *fileToCompile);
		printROMMap();
	}
}

int main(int argc, char * argv[]) {
	int opt;
	FILE* fileToCompile;
	FILE* output;
	while ((opt=getopt(argc, argv, "hc:r:d:o:m:v")) != -1) {
		switch (opt) {
			case 'h':
				usage();
			case 'c':
				assembleFile(&fileToCompile);
				break;
			case 'r':
				fileToCompile = fopen(optarg, "rb");
				if (fileToCompile != NULL) {
					fread(program, 256, 1, fileToCompile);
					emulator();
				}
				break;
			case 'd':
				disassembly(&fileToCompile);
				break;
			case 'o':
				writeOutFile(&output);
				break;
			case 'm':
				machineReady(&fileToCompile);
				break;
			case 'v':
				verbose = true;
				break;
			default:
				usage();
				break;
		}
	}
	return 0;
}
