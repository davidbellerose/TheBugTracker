using System;
using System.Text;
using System.Text.RegularExpressions;

namespace TheBugTracker.Extensions
{
    public class HtmlSubstring
    {
        public static string CleanHtml(string html, int maxlength)
        {
            //this method will correct for broken tags e.g. missing <>/
            //initialize regular expressions
            string htmltag = "</?\\w+((\\s+\\w+(\\s*=\\s*(?:\".*?\"|'.*?'|[^'\">\\s]+))?)+\\s*|\\s*)/?>";
            string emptytags = "<(\\w+)((\\s+\\w+(\\s*=\\s*(?:\".*?\"|'.*?'|[^'\">\\s]+))?)+\\s*|\\s*)/?></\\1>";

            //match all html start and end tags, otherwise get each character one by one..
            var expression = new Regex(string.Format("({0})|(.?)", htmltag));
            MatchCollection matches = expression.Matches(html);

            int i = 0;
            StringBuilder content = new StringBuilder();
            foreach (Match match in matches)
            {
                if (match.Value.Length == 1
                    && i < maxlength)
                {
                    content.Append(match.Value);
                    i++;
                }
                //the match contains a tag
                else if (match.Value.Length > 1)
                    content.Append(match.Value);
            }

            var cleanString = Regex.Replace(content.ToString(), emptytags, string.Empty);

            return cleanString;
        }

        public static string RemoveHtml(string html, int maxlength)
        {
            // this removes all the html tags
            // once they are removed, if the string is still longer than 300 characters
            // it will truncate it to 300 characters
            // otherwise it will return the stripped string
            int strLen = 0;
            var result = "";

            if (!string.IsNullOrEmpty(html))
            {
                html = Regex.Replace(html, "<.*?>", String.Empty);
                strLen = html.Length;

                if (strLen > maxlength)
                {
                    result = html.Substring(0, maxlength) + "...";
                }
                else
                {
                    result = html + "...";
                }


            }
            return result;
        }
    }
}
