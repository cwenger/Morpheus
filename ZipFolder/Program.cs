using System.IO;
using System.IO.Compression;

namespace ZipFolder
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = args[0];
            ZipFile.CreateFromDirectory(path, Path.ChangeExtension(path, ".zip"), CompressionLevel.Optimal, true);
        }
    }
}
