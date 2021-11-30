using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FairPlayTube.Common.Helpers
{
    public static class HtmlHelper
    {
        public static string FormatHtmlText(string originalText)
        {
            //based on sample here: https://stackoverflow.com/questions/10576686/c-sharp-regex-pattern-to-extract-urls-from-given-string-not-full-html-urls-but
            var linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            string replacedText = originalText;
            foreach (Match m in linkParser.Matches(originalText))
            {
                replacedText = replacedText.Replace(m.Value, $"<a href=\"{m.Value}\" target=\"_blank\">{m.Value}</a>");
            }
            return replacedText;
        }
    }
}
