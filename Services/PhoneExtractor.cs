using System.Text.RegularExpressions;

namespace WebParser.Services
{
    class PhoneExtractor
    {
        public static IEnumerable<string> Extract(string text)
        {
            List<string> phones = new List<string>();
            string phonePattern = @"(?:\+\d{1,3}[-.\s]?)?(?:\(?\d{2,3}\)?[-.\s]?)?\d{3}[-.\s]?\d{2}[-.\s]?\d{2,3}";
            Regex phoneRegex = new Regex(phonePattern, RegexOptions.Compiled);

            MatchCollection matches = phoneRegex.Matches(text);
            foreach (Match match in matches)
            {
                string cleanedPhone = Regex.Replace(match.Value, @"[\s\-().]", "");
                phones.Add(cleanedPhone);
            }
            return phones;
        }
    }
}
