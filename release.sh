rm -r release
rm -r "Morpheus/Morpheus (command line)/bin"

mkdir release
mkdir "release/Morpheus (Linux)"
mdtool build "Morpheus/Morpheus (MonoDevelop).sln" -t:Clean -c:Release
mdtool build "Morpheus/Morpheus (MonoDevelop).sln" -t:Build -c:Release
cp -r "Morpheus/Morpheus (command line)/bin/Release/." "release/Morpheus (Linux)"

cd release
tar -czf "Morpheus (Linux).tar.gz" "Morpheus (Linux)"

