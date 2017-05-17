using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Mate
{
    class Translator
    {
        List<Tuple<string,string>> ReplaceList { get; set; }

        public Translator()
        {
            this.ReplaceList = new List<Tuple<string, string>>();
            this.ReplaceList.Add(new Tuple<string, string>(@"\bmate\b", @"class"));
            this.ReplaceList.Add(new Tuple<string, string>(@"\bfunction\b", @"[&]"));
            this.ReplaceList.Add(new Tuple<string, string>(@"\bthis\.\b", @"this->"));
            this.ReplaceList.Add(new Tuple<string, string>(@"\bthis\.\B", @"this->"));
        }

        public string Translate(string text)
        {
            string cppt = text;
            foreach(var r in this.ReplaceList)
            {
                cppt= Regex.Replace(cppt, r.Item1, r.Item2);
            }

            return cppt;
        }
    }
}
