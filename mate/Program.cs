using System;
using System.Text.RegularExpressions;

using Mate;
using System.Collections.Generic;

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
            
            List<string> ts = Translator.Split(text, "/*", "*/");
            List<string> tss = Translator.Split(text, "//", "\n");



            Translator t = new Translator();

            text=t.Translate_new(text);

            //text = t.Translate(text);
            //text = t.InterfaceBuild(text);



            System.IO.File.WriteAllText(outputFile, text);


        }
    }
}