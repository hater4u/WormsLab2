using WormsLab.Interfaces;

namespace WormsLab.Writers;

public class ConsoleWriter: IWriter
{
    public void WriteLine(string line)
    {
        Console.WriteLine(line);
    }
}