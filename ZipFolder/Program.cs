using System.IO;
using System.IO.Compression;

namespace ZipFolder
{
    class Program
    {
        static void Main(string[] args)
        {
            string input_path = args[0];
            string output_path;
            if (args.Length > 1)
                output_path = args[1];
            else
                output_path = Path.ChangeExtension(input_path, ".zip");
            ZipFile.CreateFromDirectory(input_path, output_path, CompressionLevel.Optimal, true);
        }
    }
}
