NAME = TD4_asm

.PHONY: compile all clean debug test

compile:
	cd src/assembler & $(MAKE)

all: test

test: compile
	./$(NAME) -c test.s -o test

debug:
	gcc -o $(NAME) main.c -g

clean:
	rm -f *.o
	rm -f $(NAME)
	rm -f test
	rm -f *.bin
