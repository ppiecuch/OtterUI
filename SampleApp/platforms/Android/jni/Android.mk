LOCAL_PATH := $(call my-dir)

#------------------------------------------------------------------------------
# MAIN LIB
#------------------------------------------------------------------------------
include $(CLEAR_VARS)

LOCAL_MODULE := ottersample

LOCAL_CFLAGS := -DANDROID_NDK
LOCAL_CFLAGS += -DPLATFORM_ANDROID
LOCAL_CFLAGS += -DDISABLE_IMPORTGL
LOCAL_CFLAGS += -I../../../../API/inc
LOCAL_CFLAGS += -I../../../src
				
LOCAL_SRC_FILES := main.cpp
LOCAL_SRC_FILES += ../../../src/FileSystems/SampleFileSystem.cpp
LOCAL_SRC_FILES += ../../../src/Renderers/SampleRenderer.cpp
LOCAL_SRC_FILES += ../../../src/Renderers/OGLESRenderer.cpp
LOCAL_SRC_FILES += ../../../src/SoundSystems/AndroidSoundSystem.cpp
LOCAL_SRC_FILES += ../../../src/Views/IntroHandler.cpp
LOCAL_SRC_FILES += ../../../src/Views/BasicControlsViewHandler.cpp
LOCAL_SRC_FILES += ../../../src/Views/LabelsViewHandler.cpp
LOCAL_SRC_FILES += ../../../src/Views/MaskViewHandler.cpp
LOCAL_SRC_FILES += ../../../src/Views/TablesViewHandler.cpp
LOCAL_SRC_FILES += ../../../src/Views/ViewHandler.cpp
LOCAL_SRC_FILES += ../../../src/Plugins/SamplePlugin.cpp
LOCAL_SRC_FILES += ../../../src/Plugins/Circle.cpp
LOCAL_SRC_FILES += ../../../src/SampleUI.cpp

LOCAL_LDLIBS := -lGLESv1_CM -ldl -llog -lz -lstdc++
LOCAL_LDLIBS += -Wl,--no-whole-archive ../../../../API/lib/android/libOtter.a
LOCAL_LDLIBS += -Wl,--no-whole-archive libpng/libpng.a

LOCAL_ARM_MODE := arm
include $(BUILD_SHARED_LIBRARY)
