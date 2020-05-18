#macro define_machine
    #pragma DEF_SYS
    #pragma CONFIG

    #message ===BUILD_PARAMS===
    #ifndef 8_BIT
    #message TINY_NO_OUTPUT_BUILD
    #else
    #message RICH_TEXT_OS
    #endif

    #ifdef config.ALU
    #message ALU_AVALIABLE
    init_pext alu.pext config.ALU
    #endif

    #ifdef config.IO
    #message IO_ROUTER_AVALIABLE
    init_io config.IO
    add_static_device TTY config.IO.TTY1
    #define stdout TTY
    #define stdin TTY
    #endif

    #ifdef config.MULTICORE
    #message MULTICORE_TD4
    #pext mcl.pext config.MCL
    #endif
#endmacro

#macro greetings
    #ifdef 8_BIT
    print "TD4E8_OS"
    #endif
    out 10
#endmacro

#macro strt_wrt
    out '>'
#endmacro

#macro rp selected_val_addr, caller
    #ifdef 8_BIT
    print "page_to_load:"
    #endif
    //Return
    jmp caller
#endmacro

#macro sd
    hlt
#endmacro

#macro reboot init_label
    jmp init_label
#endmacro

#macro help
    #ifdef 8_BIT
    print "1-help"
    out 10
    print "2-run"
    out 10
    print "3-reboot"
    out 10
    print ">=4-shutdown"
    out 10
    #else
    nop
    #endif
#endmacro

#macro interpreter h_label, r_label, b_label, s_label
    mov a, 0
    mov b, 0
    #ifdef 8_BIT
    strt_wrt
    #endif
    getc selected
    ld selected
    mov a, b -48
    nop
    mov b, a
    gqt 4, s_label
    mov a, b
    gqt 3, b_label
    mov a, b
    gqt 2, r_label
    mov a, b
    gqt 1, h_label
    #undef selected
#endmacro

#macro main
    #message BUILDING_CORE...
    define_machine
init:
    db 0, SELECTED_PROGRAM_PAGE
    greetings
cmd:
    interpreter help_label, run_label, boot_label, shut_label
help_label:
    help
    jmp cmd
run_label:
    rp SELECTED_PROGRAM_PAGE, cmd
boot_label:
    nop
    reboot init
shut_label:
    free SELECTED_PROGRAM_PAGE
    sd
    #undef SELECTED_PROGRAM_PAGE
#endmacro