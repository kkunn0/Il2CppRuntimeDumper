using System.IO;
using Il2CppApiAnalyzer.Parsers;

namespace Il2CppApiAnalyzer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using var file = File.OpenText(Path.Combine("Test", "test_api.h"));
            using var analyzer = new Il2CppApiHeaderParser(file);
            analyzer.Parse();
        }
    }
}