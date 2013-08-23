revision=$(svnversion -n)

mkdir release
mkdir "release/Morpheus (Linux) revision $revision"
mdtool build "Morpheus/Morpheus (MonoDevelop).sln" -t:Clean -c:Release
mdtool build "Morpheus/Morpheus (MonoDevelop).sln" -t:Build -c:Release
cp -r "Morpheus/Morpheus (command line)/bin/Release/." "release/Morpheus (Linux) revision $revision"

cd release
tar -czf "Morpheus (Linux) revision $revision.tar.gz" "Morpheus (Linux) revision $revision"

