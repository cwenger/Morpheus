using System;

namespace Morpheus
{
    public class FilepathEventArgs : EventArgs
    {
        public string Filepath { get; private set; }

        public FilepathEventArgs(string filepath)
        {
            Filepath = filepath;
        }
    }
}