rmdir /s /q release
for /d /r . %%d in (bin, obj) do @if exist "%%d" rmdir /s /q "%%d"

for /f %%i in ('git rev-list --count HEAD') do set revision=%%i
if "%revision%"=="Unversioned" set revision="undefined"

mkdir release
mkdir "release\revision %revision%"
mkdir "release\revision %revision%\Morpheus (mzML)"
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\devenv.com" "Morpheus\Morpheus (mzML).sln" /rebuild Release
xcopy /y "Morpheus\mzML\bin\Release\*" /EXCLUDE:exclude.txt "release\revision %revision%\Morpheus (mzML)"
xcopy /y "Morpheus\mzML\command line\bin\Release\*" /EXCLUDE:exclude.txt "release\revision %revision%\Morpheus (mzML)"
copy /y NUL "release\revision %revision%\Morpheus (mzML)\revision %revision%" > NUL
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\devenv.com" "Morpheus\Morpheus (Agilent).sln" /rebuild "Release|x64"
mkdir "release\revision %revision%\Morpheus (Agilent)"
xcopy /y "Morpheus\Agilent\bin\Release\*" /EXCLUDE:exclude.txt "release\revision %revision%\Morpheus (Agilent)"
xcopy /y "Morpheus\Agilent\command line\bin\Release\*" /EXCLUDE:exclude.txt "release\revision %revision%\Morpheus (Agilent)"
copy /y NUL "release\revision %revision%\Morpheus (Agilent)\revision %revision%" > NUL
mkdir "release\revision %revision%\Morpheus (Thermo)"
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\devenv.com" "Morpheus\Morpheus (Thermo).sln" /rebuild Release
xcopy /y "Morpheus\Thermo\bin\Release\*" /EXCLUDE:exclude.txt "release\revision %revision%\Morpheus (Thermo)"
xcopy /y "Morpheus\Thermo\command line\bin\Release\*" /EXCLUDE:exclude.txt "release\revision %revision%\Morpheus (Thermo)"
copy /y NUL "release\revision %revision%\Morpheus (Thermo)\revision %revision%" > NUL

"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\devenv.com" ZipFolder\ZipFolder.sln /rebuild Release
ZipFolder\bin\Release\ZipFolder.exe "release\revision %revision%\Morpheus (mzML)" "release\revision %revision%\Morpheus_mzML.zip"
ZipFolder\bin\Release\ZipFolder.exe "release\revision %revision%\Morpheus (Agilent)" "release\revision %revision%\Morpheus_Agilent.zip"
ZipFolder\bin\Release\ZipFolder.exe "release\revision %revision%\Morpheus (Thermo)" "release\revision %revision%\Morpheus_Thermo.zip"

rmdir /s /q "release\revision %revision%\Morpheus (mzML)"
rmdir /s /q "release\revision %revision%\Morpheus (Agilent)"
rmdir /s /q "release\revision %revision%\Morpheus (Thermo)"
