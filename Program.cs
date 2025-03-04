using WebParser.Models;
using WebParser.Services;

namespace WebParser
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Program started...");
            //using (StreamWriter logFile = new(Consts.LogFilePath, true))
            //{
            //    TeeTextWriter teeWriter = new(Console.Out, logFile);
            //    Console.SetOut(teeWriter);
            //    Console.SetError(teeWriter);
            //}

            string url_to_parse = @"https://amountwork.com/ua/rabota/ssha/voditel";
            string html = await HtmlLoader.GetHtmlAsync(url_to_parse);
            AmountworkHtmlParser amountworkParser = new AmountworkHtmlParser(url_to_parse);
            var jobUrls = await amountworkParser.GetJobsUrls(html);
            List<JobInfoModel> models = new();
            foreach (var url in jobUrls)
            {
                models.Add(await amountworkParser.ParseUrl(url));
            }
            Console.WriteLine("Press any key to close the program");
            Console.ReadLine();
        }
    }
}
