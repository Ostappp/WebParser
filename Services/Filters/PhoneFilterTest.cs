using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json.Serialization;
using WebParser.Config;
using WebParser.Interfaces;
using WebParser.Models;

namespace WebParser.Services.Filters
{
    class PhoneFilterTest : IJobFilter
    {
        public async Task<(FiltrationStatsModel stats, IEnumerable<JobInfoModel> passeedModels)> ApplyFiltration(IEnumerable<JobInfoModel> vacancies)
        {
            HtmlDocument htmlDoc = new();
            var passeedModels = vacancies.Select(v =>
            {
                var clone = (JobInfoModel)v.Clone();
                clone.Phones = clone.Phones.Where(num => CheckNumber(htmlDoc, num));
                return clone;
            }).Where(v => v.Phones.Any());

            var stats = new FiltrationStatsModel()
            {
                Filter = typeof(PhoneFilterTest),
                Message = "SImulation of:\nFilter excludes phone numbers of vacancy, that is present in API db or has wrong format.\n" +
                "Also vacancies that after filtration had no phone numbers are excluded.",
                FilerItemsCount = null,
                IncomeItemsCount = vacancies.Count(),
                OutcomeItemsCount = passeedModels.Count(),
            };

            return (stats, passeedModels);
        }
        
        public class Response
        {
            [JsonPropertyName("value")]
            public string Value { get; set; }
        }
        private bool CheckNumber(HtmlDocument htmlDoc, string num)
        {

            string[] list = [
                "{\"value\": \"<span style=\\\"color: green;\\\">Не знайдено</span>\"\n}",
                "{\"value\": \"<span style=\\\"color: red;\\\">Неправильний формат</span>\"\n}",
                "{\"value\": \"<span style=\\\"color: red;\\\">Знайдено</span>\"\n}",
            ];
            var postResult = list[Random.Shared.Next(0, 2)];

            postResult = JObject.Parse(postResult)["value"]?.ToString();

            htmlDoc.LoadHtml(postResult);
            var spanElement = htmlDoc.DocumentNode.SelectSingleNode("//span");

            if (spanElement != null)
            {
                var style = spanElement.GetAttributeValue("style", string.Empty);
                if (style.Contains("color: green"))
                {
                    return true;
                }
            }

            return false;
        }



    }
}
