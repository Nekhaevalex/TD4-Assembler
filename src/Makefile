compile:
	@echo "Compiling src"
	@for a in $$(ls); do \
		if [ -d $$a ]; then \
			echo "processing folder $$a"; \
			$(MAKE) -C $$a; \
		fi; \
	done;
	@echo "Done!"