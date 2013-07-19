mkdir release
mkdir release\Morpheus
"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe" "Morpheus\Morpheus.sln" /rebuild Release /project "Morpheus (command line)"
xcopy "Morpheus\Morpheus (command line)\bin\Release\*" /EXCLUDE:exclude.txt "release\Morpheus"
mkdir "release\Morpheus (Agilent 32-bit)"
"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe" "Morpheus\Morpheus (Agilent).sln" /rebuild Release /project "Morpheus (Agilent 32-bit)"
xcopy "Morpheus\bin\x86\Release\*" /EXCLUDE:exclude.txt "release\Morpheus (Agilent 32-bit)"
mkdir "release\Morpheus (Agilent 64-bit)"
"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe" "Morpheus\Morpheus (Agilent).sln" /rebuild Release /project "Morpheus (Agilent 64-bit)"
xcopy "Morpheus\bin\x64\Release\*" /EXCLUDE:exclude.txt "release\Morpheus (Agilent 64-bit)"
mkdir "release\Morpheus (Thermo)"
"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe" "Morpheus\Morpheus (Thermo).sln" /rebuild Release
xcopy "Morpheus\bin\Thermo\Release\*" /EXCLUDE:exclude.txt "release\Morpheus (Thermo)"
