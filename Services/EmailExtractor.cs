using System.Text.RegularExpressions;

namespace WebParser.Services
{
    class EmailExtractor
    {
        public static IEnumerable<string> Extract(string text)
        {
            HashSet<string> emails = new HashSet<string>();
            string emailPattern = @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}";
            Regex emailRegex = new Regex(emailPattern, RegexOptions.Compiled);

            MatchCollection matches = emailRegex.Matches(text);
            foreach (Match match in matches)
            {
                emails.Add(match.Value);
            }
            return emails;
        }
    }
}
