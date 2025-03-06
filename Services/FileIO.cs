using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using WebParser.Models;

namespace WebParser.Services
{
    class FileIO
    {
        public static async Task SaveResults(IEnumerable<JobInfoModel> models, IEnumerable<string> jsonPaths, IEnumerable<string> csvPaths)
        {
            string jsonModels = JsonConvert.SerializeObject(models, Formatting.Indented);
            Console.WriteLine($"{DateTime.Now}\tWriting data into json files...");
            var jsonTasks = jsonPaths.Select(path => SaveJsonToFileAsync(path, jsonModels));
            await Task.WhenAll(jsonTasks);

            Console.WriteLine($"{DateTime.Now}\tWriting data into csv files...");
            var csvTasks = csvPaths.Select(path => SaveToCsvAsync(path, models));
            await Task.WhenAll(csvTasks);
        }

        public static async Task SaveJsonToFileAsync(string filePath, string jsonContent)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    await writer.WriteAsync(jsonContent);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{DateTime.Now}\tError when writing data into file: {filePath}");
                Console.WriteLine($"Exception message:\t{e.Message}");
            }
        }
        public static async Task SaveToCsvAsync(string filePath, IEnumerable<JobInfoModel> records)
        {
            try
            {
                using (var writer = new StreamWriter(filePath))
                using (var csv = new CsvWriter(writer,
                    new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        Delimiter = ","
                    }))
                {
                    await csv.WriteRecordsAsync(records);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{DateTime.Now}\tError when writing data into file: {filePath}");
                Console.WriteLine($"Exception message:\t{e.Message}");
            }
        }
        public static async Task<IEnumerable<string>> ReadAllLinesFromFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    return await File.ReadAllLinesAsync(filePath);
                }
                else
                {
                    Console.WriteLine($"{DateTime.Now}\tError:\tFile not found: {filePath}");
                    return [];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{DateTime.Now}\tError reading file:\t{filePath}\n{e.Message}");
                return [];
            }
        }
        public static async Task<string> ReadAllTextFromFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    return await File.ReadAllTextAsync(filePath);
                }
                else
                {
                    Console.WriteLine($"{DateTime.Now}\tError:\tFile not found: {filePath}");
                    return string.Empty;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{DateTime.Now}\tError reading file:\t{filePath}\n{e.Message}");
                return string.Empty;
            }
        }

        public static bool IsContentJSON(string content)
        {
            try
            {
                JToken.Parse(content);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

