revision=$(git rev-list --count HEAD)

rm -r release
find . -name "bin" -type d -exec rm -r "{}" \;

mkdir release
mkdir "release/revision $revision"

mkdir "release/revision $revision/Morpheus (mzML Mono)"
mdtool build "Morpheus/Morpheus (mzML Mono).sln" -t:Clean -c:Release
mdtool build "Morpheus/Morpheus (mzML Mono).sln" -t:Build -c:Release
cp -f /dev/null "release/revision $revision/Morpheus (mzML Mono)/revision $revision"
rsync -a --exclude=*.mdb "Morpheus/mzML/Mono/bin/Release/." "release/revision $revision/Morpheus (mzML Mono)"
rsync -a --exclude=*.mdb "Morpheus/mzML/Mono/command line/bin/Release/." "release/revision $revision/Morpheus (mzML Mono)"

cd "release/revision $revision"
tar -czf "Morpheus_mzML_Mono.tar.gz" "Morpheus (mzML Mono)"

rm -rf "Morpheus (mzML Mono)"
