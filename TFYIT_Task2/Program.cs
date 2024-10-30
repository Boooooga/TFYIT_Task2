using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;

namespace TFYIT_Task2
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\Users\123\Documents\Универ\Теория формальных языков и трансляций\inputCode.txt";
            LexAnalyzer analyzer = new LexAnalyzer();
            List<string> result = analyzer.Analyze(path);

            string outPath = @"C:\Users\123\Documents\Универ\Теория формальных языков и трансляций\lexemesList.txt";
            using (StreamWriter sw = new StreamWriter(outPath))
            {
                foreach (string line in result)
                {
                    sw.WriteLine(line);
                }
            }
        }
    }
}