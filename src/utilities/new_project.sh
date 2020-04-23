#!/bin/bash

mkdir $1

echo "#import std.h" > $1/main.s
echo "#import malloc.h" >> $1/main.s
echo "#import string.h" >> $1/main.s
echo "#import pexts.h" >> $1/main.s
echo "#import io.h" >> $1/main.s
echo "#import $1.h" >> $1/main.s

echo "" >> $1/main.s

echo "init_allocator" >> $1/main.s
echo "main" >> $1/main.s
echo "stop_allocator" >> $1/main.s

echo "#ifndef $1_H" > $1/$1.h
echo "#define $1_H" >> $1/$1.h
echo "#macro main" >> $1/$1.h
echo "//Your code here" >> $1/$1.h
echo "#endmacro" >> $1/$1.h
