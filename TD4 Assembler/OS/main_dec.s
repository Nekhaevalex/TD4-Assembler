;Disassembly of main:
in	a		;0
add	a, 13	;1
jnc	4		;2
hlt			;3
add	a, 1	;4
jnc	7		;5
jmp	0		;6
add	a, 1	;7
jnc	0		;8
jmp	0		;9
swm	15		;10
st	1		;11
mov	b, a	;12
st	0		;13
ld	2		;14
swi	1		;15
;******************
;     Next page
;******************
