revision=$(svnversion -n)

rm -r release
rm -r "Morpheus/Morpheus (command line)/bin"

mkdir release
mkdir "release/revision $revision"
mkdir "release/revision $revision/Morpheus (Linux)"
mdtool build "Morpheus/Morpheus (MonoDevelop).sln" -t:Clean -c:Release
mdtool build "Morpheus/Morpheus (MonoDevelop).sln" -t:Build -c:Release
cp -r "Morpheus/Morpheus (command line)/bin/Release/." "release/revision $revision/Morpheus (Linux)"

cd "release/revision $revision"
tar -czf "Morpheus (Linux).tar.gz" "Morpheus (Linux)"

rm -r "Morpheus (Linux)"

