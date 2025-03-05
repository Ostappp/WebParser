using Newtonsoft.Json;
using WebParser.Models;
using WebParser.Services;

namespace WebParser
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Program started...");
            string url_to_parse = @"https://amountwork.com/ua/rabota/ssha/voditel";
            using (StreamWriter logFile = new(Consts.LogFilePath, true))
            {
                TeeTextWriter teeWriter = new(Console.Out, logFile);
                Console.SetOut(teeWriter);
                Console.SetError(teeWriter);

                Console.WriteLine($"{DateTime.Now}\tGetting data from {url_to_parse}...");
                string htmlPage = await HtmlLoader.GetHtmlAsync(url_to_parse);

                AmountworkHtmlParser amountworkParser = new AmountworkHtmlParser(url_to_parse);
                var jobUrls = await amountworkParser.GetJobsUrls(htmlPage);
                Console.WriteLine($"{DateTime.Now}\tReceived {jobUrls.Count()} urls");

                List<JobInfoModel> models = new();
                foreach (var url in jobUrls)
                {
                    htmlPage = await HtmlLoader.GetHtmlAsync(url);
                    models.Add(await amountworkParser.ParseHtmlPage(htmlPage));
                }

                Console.WriteLine($"{DateTime.Now}\tCreated {models.Count} objects");

                // Серіалізація масиву в JSON
                string jsonModels = JsonConvert.SerializeObject(models, Formatting.Indented);

                Console.WriteLine($"{DateTime.Now}\tWriting data into json file...");
                await StringObjectSaver.SaveJsonToFileAsync(Consts.JsonVacanciesPath, jsonModels);
                Console.WriteLine($"{DateTime.Now}\tWriting data into csv file...");
                await StringObjectSaver.SaveToCsvAsync(Consts.CsvVacanciesPath, models);

                Console.WriteLine($"{DateTime.Now}\tParsing completed. Results stored in {Consts.JsonVacanciesPath} and {Consts.CsvVacanciesPath}");
            }

        }
    }
}
