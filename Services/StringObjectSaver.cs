using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using WebParser.Models;

namespace WebParser.Services
{
    class StringObjectSaver
    {
        public static async Task SaveJsonToFileAsync(string filePath, string jsonContent)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                await writer.WriteAsync(jsonContent);
            }
        }
        public static async Task SaveToCsvAsync(string filePath, IEnumerable<JobInfoModel> records)
        {
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ","
            }))
            {
                await csv.WriteRecordsAsync(records);
            }
        }
    }
}

