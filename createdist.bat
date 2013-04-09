@echo off

rm -r -f dist

goto android

:windows_vc9
echo.
echo BUILDING FOR WINDOWS (vc9)
echo.

cd API/build/vc/vs2008
call devenv Otter.sln /clean "Win Release MD|Win32"
call devenv Otter.sln /clean "Win Release MT|Win32"
call devenv Otter.sln /clean "Win Debug MDd|Win32"
call devenv Otter.sln /clean "Win Debug MTd|Win32"
call devenv Otter.sln /clean "Win Release MD|x64"
call devenv Otter.sln /clean "Win Release MT|x64"
call devenv Otter.sln /clean "Win Debug MDd|x64"
call devenv Otter.sln /clean "Win Debug MTd|x64"
call devenv Otter.sln /build "Win Release MD|Win32" /project "Otter.vcproj"
call devenv Otter.sln /build "Win Release MT|Win32" /project "Otter.vcproj"
call devenv Otter.sln /build "Win Debug MDd|Win32" /project "Otter.vcproj"
call devenv Otter.sln /build "Win Debug MTd|Win32" /project "Otter.vcproj"
call devenv Otter.sln /build "Win Release MD|x64" /project "Otter.vcproj"
call devenv Otter.sln /build "Win Release MT|x64" /project "Otter.vcproj"
call devenv Otter.sln /build "Win Debug MDd|x64" /project "Otter.vcproj"
call devenv Otter.sln /build "Win Debug MTd|x64" /project "Otter.vcproj"
cd ../../../..


:tool
echo.
echo BUILDING TOOL
echo.

cd Tool
call devenv OtterEditor.sln /project Setup\Setup.vdproj /rebuild "Release"
cd ..

:android
echo.
echo BUILDING FOR ANDROID
echo.

cd API/build/android
call make clean
call make PLATFORM=android TARGET=release
call make PLATFORM=android TARGET=debug
cd ../../..

:dist
echo.
echo CREATING DISTRIBUTION FOLDERS
echo.

call mkdir dist\API
call mkdir dist\SampleApp
call mkdir dist\Editor

echo.
echo COPYING TO DISTRIBUTION FOLDERS
echo.

cp -r API/inc dist/API
cp -r API/lib dist/API
cp -r SampleApp/* dist/SampleApp
cp -r Tool/Setup/Release/* dist/Editor
cp -r Docs/README.txt dist/README.txt

rm -r -f dist/SampleApp/platforms/Android/libs
rm -r -f dist/SampleApp/platforms/Android/obj
rm -r -f dist/SampleApp/platforms/Android/gen
rm -r -f dist/SampleApp/platforms/Android/bin
rm -r -f "dist/SampleApp/platforms/Win/tmp"
rm -r -f dist/SampleApp/platforms/Win/ipch
rm -r -f dist/SampleApp/platforms/Win/*.sdf
rm -r -f dist/SampleApp/platforms/Win/*.user
rm -r -f dist/SampleApp/platforms/Win/*.suo
rm -r -f dist/SampleApp/platforms/Win/*.ncb
rm -r -f dist/SampleApp/plugin/bin
rm -r -f dist/SampleApp/plugin/obj
rm -r -f dist/SampleApp/plugin/*.suo
rm -r -f dist/SampleApp/plugin/*.sdf
rm -r -f dist/SampleApp/plugin/*.user

REM Prepare the Unity Plugin
cp dist/API/lib/unity/PC/OtterCMT.dll dist/API/lib/unity/OtterC.dll
cp -r dist/API/lib/unity/MacOS/OtterC.bundle dist/API/lib/unity

rm -r -f dist/API/lib/unity/PC
rm -r -f dist/API/lib/unity/MacOS

REM Prepare the Unity Sample App package
rm -r -f dist/SampleApp/platforms/Unity
cp -r -f SampleApp/platforms/Unity/Package dist/SampleApp/platforms/Unity

REM Remove Unity
rm -r -f dist/SampleApp/platforms/Unity
rm -r -f dist/API/lib/unity

REM Remove PS3 and SDL for now
call find ./dist -name 'PS3' -exec rm -f -r {} ;
call find ./dist -name 'ps3' -exec rm -f -r {} ;
call find ./dist -name 'SDL' -exec rm -f -r {} ;
call find ./dist -name 'sdl' -exec rm -f -r {} ;
call find ./dist -name 'RAW' -exec rm -f -r {} ;

call find ./dist -name '*.svn' -exec rm -f -r {} ;

:end