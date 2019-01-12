# _________  ________  ___   ___          ________  ________
#|\___   ___\\   ___ \|\  \ |\  \        |\   __  \|\   ____\
#\|___ \  \_\ \  \_|\ \ \  \\_\  \       \ \  \|\  \ \  \___|_
#     \ \  \ \ \  \ \\ \ \______  \       \ \  \\\  \ \_____  \
#      \ \  \ \ \  \_\\ \|_____|\  \       \ \  \\\  \|____|\  \
#       \ \__\ \ \_______\     \ \__\       \ \_______\____\_\  \
#        \|__|  \|_______|      \|__|        \|_______|\_________\
#                                                     \|_________|
#
# Boot sequence:
# I) Bootloader
# 	1) Start booting
# II) OS
# 	1) Define used pexts
# 	2) Define avaliable memory
# 	3) Define avaliable cores
# 	4) Define runnable kernel extentions
# 	5) Run OS menu
# RAM Map:
# Page 0:
#	0:	Avaliable RAM pages
#	1:	Cores amount
#	2-15: PEXTS
#
# ©2018 JL Computer Inc. All rights reserved
# Author: Alexander Nekhaev
#Part 1: Menu
#	1) Select program
#	2) Restart
#	3) Shutdown

%import sys.h
%pext alu.pext 15

.boot:
	in a
	mappxt 2
.test3:
	add a 13		# 0 = 13 | 1 = 14 | 2 = 15 | 3 = 16
	jncl .test2
	hlt
.test2:
	add a 1 		# 0 = 14 |14 = 15 | 15 = 16
	jncl .test1
	jmpl .boot
.test1:
	add a 1			# 0 = 15 |15 = 16
	jncl .boot
	jmpl .load
#Part 2: Program place
.load:
