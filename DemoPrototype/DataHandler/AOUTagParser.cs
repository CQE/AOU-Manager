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
            nextLines = string.Empty;

            // var tst = Encoding.ASCII.GetBytes(input);

            int lfIndex = input.IndexOf((char)10);
            if (lfIndex > 0) // index for end of line char
            {
                // firstTextLine = input.Substring(0, lfIndex).TrimEnd(new char[13]); // Get first whole text line. delete return char if in the end
                firstTextLine = input.Substring(0, lfIndex).Trim(); // Get first whole text line. delete return char if in the end
                if (input.Length > (lfIndex + 1))
                {
                    nextLines = input.Substring(lfIndex + 1); // Get rest of the text
                }
            }

            if (firstTextLine == string.Empty)
            {
                return input.Trim(); ; // If no lf return all
            }
            else
            {
                return firstTextLine; // next line found
            }
        }


        public static string FindNextTextLine2(string input, out string nextLines)
        {
            int lfIndex = -1;

            nextLines = string.Empty;

            if (input != null && input != string.Empty && input.Length > 0 &&
                (lfIndex = input.IndexOf((char)10)) >= 0)  // Index for end of line char ("\n" or LF)
            {
                // Take the input that is beyond the end of line, and submit it for further processing
                if (input.Length > (lfIndex + 1))
                {
                    nextLines = input.Substring(lfIndex + 1); // Get rest of the text
                }
                // Return the whole text line except for the end of line char at the end
                return (input.Substring(0, lfIndex).Trim());
            }
            else
            {
                // If no end of line, return nothing and submit all for further processing
                if (input != null && input != string.Empty && input.Length > 0 && lfIndex < 0)
                    nextLines = input;

                // Return nothing
                 return (null);
             //   return string.Empty; //tetar detta MW
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

        public static bool ParseMMMMSSSS(string tag, string textline, out UInt32 mmmmssss)
        {
            int endpos = 0;
            string tagValue = "";

            if (FindTagAndExtractText(tag, textline, out tagValue, out endpos) && tagValue.Length == 8)
            {
                if (UInt32.TryParse(tagValue, System.Globalization.NumberStyles.HexNumber, null, out mmmmssss))
                {
                    return true;
                }
            }
            mmmmssss = 0;
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

        public static bool ParseUInt(string tagText, string textline, out uint value)
        {
            int endpos = 0;
            if (FindTagAndExtractText(tagText, textline, out tagText, out endpos) &&
                uint.TryParse(tagText, out value))
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
            long time_ds = 0;
            time_ms = 0;
            if (ParseLong(AOUInputParser2.tagSubTagTime, textline, out time_ds))
            {
                time_ms = time_ds * 100;
                return true;
            }
            return false;
        }

    }
}
