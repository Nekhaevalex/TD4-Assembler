/*
    (c) 2020 Fedor Gruzdev
    Address selector for TD4E Brainfuck interpreter.

    Notes:
    Takes address from B
    Loads to B
    Trashes A
    Entry point: page 0 cell 0
    Exit point: page 5 cell 13

    Manualy tested for all numbers from 0..15

    Algorythm is based on binary search
*/

// Page 0
mov a, b + 8    // 0
jnc 12          // 1
mov a, b + 4    // 2
jnc 13          // 3
mov a, b + 2    // 4
jnc 14          // 5
mov a, b + 1    // 6
jnc 10          // 7
ld 15           // 8
jmp 11          // 9
ld 14           // 10
swi 4           // 11
swi 1           // 12
swi 2           // 13
swi 3           // 14
add a, 0        // 15
// Page 1
jnc 7           // 0
mov a, b + 9    // 1
jnc 5           // 2
ld 7            // 3
jmp 11          // 4
ld 6            // 5
jmp 11          // 6
swi 3           // 7
add a, 0        // 8
add a, 0        // 9
add a, 0        // 10
swi 4           // 11
swi 4           // 12
mov a, b + 12   // 13
jnc 12          // 14
mov a, b + 10   // 15
// Page 2
mov a, b + 5    // 0
jnc 4           // 1
ld 11           // 2
jmp 11          // 3
ld 10           // 4
jmp 11          // 5
mov a, b + 7    // 6
jnc 10          // 7
ld 9            // 8
jmp 11          // 9
ld 8            // 10
swi 4           // 11
add a, 0        // 12
add a, 0        // 13
mov a, b + 6    // 14
jnc 6           // 15
// Page 3
jnc 3           // 0
ld 13           // 1
jmp 11          // 2
ld 12           // 3
jmp 11          // 4
add a, 0        // 5
add a, 0        // 6
add a, 0        // 7
mov a, b + 11   // 8
jnc 12          // 9
ld 5            // 10
swi 4           // 11
ld 4            // 12
jmp 11          // 13
add a, 0        // 14
mov a, b + 3    // 15
// Page 4
jnc 3           // 0
ld 3            // 1
jmp 12          // 2
ld 2            // 3
jmp 12          // 4
mov a, b + 15   // 5
jnc 9           // 6
ld 1            // 7
jmp 12          // 8
ld 0            // 9
jmp 12          // 10
add a, 0        // 11
swi 5           // 12
mov a, b + 14   // 13
jnc 5           // 14
mov a, b + 13   // 15