using WormsLab.Interfaces;

namespace WormsLab.Writers
{
    public class FileWriter : IDisposable, IWriter
    {
        private StreamWriter _streamWriter;

        public FileWriter(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            _streamWriter = File.CreateText(path);
        }
        public void WriteLine(string line)
        {
            _streamWriter.WriteLine(line);
        }

        public void Dispose()
        {
            _streamWriter.Dispose();
        }
    }
}