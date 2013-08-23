for /f %%i in ('svnversion -n') do set revision=%%i

mkdir release
mkdir "release\Morpheus revision %revision%"
"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe" Morpheus\Morpheus.sln /rebuild Release /project "Morpheus (command line)"
xcopy "Morpheus\Morpheus (command line)\bin\Release\*" /EXCLUDE:exclude.txt "release\Morpheus revision %revision%"
mkdir "release\Morpheus (Agilent 32-bit) revision %revision%"
"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe" "Morpheus\Morpheus (Agilent).sln" /rebuild Release /project "Morpheus (Agilent 32-bit command line)"
xcopy "Morpheus\Morpheus (command line)\bin\x86\Release\*" /EXCLUDE:exclude.txt "release\Morpheus (Agilent 32-bit) revision %revision%"
mkdir "release\Morpheus (Agilent 64-bit) revision %revision%"
"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe" "Morpheus\Morpheus (Agilent).sln" /rebuild Release /project "Morpheus (Agilent 64-bit command line)"
xcopy "Morpheus\Morpheus (command line)\bin\x64\Release\*" /EXCLUDE:exclude.txt "release\Morpheus (Agilent 64-bit) revision %revision%"
mkdir "release\Morpheus (Thermo) revision %revision%"
"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe" "Morpheus\Morpheus (Thermo).sln" /rebuild Release /project "Morpheus (Thermo command line)"
xcopy "Morpheus\Morpheus (command line)\bin\Thermo\Release\*" /EXCLUDE:exclude.txt "release\Morpheus (Thermo) revision %revision%"


"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe" ZipFolder\ZipFolder.sln /rebuild Release /project "ZipFolder"
ZipFolder\bin\Release\ZipFolder.exe "release\Morpheus revision %revision%"
ZipFolder\bin\Release\ZipFolder.exe "release\Morpheus (Agilent 32-bit) revision %revision%"
ZipFolder\bin\Release\ZipFolder.exe "release\Morpheus (Agilent 64-bit) revision %revision%"
ZipFolder\bin\Release\ZipFolder.exe "release\Morpheus (Thermo) revision %revision%"
