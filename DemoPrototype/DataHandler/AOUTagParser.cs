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
        // returns the first line in input. out: the rest of text
        public static string FindNextTextLine(string input, out string nextLines)
        {
            string firstTextLine = string.Empty;
            string workingString = input;
            string[] lineOfEndChars = new string[] { "\r\n", "\n" }; // New line sequences. Important! right order

            nextLines = string.Empty;
            foreach (var eol in lineOfEndChars)
            {
                int eolLineIndex = input.IndexOf(eol);
                if (eolLineIndex > 0) // index for end of line char
                {
                    firstTextLine = input.Substring(0, eolLineIndex).Trim(); // First whole text line
                    nextLines = input.Substring(input.IndexOf(eol) + eol.Length); // Rest of text
                    break; // Finished
                }
            }

            if (firstTextLine == string.Empty)
            {
                return input; // If no eol return all
            }
            else
            {
                return firstTextLine; // next line found
            }
        }

 
        // Find tag and it's content. tagEndPos is the position after the end tag
        public static bool GetTagAndContent(string text, out string tag, out string content, out int tagEndPos)
        {
            tag = "";
            content = text;
            tagEndPos = 0;

            Regex rTag = new Regex("<[a-zA-Z0-9]+>"); // Match tag with any letter or number and at least one character
            Match m = rTag.Match(text, 0);

            if (m.Success)
            {
                tag = m.Groups[0].Value.Substring(1, m.Groups[0].Value.Length - 2);
                FindTagAndExtractText(tag, text, out content, out tagEndPos);
            }
            return m.Success;
        }

        // Try to find specific tag
        public static bool FindTag(string tag, string textLine)
        {
            string startTag = "<" + tag + ">";
            string endTag = "</" + tag + ">";
            int pos1 = textLine.IndexOf(startTag);
            int pos2 = textLine.IndexOf(endTag);

            return (pos1 != -1 && pos2 != -1);
        }

        // Try to find specific tag and extract the text between the tag pair. 
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

        /*
         * Parse different types of content in tags
         */
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
                if (tag == AOUInputParser2.tagTempSubTagReturnForecasted)
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
            if (ParseLong(AOUInputParser2.tagSubTagTime, textline, out time_s_x_10))
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
            return ParseLong(AOUInputParser2.tagSubTagTime, textline, out time_ms);
        }

    }
}
