#ifndef PEXTS_H
#define PEXTS_H

#macro init_pext pext_name, mount_point
    #pext pext_name mount_point
    #ifndef pext_name
    #define pext_name mount_point
    #endif
#endmacro

#macro pext_id pext_name
    swm pext_name
    ld 0
#endmacro

#endif