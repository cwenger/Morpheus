rmdir /s /q release
for /d /r . %%d in (bin, obj) do @if exist "%%d" rmdir /s /q "%%d"

for /f %%i in ('svnversion -n') do set revision=%%i
if "%revision%"=="Unversioned" set revision="undefined"

mkdir release
mkdir "release\revision %revision%"
mkdir "release\revision %revision%\Morpheus (mzML)"
"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe" "Morpheus\Morpheus (mzML).sln" /rebuild Release
xcopy /y "Morpheus\mzML\bin\Release\*" /EXCLUDE:exclude.txt "release\revision %revision%\Morpheus (mzML)"
xcopy /y "Morpheus\mzML\command line\bin\Release\*" /EXCLUDE:exclude.txt "release\revision %revision%\Morpheus (mzML)"
mkdir "release\revision %revision%\Morpheus (Agilent 32-bit)"
"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe" "Morpheus\Morpheus (Agilent).sln" /rebuild "Release|All"
xcopy /y "Morpheus\Agilent\32-bit\bin\Release\*" /EXCLUDE:exclude.txt "release\revision %revision%\Morpheus (Agilent 32-bit)"
xcopy /y "Morpheus\Agilent\32-bit\command line\bin\Release\*" /EXCLUDE:exclude.txt "release\revision %revision%\Morpheus (Agilent 32-bit)"
mkdir "release\revision %revision%\Morpheus (Agilent 64-bit)"
xcopy /y "Morpheus\Agilent\64-bit\bin\Release\*" /EXCLUDE:exclude.txt "release\revision %revision%\Morpheus (Agilent 64-bit)"
xcopy /y "Morpheus\Agilent\64-bit\command line\bin\Release\*" /EXCLUDE:exclude.txt "release\revision %revision%\Morpheus (Agilent 64-bit)"
mkdir "release\revision %revision%\Morpheus (Thermo)"
"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe" "Morpheus\Morpheus (Thermo).sln" /rebuild Release
xcopy /y "Morpheus\Thermo\bin\Release\*" /EXCLUDE:exclude.txt "release\revision %revision%\Morpheus (Thermo)"
xcopy /y "Morpheus\Thermo\command line\bin\Release\*" /EXCLUDE:exclude.txt "release\revision %revision%\Morpheus (Thermo)"

"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe" ZipFolder\ZipFolder.sln /rebuild Release
ZipFolder\bin\Release\ZipFolder.exe "release\revision %revision%\Morpheus (mzML)"
ZipFolder\bin\Release\ZipFolder.exe "release\revision %revision%\Morpheus (Agilent 32-bit)"
ZipFolder\bin\Release\ZipFolder.exe "release\revision %revision%\Morpheus (Agilent 64-bit)"
ZipFolder\bin\Release\ZipFolder.exe "release\revision %revision%\Morpheus (Thermo)"

rmdir /s /q "release\revision %revision%\Morpheus (mzML)"
rmdir /s /q "release\revision %revision%\Morpheus (Agilent 32-bit)"
rmdir /s /q "release\revision %revision%\Morpheus (Agilent 64-bit)"
rmdir /s /q "release\revision %revision%\Morpheus (Thermo)"
