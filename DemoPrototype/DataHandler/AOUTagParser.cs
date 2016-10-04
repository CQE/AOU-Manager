using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace DemoPrototype
{

    public class AOUTagParser
    {
        public static string FindNextTextLine()
        {
            return "";
        }

        public static string GetNextTag(string text, out long time_ms, out string tagContent, out List<string> logs, out int numHandled)
        {
            logs = new List<string>();
            tagContent = "";
            numHandled = 0;

            int lastTextPos = 0;
            bool eot = false;
            string tag = "";
            int tlen = text.Length;
            string textLine = "";
            do
            {
                tag = "";
                textLine = "";
                if (text.IndexOf("\r\n") > 0)
                {
                    int endPos = text.IndexOf("\r\n", lastTextPos + 1);
                    if (endPos >= 0)
                    {
                        textLine = text.Substring(lastTextPos, endPos - lastTextPos).Trim();
                        lastTextPos = endPos + 1;
                    }
                    else
                    {
                        lastTextPos = lastTextPos + 2;
                    }
                }
                /* If only LF*/
                else if (text.IndexOf("\n") > 0)
                {
                    if ((lastTextPos + 1) < tlen)
                    {
                        int endPos = text.IndexOf("\n", lastTextPos + 1);
                        if (endPos >= 0)
                        {
                            textLine = text.Substring(lastTextPos, endPos - lastTextPos).Trim();
                            lastTextPos = endPos + 1;
                        }
                        else
                        {
                            lastTextPos = lastTextPos + 1;
                        }
                    }
                    else
                    {
                        int err = lastTextPos;
                    }
                }

                if (!eot && textLine.Length > 0)
                {
                    int tagEndPos;
                    if (GetTagAndContent(textLine, out tag, out tagContent, out tagEndPos))
                    {
                        int tagStart = textLine.IndexOf(tag);
                        if (tagStart > 1)
                        {
                            logs.Add(textLine); // Text before tag
                        }
                        else if (tagEndPos < textLine.Length)
                        {
                            // Todo: text after tag pair
                        }
                        break; // Found tag and it´s content
                    }
                    else
                    {
                        logs.Add(textLine);
                        textLine = "";
                    }
                }
            } while (tag == "" && textLine.Length > 0);
            numHandled = lastTextPos;

            long time = 0;
            if (ParseLong(AOUInputParser.tagSubTagTime, textLine, out time))
            {
               // tagContent = tagContent.Substring(tagContent.IndexOf("</Time>") + 7); // Handled
            }
            time_ms = time * 100; // Transform deciseconds to milliseconds

            return tag;
        }

        public static bool GetTagAndContent(string text, out string tag, out string content, out int tagEndPos)
        {
            tag = "";
            content = text;
            tagEndPos = 0;

            Regex rTag = new Regex("<[a-zA-Z]+>");
            Match m = rTag.Match(text, 0);

            if (m.Success)
            {
                tag = m.Groups[0].Value.Substring(1, m.Groups[0].Value.Length - 2);
                FindTagAndExtractText(tag, text, out content, out tagEndPos);
                return true;
            }
            return false;
        }

        public static bool FindTag(string tag, string textLine)
        {
            string startTag = "<" + tag + ">";
            string endTag = "</" + tag + ">";
            int pos1 = textLine.IndexOf(startTag);
            int pos2 = textLine.IndexOf(endTag);

            return (pos1 != -1 && pos2 != -1);
        }

        public static bool FindTagAndExtractText(string tag, string textLine, out string tagText, out int endPos)
        {
            string startTag = "<" + tag + ">";
            string endTag = "</" + tag + ">";
            int pos1 = textLine.IndexOf(startTag);
            int pos2 = textLine.IndexOf(endTag);
            if (pos1 != -1 && pos2 != -1)
            {
                pos1 += startTag.Length;
                tagText = textLine.Substring(pos1, pos2 - pos1);
                endPos = pos2 + endTag.Length;
                tagText = tagText.Trim();
                return true;
            }
            else
            {
                tagText = "";
                endPos = 0;
                return false;
            }

        }

        public static bool ParseString(string tagText, string textline, out string text)
        {
            int endpos = 0;
            return FindTagAndExtractText(tagText, textline, out text, out endpos);
        }

        public static bool ParseWord(string tag, string textline, out UInt16 value)
        {
            double dbl = double.NaN;
            if (Parsedouble(tag, textline, out dbl))
            {
                value = (UInt16)Math.Round(dbl);
                return true;
            }
            value = UInt16.MaxValue;
            return false;
        }

        public static bool ParseWord(string tag, string textline, out Int16 value)
        {
            double dbl = double.NaN;
            if (Parsedouble(tag, textline, out dbl))
            {
                value = (Int16)Math.Round(dbl);
                if (tag == AOUInputParser.tagTempSubTagReturnForecasted)
                {
                    string s = textline;
                }
                return true;
            }
            value = Int16.MaxValue;
            return false;
        }

        public static bool ParseMMSS(string tag, string textline, out UInt16 mmss)
        {
            int endpos = 0;
            string tagValue = "";
 
            if (FindTagAndExtractText(tag, textline, out tagValue, out endpos) && tagValue.Length == 4)
            {
                if (UInt16.TryParse(tagValue, System.Globalization.NumberStyles.HexNumber, null, out mmss))
                {
                    return true;
                }
            }
            mmss = 0;
            return false;
        }

        public static bool Parsedouble(string tag, string textline, out double value)
        {
            int endpos = 0;
            string tagValue = "";

            if (FindTagAndExtractText(tag, textline, out tagValue, out endpos))
            {
                tagValue.Replace(',', '.');
                return double.TryParse(tagValue, out value);
            }
            value = 0;
            return false;
        }

        public static bool ParseLong(string tagText, string textline, out long value)
        {
            int endpos = 0;
            if (FindTagAndExtractText(tagText, textline, out tagText, out endpos) &&
                long.TryParse(tagText, out value))
            {
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }


        public static bool ParseWordTime_sek_x_10(string textline, out UInt16 time_hours, out UInt16 time_sek_x_10)
        {
            long time_s_x_10 = 0;
            if (ParseLong(AOUInputParser.tagSubTagTime, textline, out time_s_x_10))
            {
                AOUDataTypes.Time_ms_to_AOUModelTimeSecX10(time_s_x_10 * 100, out time_hours, out time_sek_x_10);
                return true;
            }
            else
            {
                time_sek_x_10 = 0;
                time_hours = 0;
                return false;
            }
        }

        public static bool ParseLongTime(string textline, out long time_ms) // Not to be misunderstood
        {
            return ParseLong(AOUInputParser.tagSubTagTime, textline, out time_ms);
        }

    }
}
