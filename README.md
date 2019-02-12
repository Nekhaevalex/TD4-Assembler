Ассебмлер для TD4+

Текущая версия поддерживает метки

* Операции
0000	Add	A, Im	(A=A+Im)<br/>
0001	Mov	A, B	(A=B)<br/>
0010	In	A	(A=In)<br/>
0011	Mov	A, Im	(A=Im)<br/>
0100	Mov	B, A	(B=A)<br/>
0101	Add	B, Im	(B=B+Im)<br/>
0110	In	B	(B=In)<br/>
0111	Mov	B, Im	(B=Im)<br/>
1000	Swi	Im	(Switch memory bank)<br/>
1001	Out	B	(Out=B)<br/>
1010	Swm	Im	(Switch memory page)<br/>
1011	Out	Im	(Out=Im)<br/>
1100	Ld	Im	(Load from RAM[Im] to b)<br/>
1101	St	Im	(Store from b to RAM[Im]<br/>
1110	JNC	Im	(PC=Im if C!=1)<br/>
1111	JMP	Im	(PC=Im)<br/>

* Дополнительные ключевые слова
label: -- метка<br/>
jmpl -- jmp to label<br/>
jncl -- jnc to label<br/>
nop <=> add a 0<br/>
hlt <=> jump PC<br/>

Подключение pExt<br/>
%pext <name.pext> <bank number>