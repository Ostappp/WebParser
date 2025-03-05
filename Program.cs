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
            List<string> blackList = new List<string>() { "Водитель" };
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
                models.RemoveAll(m => m == null); // removes empty models (nulls)
                Console.WriteLine($"{DateTime.Now}\tCreated {models.Count} objects");
                
                Console.WriteLine($"{DateTime.Now}\tApplying filtration...");
                var filtration = await (new BlackListFilter(blackList)).ApplyFiltration(models);
                Console.WriteLine($"{DateTime.Now}\tFiltration completed. Filtration statistics:\n{filtration.stats}");

                // Серіалізація масиву в JSON
                string jsonModels = JsonConvert.SerializeObject(filtration.passeedModels, Formatting.Indented);

                Console.WriteLine($"{DateTime.Now}\tWriting data into json file...");
                await StringObjectSaver.SaveJsonToFileAsync(Consts.JsonVacanciesPath, jsonModels);
                Console.WriteLine($"{DateTime.Now}\tWriting data into csv file...");
                await StringObjectSaver.SaveToCsvAsync(Consts.CsvVacanciesPath, filtration.passeedModels);

                Console.WriteLine($"{DateTime.Now}\tParsing completed. Results stored in {Consts.JsonVacanciesPath} and {Consts.CsvVacanciesPath}");
            }

        }
    }
}
