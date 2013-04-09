#!/bin/bash

DEVICE=iphoneos4.2
SIMULATOR=iphonesimulator4.2

xcodebuild -target "Otter" -configuration Debug -sdk ${DEVICE}
xcodebuild -target "Otter" -configuration Debug -sdk ${SIMULATOR}
xcodebuild -target "Otter" -configuration Release -sdk ${DEVICE}
xcodebuild -target "Otter" -configuration Release -sdk ${SIMULATOR}

xcodebuild -target "OtterC" -configuration Debug -sdk ${DEVICE}
xcodebuild -target "OtterC" -configuration Debug -sdk ${SIMULATOR}
xcodebuild -target "OtterC" -configuration Release -sdk ${DEVICE}
xcodebuild -target "OtterC" -configuration Release -sdk ${SIMULATOR}

DEBUG_DEVICE_DIR=build/Debug-iphoneos
DEBUG_SIMULATOR_DIR=build/Debug-iphonesimulator

RELEASE_DEVICE_DIR=build/Release-iphoneos
RELEASE_SIMULATOR_DIR=build/Release-iphonesimulator

OUTPUT_DIR=../../lib/ios
C_OUTPUT_DIR=../../lib/unity/ios

rm -rf "${OUTPUT_DIR}/*.a"

lipo -create -output "${OUTPUT_DIR}/libOtterD.a" "${DEBUG_DEVICE_DIR}/libOtter.a" "${DEBUG_SIMULATOR_DIR}/libOtter.a"
lipo -create -output "${OUTPUT_DIR}/libOtter.a" "${RELEASE_DEVICE_DIR}/libOtter.a" "${RELEASE_SIMULATOR_DIR}/libOtter.a"

lipo -create -output "${OUTPUT_DIR}/libOtterCD.a" "${DEBUG_DEVICE_DIR}/libOtterC.a" "${DEBUG_SIMULATOR_DIR}/libOtterC.a"
lipo -create -output "${OUTPUT_DIR}/libOtterC.a" "${RELEASE_DEVICE_DIR}/libOtterC.a" "${RELEASE_SIMULATOR_DIR}/libOtterC.a"

cp -f "${OUTPUT_DIR}/libOtterC.a" "../../lib/unity/iOS/libOtterC.a"