    mov b, 1
    st 0
#pragma DEF_SYS
#pragma CONFIG
init:
    mov b, 1
    st 0
    mov b, 0
    st address
    out 10
cmd:
    mov a, 0
    mov b, 0
    mov b, 1
    st 0
scan:
    mov b, 0
    in b
    mov a, b
    add a, 15
    jnc scan
    st ptr
    mov a, 0
    ld selected
    mov a, b -48
    add a, 0
    mov b, a
    add a, 11
    jnc shut_label
    mov a, b
    add a, 12
    jnc boot_label
    mov a, b
    add a, 13
    jnc run_label
    mov a, b
    add a, 14
    jnc help_label
help_label:
    add a, 0
    jmp cmd
run_label:
    jmp cmd
boot_label:
    add a, 0
    jmp init
shut_label:
    mov b, 0
    st 0
    add a, 0
hlt_label:
    jmp hlt_label