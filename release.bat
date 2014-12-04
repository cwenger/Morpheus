rmdir /s /q release
rmdir /s /q "Morpheus\Morpheus (command line)\obj"
rmdir /s /q "Morpheus\Morpheus (command line)\bin"

for /f %%i in ('svnversion -n') do set revision=%%i
if "%revision%"=="Unversioned" set revision="undefined"

mkdir release
mkdir "release\revision %revision%"
mkdir "release\revision %revision%\Morpheus"
"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe" Morpheus\Morpheus.sln /rebuild Release /project "Morpheus (command line)"
xcopy "Morpheus\Morpheus (command line)\bin\Release\*" /EXCLUDE:exclude.txt "release\revision %revision%\Morpheus"
mkdir "release\revision %revision%\Morpheus (Agilent 32-bit)"
"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe" "Morpheus\Morpheus (Agilent).sln" /rebuild "Release|32-bit" /project "Morpheus (Agilent 32-bit command line)"
xcopy "Morpheus\Morpheus (command line)\bin\Agilent\32-bit\Release\*" /EXCLUDE:exclude.txt "release\revision %revision%\Morpheus (Agilent 32-bit)"
mkdir "release\revision %revision%\Morpheus (Agilent 64-bit)"
"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe" "Morpheus\Morpheus (Agilent).sln" /rebuild "Release|64-bit" /project "Morpheus (Agilent 64-bit command line)"
xcopy "Morpheus\Morpheus (command line)\bin\Agilent\64-bit\Release\*" /EXCLUDE:exclude.txt "release\revision %revision%\Morpheus (Agilent 64-bit)"
mkdir "release\revision %revision%\Morpheus (Thermo)"
"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe" "Morpheus\Morpheus (Thermo).sln" /rebuild Release /project "Morpheus (Thermo command line)"
xcopy "Morpheus\Morpheus (command line)\bin\Thermo\Release\*" /EXCLUDE:exclude.txt "release\revision %revision%\Morpheus (Thermo)"

"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe" ZipFolder\ZipFolder.sln /rebuild Release /project "ZipFolder"
ZipFolder\bin\Release\ZipFolder.exe "release\revision %revision%\Morpheus"
ZipFolder\bin\Release\ZipFolder.exe "release\revision %revision%\Morpheus (Agilent 32-bit)"
ZipFolder\bin\Release\ZipFolder.exe "release\revision %revision%\Morpheus (Agilent 64-bit)"
ZipFolder\bin\Release\ZipFolder.exe "release\revision %revision%\Morpheus (Thermo)"

rmdir /s /q "release\revision %revision%\Morpheus"
rmdir /s /q "release\revision %revision%\Morpheus (Agilent 32-bit)"
rmdir /s /q "release\revision %revision%\Morpheus (Agilent 64-bit)"
rmdir /s /q "release\revision %revision%\Morpheus (Thermo)"
