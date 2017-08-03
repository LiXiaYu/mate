using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Mate
{
    class Translator
    {
        List<Tuple<string,string>> ReplaceList { get; set; }

        List<Tuple<string,string>> NoteList { get; set; }

        List<Tuple<string, string>> StringList { get; set; }

        List<string> IntervalList { get; set; }

        public Translator()
        {
            this.ReplaceList = new List<Tuple<string, string>>();
            this.ReplaceList.Add(new Tuple<string, string>(@"\bmate\b", @"class"));
            this.ReplaceList.Add(new Tuple<string, string>(@"\bfunction\b", @"[&]"));
            this.ReplaceList.Add(new Tuple<string, string>(@"\bthis\.\b", @"this->"));
            this.ReplaceList.Add(new Tuple<string, string>(@"\bthis\.\B", @"this->"));

            this.NoteList = new List<Tuple<string, string>>();
            this.NoteList.Add(new Tuple<string, string>("/*", "*/"));
            this.NoteList.Add(new Tuple<string, string>("//", "\n"));

            this.StringList = new List<Tuple<string, string>>();
            this.StringList.Add(new Tuple<string, string>("\"", "\""));

            this.IntervalList = new List<string>();
            this.IntervalList.Add(" ");
            this.IntervalList.Add("\n");
            this.IntervalList.Add("\r\n");


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

        public string InterfaceBuild_new(List<string> texts, Func<string, bool> exception = null)
        {
            List<string> interfaceList = new List<string>();

            bool strClock = false;
            List<char> tcs = new List<char>();
            string cppt = "";
            string tss = "";
            MatchCollection m;

            List<string> result = new List<string>();
            //building
            Func<int> interfaceBuild = () => {
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
                    for (int isi = 0; isi < interS.Length; isi++)
                    {
                        if (interS[isi] == '{')
                        {
                            interS = interS.Insert(isi + 1, @"
public:");
                            break;
                        }
                    }
                    MatchCollection virtualfuncmatch = Regex.Matches(interS, @"(?<=(\n|\{)\s*)\S(.*)(?<=\)(.|\n)*;)");
                    int addlength = 0;
                    foreach (Match vresult in virtualfuncmatch)
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
                foreach (Match mint in minter)
                {
                    string s = mint.Value;
                    string[] ss = s.Split(new char[] { ' ', ',', '{', '\r', '\n' });
                    for (int si = 0; si < ss.Length; si++)
                    {
                        foreach (string inter in interfaceList)
                        {
                            if (inter == ss[si])
                            {
                                int sslength = 0;
                                for (int sil = 0; sil < si; sil++)
                                {
                                    sslength += ss[sil].Length + 1;
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
            foreach(string text in texts)
            {
                if (exception != null)
                {
                    if (exception(text))
                    {
                        result.Add(text);
                        cppt += text;
                        continue;
                    }
                }

                for (int i = 0; i < text.Length; i++)
                {
                    tcs.Add(text[i]);

                }
                interfaceBuild();
                cppt += tss;

                strClock = true;
                tcs.Clear();

                result.Add(text);

            }



            return cppt;
        }

        public string Translate_new(string text)
        {
            string result = "";

            List<string> afterNoteSplit = Translator.Split(text, this.NoteList[0].Item1, this.NoteList[0].Item2);

            for (int i = 1; i < this.NoteList.Count; i++)
            {
                afterNoteSplit = Translator.Split(afterNoteSplit, this.NoteList[i].Item1, this.NoteList[i].Item2, o =>
                {
                    for(int j=0;j<i;j++)
                    {
                        char[] begin = new char[this.NoteList[j].Item1.Length];
                        char[] end = new char[this.NoteList[j].Item2.Length];
                        for (int k=0;k<this.NoteList[j].Item1.Length&&k<o.Length;k++)
                        {
                            begin[k] = o[k];
                        }
                        for(int k=0;k<this.NoteList[j].Item2.Length&&k<o.Length;k++)
                        {
                            end[end.Length - 1 - k] = o[o.Length - 1 - k];
                        }

                        if (this.NoteList[j].Item1.Equals(new string(begin)) && this.NoteList[j].Item2.Equals(new string(end)))
                        {
                            return true;
                        }
                    }

                    return false;
                });
            }

            List<Tuple<string, string>> noteandstringList = new List<Tuple<string, string>>();
            noteandstringList.AddRange(this.NoteList);
            noteandstringList.AddRange(this.StringList);

            List<string> afterStringSplit = Translator.Split(new List<string>(afterNoteSplit.ToArray()), this.StringList[0].Item1, this.StringList[0].Item2, o =>
            {
                for (int j = 0; j < this.NoteList.Count; j++)
                {
                    char[] begin = new char[noteandstringList[j].Item1.Length];
                    char[] end = new char[noteandstringList[j].Item2.Length];
                    for (int k = 0; k < noteandstringList[j].Item1.Length && k < o.Length; k++)
                    {
                        begin[k] = o[k];
                    }
                    for (int k = 0; k < noteandstringList[j].Item2.Length && k < o.Length; k++)
                    {
                        end[end.Length - 1 - k] = o[o.Length - 1 - k];
                    }

                    if (noteandstringList[j].Item1.Equals(new string(begin)) && noteandstringList[j].Item2.Equals(new string(end)))
                    {
                        return true;
                    }
                }

                return false;
            });

            for (int i = 1; i < this.StringList.Count; i++)
            {
                afterStringSplit = Translator.Split(afterStringSplit, this.StringList[i].Item1, this.StringList[i].Item2, o =>
                {
                    for (int j = 0; j < this.NoteList.Count+i; j++)
                    {
                        char[] begin = new char[noteandstringList[j].Item1.Length];
                        char[] end = new char[noteandstringList[j].Item2.Length];
                        for (int k = 0; k < noteandstringList[j].Item1.Length && k < o.Length; k++)
                        {
                            begin[k] = o[k];
                        }
                        for (int k = 0; k < noteandstringList[j].Item2.Length && k < o.Length; k++)
                        {
                            end[end.Length - 1 - k] = o[o.Length - 1 - k];
                        }

                        if (noteandstringList[j].Item1.Equals(new string(begin)) && noteandstringList[j].Item2.Equals(new string(end)))
                        {
                            return true;
                        }
                    }

                    return false;
                });
            }

            List<string> afterReplace = Translator.Replace(afterStringSplit, this.ReplaceList, o =>
            {
                for (int j = 0; j < noteandstringList.Count; j++)
                {
                    char[] begin = new char[noteandstringList[j].Item1.Length];
                    char[] end = new char[noteandstringList[j].Item2.Length];
                    for (int k = 0; k < noteandstringList[j].Item1.Length && k < o.Length; k++)
                    {
                        begin[k] = o[k];
                    }
                    for (int k = 0; k < noteandstringList[j].Item2.Length && k < o.Length; k++)
                    {
                        end[end.Length - 1 - k] = o[o.Length - 1 - k];
                    }

                    if (noteandstringList[j].Item1.Equals(new string(begin)) && noteandstringList[j].Item2.Equals(new string(end)))
                    {
                        return true;
                    }
                }

                return false;
            });

            afterReplace.ForEach(s => result += s);

            result = this.InterfaceBuild_new(afterReplace, o =>
              {
                  for (int j = 0; j < noteandstringList.Count; j++)
                  {
                      char[] begin = new char[noteandstringList[j].Item1.Length];
                      char[] end = new char[noteandstringList[j].Item2.Length];
                      for (int k = 0; k < noteandstringList[j].Item1.Length && k < o.Length; k++)
                      {
                          begin[k] = o[k];
                      }
                      for (int k = 0; k < noteandstringList[j].Item2.Length && k < o.Length; k++)
                      {
                          end[end.Length - 1 - k] = o[o.Length - 1 - k];
                      }

                      if (noteandstringList[j].Item1.Equals(new string(begin)) && noteandstringList[j].Item2.Equals(new string(end)))
                      {
                          return true;
                      }
                  }

                  return false;
              });

            return result;
        }

        public static List<string> Split(string origin,string beginSign,string endSign)
        {
            List<string> result = new List<string>();


            char[] beginCheck_c = new char[beginSign.Length];
            char[] endCheck_c = new char[endSign.Length];

            result.Add("");

            bool ifBegin = false;
            for (int i=0;i<origin.Length;i++)
            {

                for (int j = 0; j < beginSign.Length && i + j < origin.Length; j++)
                {
                    beginCheck_c[j] = origin[i + j];
                }
                for (int j = 0; j < endSign.Length && i + j < origin.Length; j++)
                {
                    endCheck_c[j] = origin[i + j];
                }

                if(ifBegin)
                {
                    if (endSign.Equals(new string(endCheck_c)))
                    {
                        ifBegin = false;
                        for(int j=0;j<endSign.Length&&i+j<origin.Length;j++,i++)
                        {
                            result[result.Count - 1] += origin[i];
                        }
                        i--;
                        result.Add("");
                        continue;
                    }
                }
                else
                {
                    if (beginSign.Equals(new string(beginCheck_c)))
                    {
                        result.Add("");
                        ifBegin = true;
                    }
                }



                result[result.Count - 1] += origin[i];
            }



            return result;

        }

        public static List<string> Split(List<string> origins,string beginSign,string endSign,Func<string,bool> exception=null)
        {

            List<string> result = new List<string>();

            int i = -1;
            foreach(string origin in origins)
            {
                i++;
                if(exception!=null)
                {
                    if(exception(origin))
                    {
                        result.Add(origin);
                        continue;
                    }
                }
                result.AddRange(Translator.Split(origin, beginSign, endSign));              
            }

            return result;
        }

        public static string Replace(string origin,string source,string goal)
        {
            return Regex.Replace(origin, source, goal);
        }
        public static List<string> Replace(List<string> origins,string source,string goal, Func<string, bool> exception = null)
        {
            List<string> result = new List<string>(origins);
            for(int i=0;i<result.Count;i++)
            {
                if (exception != null)
                {
                    if (exception(origins[i]))
                    {
                        continue;
                    }
                }


                result[i]= Translator.Replace(origins[i], source, goal);
            }
            return result;
        }
        public static List<string> Replace(List<string> origins, List<Tuple<string, string>> lists, Func<string, bool> exception = null)
        {
            List<string> result = new List<string>(origins);

            foreach(Tuple<string,string> list in lists)
            {

                result = Translator.Replace(result, list.Item1, list.Item2,exception);
            }

            return result;
        }

    }
}
