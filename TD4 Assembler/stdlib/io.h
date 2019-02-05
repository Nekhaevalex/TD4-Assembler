set_io_on $v1 {
	%pext io.pext $v1
}

set_addr $v0 $v1 $v2 $v3 $v4 $v5 $v6 $v7 $v8 $v9 $v10 $v11 {
	mov b, $v0
	st 0
	mov b, $v1
	st 1
	mov b, $v2
	st 2
	mov b, $v3
	st 3
	mov b, $v4
	st 4
	mov b, $v5
	st 5
	mov b, $v6
	st 6
	mov b, $v7
	st 7
	mov b, $v8
	st 8
	mov b, $v9
	st 9
	mov b, $v10
	st 10
	mov b, $v11
	st 11
}

mode_read {
	mov b, 0
	st 12
}

mode_write {
	mov b, 1
	st 12
}

enable_auto_inc {
	mov b, 1
	st 13
} 
disable_auto_inc {
	mov b, 0
	st 13
}

bus_on $v1 {
	mov b, 1
	st $v1
}

bus_off $v1 {
	mov b, 0
	st $v1
}