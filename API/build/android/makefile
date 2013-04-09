# Builds the Otter Library
PROJ = Otter

LIB_DIR 			:= ./../../lib
TMP_DIR 			:= ./tmp
SUFFIX				:=
LDFLAGS				:=
CCFLAGS				:=
TOOLCHAIN_PREFIX	:=
ARM_EABI_VER		:= 4.7
ANDROID_SDK_LEVEL	:= android-8
PLATFORM			:=

ifeq ($(TARGET),debug)
	SUFFIX				:= D
else
	TARGET				:= release
endif

BIN_DIR 			:= $(LIB_DIR)/$(PLATFORM)
OBJ_DIR 			:= $(TMP_DIR)/$(PLATFORM)/$(TARGET)

SOURCE_DIRS 		:= ../../src
SOURCE_DIRS			+= ../../src/Common
SOURCE_DIRS			+= ../../src/Data
SOURCE_DIRS			+= ../../src/FileSys
SOURCE_DIRS			+= ../../src/Math
SOURCE_DIRS			+= ../../src/Memory
SOURCE_DIRS			+= ../../src/Renderer
SOURCE_DIRS			+= ../../src/Graphics
SOURCE_DIRS			+= ../../src/Utilities
SOURCE_DIRS			+= ../../src/utf8proc

FILES 				:= $(foreach DIR,$(SOURCE_DIRS),$(wildcard $(DIR)/*.cpp))
FILES2 				:= $(foreach SRC,$(FILES),$(subst ../../,,$(SRC)))
OBJS 				:= $(foreach SRC,$(FILES2),$(OBJ_DIR)/$(SRC:.cpp=.o))

# Add the include directories

INCLUDE_DIRS		:= ../../inc
INCLUDE_DIRS		+= ../../inc/Common
INCLUDE_DIRS		+= ../../inc/Interfaces
INCLUDE_DIRS		+= ../../inc/Math
INCLUDE_DIRS		+= ../../inc/Renderer
INCLUDE_DIRS		+= ../../inc/Graphics
INCLUDE_DIRS		+= ../../src

CCFLAGS 			+= $(foreach INC_DIR,$(INCLUDE_DIRS),-I$(INC_DIR))

ifeq ($(PLATFORM),android)
	NDK_HOST 			:= windows-x86_64
	SDKTOOL 			:= $(ANDROID_SDK_DIR)/tools
	TOOLCHAIN_PREFIX 		:= $(ANDROID_NDK_DIR)/toolchains/arm-linux-androideabi-$(ARM_EABI_VER)/prebuilt/$(NDK_HOST)/bin/arm-linux-androideabi-
	
	CCFLAGS				+= -I$(ANDROID_NDK_DIR)/sources/cxx-stl/gnu-libstdc++/$(ARM_EABI_VER)/include
	CCFLAGS				+= -I$(ANDROID_NDK_DIR)/sources/cxx-stl/gnu-libstdc++/$(ARM_EABI_VER)/libs/armeabi/include
	CCFLAGS				+= -I$(ANDROID_NDK_DIR)/platforms/$(ANDROID_SDK_LEVEL)/arch-arm/usr/include 
	CCFLAGS				+= -I./../../../API/inc
	
	CCFLAGS				+= -fpic 
	CCFLAGS				+= -mthumb-interwork 
	CCFLAGS				+= -ffunction-sections 
	CCFLAGS				+= -funwind-tables 
	CCFLAGS				+= -fstack-protector 
	CCFLAGS				+= -fno-short-enums 
	CCFLAGS				+= -D__ARM_ARCH_5__ 
	CCFLAGS				+= -D__ARM_ARCH_5T__ 
	CCFLAGS				+= -D__ARM_ARCH_5E__ 
	CCFLAGS				+= -D__ARM_ARCH_5TE__  
	CCFLAGS				+= -Wno-psabi 
	CCFLAGS				+= -march=armv5te 
	CCFLAGS				+= -mtune=xscale 
	CCFLAGS				+= -msoft-float 
	CCFLAGS				+= -fno-exceptions 
	CCFLAGS				+= -fno-rtti 
	CCFLAGS				+= -fomit-frame-pointer 
	CCFLAGS				+= -fstrict-aliasing 
	CCFLAGS				+= -funswitch-loops 
	CCFLAGS				+= -finline-limit=300
	CCFLAGS				+= -DANDROID 
	CCFLAGS				+= -DANDROID_NDK
	CCFLAGS				+= -Wa,--noexecstack
endif

ifeq ($(TARGET),debug)
	CCFLAGS 			+= -g
	CCFLAGS				+= -DDEBUG
else
	CCFLAGS 			+= -O2
	CCFLAGS 			+= -DNDEBUG
	CCFLAGS 			+= -UDEBUG
endif

CPP 				:= $(TOOLCHAIN_PREFIX)g++
LD 					:= $(CPP)
AR					:= $(TOOLCHAIN_PREFIX)ar

OUTFILE 			:= lib$(PROJ)$(SUFFIX).a

$(PROJ) 		: makefile create_dirs make_lib

make_lib		: $(OBJS)
				@echo Creating library: $(OUTFILE)
				@ $(AR) crs $(BIN_DIR)/$(OUTFILE) $^
				
create_dirs		:
				mkdir -p $(OBJ_DIR)
				mkdir -p $(BIN_DIR)
						
$(OBJ_DIR)/%.o	: ../../%.cpp
				@echo Compiling $< to $@
				@ test -d $(dir $@) || mkdir -p $(dir $@)
				@ $(CPP) $(CCFLAGS) -c $< -o $@

clean			:
				rm -rf $(TMP_DIR)
