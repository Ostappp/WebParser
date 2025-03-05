using Newtonsoft.Json;
using WebParser.Interfaces;
using WebParser.Models;
using WebParser.Parsers;
using WebParser.Services;
using WebParser.Services.Filters;

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

                var filters = new List<IJobFilter>()
                {
                    { new BlackListFilter(blackList) },
                    //{ new PhoneFilter()},
                };
                var models = new List<JobInfoModel>();

                var amountworkParser = new AmountworkParser(filters, url_to_parse);
                models.AddRange(await amountworkParser.ParseAsync());

               
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
