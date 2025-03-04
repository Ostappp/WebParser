﻿using Newtonsoft.Json;
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

                string html = await HtmlLoader.GetHtmlAsync(url_to_parse);
                AmountworkHtmlParser amountworkParser = new AmountworkHtmlParser(url_to_parse);
                var jobUrls = await amountworkParser.GetJobsUrls(html);
                List<JobInfoModel> models = new();
                foreach (var url in jobUrls)
                {
                    models.Add(await amountworkParser.ParseUrl(url));
                }

                // Серіалізація масиву в JSON
                string jsonModels = JsonConvert.SerializeObject(models, Formatting.Indented);

                await StringObjectSaver.SaveJsonToFileAsync(Consts.JsonVacanciesPath, jsonModels);
                await StringObjectSaver.SaveToCsvAsync(Consts.CsvVacanciesPath, models);

                Console.WriteLine("Press any key to close the program");
            }

        }
    }
}
