revision=$(svnversion -n)

rm -r release
find . -name "bin" -type d -exec rm -r "{}" \;

mkdir release
mkdir "release/revision $revision"
mkdir "release/revision $revision/Morpheus (mzML Linux)"
mdtool build "Morpheus/Morpheus (mzML Mono).sln" -t:Clean -c:Release
mdtool build "Morpheus/Morpheus (mzML Mono).sln" -t:Build -c:Release
rsync -a --exclude=*.mdb "Morpheus/mzML/Mono/bin/Release/." "release/revision $revision/Morpheus (mzML Linux)"
rsync -a --exclude=*.mdb "Morpheus/mzML/Mono/command line/bin/Release/." "release/revision $revision/Morpheus (mzML Linux)"

cd "release/revision $revision"
tar -czf "Morpheus (Linux).tar.gz" "Morpheus (mzML Linux)"

rm -r "Morpheus (Linux)"
