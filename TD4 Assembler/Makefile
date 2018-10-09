NAME = TD4_asm

.PHONY: compile all clean debug

compile:
	gcc -o $(NAME) main.c

all: compile

debug:
	gcc -o $(NAME) main.c -g

clean:
	rm -f *.o
	rm -f $(NAME)
