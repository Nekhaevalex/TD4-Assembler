//Initialize I/O pext and store it's address
#define $vM null

#macro init_io $v1 {
	#pext io.pext $v1
	#define $vM $v1
}

//Set working address
#macro set_addr $v0 $v1 $v2 $v3 $v4 $v5 $v6 $v7 $v8 $v9 $v10 $v11 {
	swm $vM
	mov b, $v0
	st 1
	mov b, $v1
	st 2
	mov b, $v2
	st 3
	mov b, $v3
	st 4
	mov b, $v4
	st 5
	mov b, $v5
	st 6
	mov b, $v6
	st 7
	mov b, $v7
	st 8
	mov b, $v8
	st 9
	mov b, $v9
	st 10
	mov b, $v10
	st 11
	mov b, $v11
	st 12
}

//Prepare for reading
#macro mode_read {
	swm $vM
	mov b, 0
	st 13
}

//Prepare for writing
#macro mode_write {
	swm $vM
	mov b, 1
	st 13
}

//Enable auto incrementation -- after writing data address will be increamented automaticly by pext
#macro enable_auto_inc {
	swm $vM
	mov b, 1
	st 14
} 

//Disable auto increment
#macro disable_auto_inc {
	swm $vM
	mov b, 0
	st 14
}

//Enable bus
#macro bus_on $v1 {
	swm $vM
	mov b, 1
	st $v1
}

//Disable bus
#macro bus_off $v1 {
	swm $vM
	mov b, 0
	st $v1
}