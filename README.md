Ассебмлер для TD4+

Текущая версия поддерживает метки
"Нативные" операции:
0000	Add	A, Im	(A=A+Im)
0001	Mov	A, B	(A=B)
0010	In	A	(A=In)
0011	Mov	A, Im	(A=Im)
0100	Mov	B, A	(B=A)
0101	Add	B, Im	(B=B+Im)
0110	In	B	(B=In)
0111	Mov	B, Im	(B=Im)
1000	Swi	Im	(Switch memory bank)
1001	Out	B	(Out=B)
1010	Swm	Im	(Switch memory page)
1011	Out	Im	(Out=Im)
1100	Ld	Im	(Load from RAM[Im] to b)
1101	St	Im	(Store from b to RAM[Im]
1110	JNC	Im	(PC=Im if C!=1)
1111	JMP	Im	(PC=Im)

"Наши" операции

Дополнительные операции
label: -- метка
jmpl -- jmp to label
jncl -- jnc to label
nop <=> add a 0
hlt <=> jump PC

Подключение pExt
%pext <name.pext> <bank number>