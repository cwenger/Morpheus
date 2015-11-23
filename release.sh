revision=$(svnversion -n)

rm -r release
find . -name "bin" -type d -exec rm -r "{}" \;

mkdir release
mkdir "release/revision $revision"
mkdir "release/revision $revision/Morpheus (Linux)"
mdtool build "Morpheus/Morpheus (MonoDevelop).sln" -t:Clean -c:Release
mdtool build "Morpheus/Morpheus (MonoDevelop).sln" -t:Build -c:Release
cp -r "Morpheus/mzML/MonoDevelop/command line/bin/Release/." "release/revision $revision/Morpheus (Linux)"

cd "release/revision $revision"
tar -czf "Morpheus (Linux).tar.gz" "Morpheus (Linux)"

rm -r "Morpheus (Linux)"

