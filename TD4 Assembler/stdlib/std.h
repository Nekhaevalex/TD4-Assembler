//nop macros
#macro nop {
	add a 0
}

//hlt macros
#macro hlt {
	hlt_label:
	jmp hlt_label
}