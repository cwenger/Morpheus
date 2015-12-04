rmdir /s /q release
for /d /r . %%d in (bin, obj) do @if exist "%%d" rmdir /s /q "%%d"

for /f %%i in ('git rev-list --count HEAD') do set revision=%%i
if "%revision%"=="Unversioned" set revision="undefined"

mkdir release
mkdir "release\revision %revision%"
mkdir "release\revision %revision%\Morpheus (mzML)"
"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe" "Morpheus\Morpheus (mzML).sln" /rebuild Release
xcopy /y "Morpheus\mzML\bin\Release\*" /EXCLUDE:exclude.txt "release\revision %revision%\Morpheus (mzML)"
xcopy /y "Morpheus\mzML\command line\bin\Release\*" /EXCLUDE:exclude.txt "release\revision %revision%\Morpheus (mzML)"
"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe" "Morpheus\Morpheus (Agilent).sln" /rebuild "Release|x64"
mkdir "release\revision %revision%\Morpheus (Agilent)"
xcopy /y "Morpheus\Agilent\bin\Release\*" /EXCLUDE:exclude.txt "release\revision %revision%\Morpheus (Agilent)"
xcopy /y "Morpheus\Agilent\command line\bin\Release\*" /EXCLUDE:exclude.txt "release\revision %revision%\Morpheus (Agilent)"
mkdir "release\revision %revision%\Morpheus (Thermo)"
"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe" "Morpheus\Morpheus (Thermo).sln" /rebuild Release
xcopy /y "Morpheus\Thermo\bin\Release\*" /EXCLUDE:exclude.txt "release\revision %revision%\Morpheus (Thermo)"
xcopy /y "Morpheus\Thermo\command line\bin\Release\*" /EXCLUDE:exclude.txt "release\revision %revision%\Morpheus (Thermo)"

"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe" ZipFolder\ZipFolder.sln /rebuild Release
ZipFolder\bin\Release\ZipFolder.exe "release\revision %revision%\Morpheus (mzML)"
ZipFolder\bin\Release\ZipFolder.exe "release\revision %revision%\Morpheus (Agilent)"
ZipFolder\bin\Release\ZipFolder.exe "release\revision %revision%\Morpheus (Thermo)"

rmdir /s /q "release\revision %revision%\Morpheus (mzML)"
rmdir /s /q "release\revision %revision%\Morpheus (Agilent)"
rmdir /s /q "release\revision %revision%\Morpheus (Thermo)"
