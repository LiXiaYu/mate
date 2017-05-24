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

        public string InterfaceBuild(string text)
        {
            List<string> interfaceList = new List<string>();

            bool strClock = false;
            List<char> tcs = new List<char>();
            string cppt = "";
            string tss = "";
            MatchCollection m;
            
            //building
            Func<int> interfaceBuild = ()=>{
                tss = new String(tcs.ToArray());

                m = Regex.Matches(tss, @"((?<=(\binterface\s*))\S[^:|^\{|\r|^\n]*)");
                foreach (Match mresult in m)
                {
                    interfaceList.Add(mresult.Value);
                    int knum = 0;
                    int beginIndex = mresult.Index;
                    int endIndex = mresult.Index;
                    for (int si = beginIndex; si < tss.Length; si++)
                    {
                        if (tss[si] == '{')
                        {
                            knum++;
                        }
                        else if (tss[si] == '}')
                        {
                            knum--;

                            if (knum == 0)
                            {
                                endIndex = si;
                                break;
                            }
                        }
                    }

                    string interS = tss.Substring(beginIndex, endIndex - beginIndex);
                    for(int isi=0;isi<interS.Length;isi++)
                    {
                        if(interS[isi]=='{')
                        {
                            interS = interS.Insert(isi + 1, @"
public:");
                            break;
                        }
                    }
                    MatchCollection virtualfuncmatch = Regex.Matches(interS, @"(?<=(\n|\{)\s*)\S(.*)(?<=\)(.|\n)*;)");
                    int addlength = 0;
                    foreach(Match vresult in virtualfuncmatch)
                    {
                        interS = interS.Insert(vresult.Index + addlength, "virtual ");
                        addlength += "virtual ".Length;
                    }

                    interS = Regex.Replace(interS, @"\)(.|\n)*;", @")=0;");
                    tss = tss.Remove(beginIndex, endIndex - beginIndex);
                    tss = tss.Insert(beginIndex, interS);
                }

                tss = Regex.Replace(tss, @"\binterface\b", @"class");

                MatchCollection minter = Regex.Matches(tss, @"(?<=((\bclass\b[^\{(.|\n|)]*?)):)(.|\n)*?\{");
                int addlengthpublic = 0;
                foreach(Match mint in minter)
                {
                    string s=mint.Value;
                    string[] ss = s.Split(new char[] { ' ', ',', '{', '\r', '\n' });
                    for(int si=0;si<ss.Length;si++)
                    {
                        foreach (string inter in interfaceList)
                        {
                            if (inter == ss[si])
                            {
                                int sslength = 0;
                                for(int sil=0;sil<si;sil++)
                                {
                                    sslength += ss[sil].Length+1;
                                }
                                tss = tss.Insert(mint.Index + sslength + addlengthpublic, "public ");
                                addlengthpublic += "public ".Length;
                                break;
                            }
                        }
                    }
                }

                return 0;
            };

            for (int i = 0; i < text.Length; i++)
            {
                tcs.Add(text[i]);
                //String Exception Principle
                if (text[i] == '\"')
                {
                    if (strClock == false)
                    {
                        tcs.RemoveAt(tcs.Count - 1);
                        interfaceBuild();
                        cppt += tss + "\"";

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
            interfaceBuild();
            cppt += tss;

            strClock = true;
            tcs.Clear();

            return cppt;
        }
    }
}
