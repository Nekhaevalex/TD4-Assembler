NAME = TD4_asm

.PHONY: compile all clean debug test

compile:
	rm -rf bin
	mkdir bin
	@for a in $$(ls); do \
		if [ -d $$a ]; then \
			echo "processing folder $$a"; \
			$(MAKE) -C $$a; \
		fi; \
	done;
	@echo "Done!"

all: test

test: compile
	./$(NAME) -c test.s -o test

debug:
	gcc -o $(NAME) main.c -g

clean:
	rm -rf bin
	rm -f *.o
	rm -f $(NAME)
	rm -f test
	rm -f *.bin
