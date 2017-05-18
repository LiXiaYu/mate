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
            bool strClock = false;
            List<char> tcs = new List<char>();
            string cppt = "";
            string tss = "";
            for (int i=0;i<text.Length;i++)
            {
                tcs.Add(text[i]);
                //String Exception Principle
                if (text[i]=='\"')
                {
                    if(strClock==false)
                    {
                        tcs.RemoveAt(tcs.Count - 1);
                        tss = new String(tcs.ToArray());
                        foreach (var r in this.ReplaceList)
                        {
                            tss = Regex.Replace(tss, r.Item1, r.Item2);
                        }
                        cppt += tss+"\"";

                        strClock = true;
                        tcs.Clear();
                    }
                    else
                    {
                        tss = new String(tcs.ToArray());
                        cppt += tss;

                        strClock = false;
                        tcs.Clear();
                    }
                }
            }
            tss = new String(tcs.ToArray());
            foreach (var r in this.ReplaceList)
            {
                tss = Regex.Replace(tss, r.Item1, r.Item2);
            }
            cppt += tss;

            strClock = true;
            tcs.Clear();

            return cppt;
        }
    }
}
