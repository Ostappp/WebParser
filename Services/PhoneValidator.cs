using PhoneNumbers;
using System.Collections.Concurrent;
namespace WebParser.Services
{
    public static class PhoneValidator
    {
        public static async Task<string> VerifyNumberAsync(string number)
        {
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();

            // check if number is written with country code ('+<code>')
            var num = phoneNumberUtil.ParseAndKeepRawInput(number, null);
            var regionCode = phoneNumberUtil.GetRegionCodeForNumber(num);

            if (regionCode == null)
            {
                var result = new ConcurrentBag<string>();

                await Parallel.ForEachAsync(Consts.CountryAlpha2Code, async (rCode, token) =>
                {
                    var parsedNum = phoneNumberUtil.ParseAndKeepRawInput(number, rCode);
                    var parsedRegionCode = phoneNumberUtil.GetRegionCodeForNumber(parsedNum);
                    if (parsedRegionCode != null)
                    {
                        result.Add(phoneNumberUtil.Format(parsedNum, PhoneNumberFormat.E164));
                        token.ThrowIfCancellationRequested(); //Stops parallel processing at the first match
                    }
                });

                if (!result.IsEmpty)
                {
                    return result.First();
                }
            }
            else
            {
                return phoneNumberUtil.Format(num, PhoneNumberFormat.E164);
            }

            return null;
        }

        public static async Task<IEnumerable<string>> VerifyNumbersAsync(IEnumerable<string> numbers)
        {
            var result = new ConcurrentBag<string>();

            await Parallel.ForEachAsync(numbers, async (number, token) =>
            {
                var verifiedNumber = await VerifyNumberAsync(number);
                if (!string.IsNullOrEmpty(verifiedNumber))
                {
                    result.Add(verifiedNumber);
                }
            });
            return result;
        }

        public static async Task<IEnumerable<string>> GetUniqueVerifiedNumbersAsync(IEnumerable<string> numbers)
        {
            return new HashSet<string>(await VerifyNumbersAsync(numbers));
        }
    }
}
