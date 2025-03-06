using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Text;
using WebParser.Config;
using WebParser.Interfaces;
using WebParser.Models;

namespace WebParser.Services.Filters
{
    class PhoneFilter : IJobFilter
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
                Filter = typeof(PhoneFilter),
                Message = "Filter excludes phone numbers of vacancy, that is present in API db or has wrong format.\n" +
                "Also vacancies that after filtration had no phone numbers are excluded.",
                FilerItemsCount = null,
                IncomeItemsCount = vacancies.Count(),
                OutcomeItemsCount = passeedModels.Count(),
            };

            return (stats, passeedModels);
        }

        private bool CheckNumber(HtmlDocument htmlDoc, string num)
        {
            var jsonData = new JObject { ["value"] = num };
            var content = new StringContent(jsonData.ToString(), Encoding.UTF8, "application/json");
            var postResult = Task.Run(async () => await HttpHandler.GetFormResultAsync(Consts.PhoneCheckerApiUrl + "check_number", content)).Result;
           
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
