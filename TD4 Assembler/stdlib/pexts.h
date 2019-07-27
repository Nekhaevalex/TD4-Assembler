#macro init_pext pext_name mount_point {
    #pext pext_name mount_point
    #define pext_name mount_point
}

#macro pext_id pext_name {
    swm pext_name
    ld 0
}