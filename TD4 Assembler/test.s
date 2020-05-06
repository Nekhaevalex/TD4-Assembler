    OUT 0   
    ADD A 1 
    JNC 0   
    MOV A 11
    ADD A 1 
    JNC 4   
    OUT 15  
    JMP 7   
    jmp b2
lab1:
    out 1
    add a, 1
b2: out b
    jnc lab1
mov a b -12
mov a b +12

add a 1