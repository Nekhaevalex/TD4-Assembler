NAME = TD4_asm

.PHONY: compile all clean debug test
	gcc -o ./bin/assembler ./assembler/assembler.c
	gcc -o ./bin/disassembler ./disassembler/disassembler.c
	gcc -o ./bin/emulator ./emulator/emulator.c
compile:
	gcc -o $(NAME) main.c
	gcc -o ./bin/disassembler ./disassembler/disassembler.c
	gcc -o ./bin/emulator ./emulator/emulator.c
	gcc -o ./bin/assembler ./assembler/assembler.c
	gcc -o ./bin/disassembler ./disassembler/disassembler.c
	gcc -o ./bin/emulator ./emulator/emulator.c

all: test

test: compile
	./$(NAME) -c test.s -o test	gcc -o ./bin/assembler ./assembler/assembler.c
	gcc -o ./bin/disassembler ./disassembler/disassembler.c
	gcc -o ./bin/emulator ./emulator/emulator.c

debug:
	gcc -o $(NAME) main.c -g

clean:
	rm -f *.o
	rm -f $(NAME)
	rm -f test
	rm -f *.bin
