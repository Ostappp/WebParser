using PhoneNumbers;
using System.Collections.Concurrent;
using WebParser.Config;
namespace WebParser.Services
{
    public static class PhoneValidator
    {
        public static async Task<string> VerifyNumberAsync(string number)
        {
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();
            var result = new ConcurrentBag<string>();
            try
            {
                await Parallel.ForEachAsync(Configuration.GetAlpha2Codes, async (rCode, token) =>
                {
                    var parsedNum = phoneNumberUtil.ParseAndKeepRawInput(number, rCode);
                    var parsedRegionCode = phoneNumberUtil.GetRegionCodeForNumber(parsedNum);
                    if (parsedRegionCode != null)
                    {
                        result.Add(phoneNumberUtil.Format(parsedNum, PhoneNumberFormat.E164));
                        token.ThrowIfCancellationRequested(); //Stops parallel processing at the first match
                    }
                });
            }
            catch (Exception)
            {
                // ignore exception when number does not matches region code
            }
            if (!result.IsEmpty)
            {
                return result.First();
            }

            return string.Empty;


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
