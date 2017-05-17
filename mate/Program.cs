using System;
using System.Text.RegularExpressions;

using Mate;

namespace mate
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFile = @"test1.mate";
            string outputFile = @"test1.cpp";
            Console.WriteLine("Hello World!");

            string text = System.IO.File.ReadAllText(inputFile);

            Translator t = new Translator();

            text = t.Translate(text);

            System.IO.File.WriteAllText(outputFile, text);

        }
    }
}