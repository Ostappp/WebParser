using PhoneNumbers;
using System.Text.RegularExpressions;

namespace WebParser.Services.ObjExtractors
{
    class PhoneExtractor
    {
        public static IEnumerable<string> Extract(string text)
        {
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();
            return phoneNumberUtil.FindNumbers(text, "US").Select(num=> phoneNumberUtil.Format(num.Number, PhoneNumberFormat.E164));
            


            //HashSet<string> phones = new HashSet<string>();
            //string phonePattern = @"(?<!\d)(?:\+?\d{1,3}[-.\s]?)?(?:\(?\d{2,3}\)?[-.\s]?)?\d{3}[-.\s]?\d{2}[-.\s]?\d{2,4}(?!\d)";
            //Regex phoneRegex = new Regex(phonePattern, RegexOptions.Compiled);

            //MatchCollection matches = phoneRegex.Matches(text);
            //foreach (Match match in matches)
            //{
            //    string cleanedPhone = Regex.Replace(match.Value, @"[\s\-().]", "");
            //    if (cleanedPhone.Length >= 7 && cleanedPhone.Length <= 15) 
            //    {
            //        phones.Add(cleanedPhone);
            //    }
            //}
            //return phones;
        }

    }
}
