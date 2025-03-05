using WebParser.Interfaces;
using WebParser.Models;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace WebParser.Services
{
    class AmountworkHtmlParser : IHtmlParser
    {
        private readonly string _coreUrl;
        public AmountworkHtmlParser(string coreUrl)
        {
            if (coreUrl.Contains(Consts.CoreUrls[Consts.WebSitesNames.Amountwork]))
            {
                _coreUrl = coreUrl;
            }
            else
            {
                Console.WriteLine($"\n[{DateTime.Now}]\nWrong url address. Parser can not perform its task when working on '{coreUrl}'");
                return;
            }
        }

        public async Task<IEnumerable<string>> GetJobsUrls(string htmlSearchPage)
        {
            if (string.IsNullOrEmpty(_coreUrl))
                return null;

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlSearchPage);


            //Get pages count
            HtmlNodeCollection nodesWithPageCounts = htmlDoc.DocumentNode.SelectNodes("//ul[@class='pagination']/li/a");
            if (nodesWithPageCounts != null)
            {
                int pages = 0;
                HtmlNode secondLastItem = nodesWithPageCounts[nodesWithPageCounts.Count - 2];
                string lastPageUrl = secondLastItem.GetAttributeValue("href", string.Empty);

                string pattern = @"[?&]page=(\d+)";

                Match match = Regex.Match(lastPageUrl, pattern);
                if (match.Success)
                {
                    string pageValue = match.Groups[1].Value;
                    pages = Convert.ToInt32(pageValue);

                    IEnumerable<string> urlsWithJobs = GetPagesUrls(pages);

                    List<string> result = new();
                    foreach (var jobUrl in urlsWithJobs)
                    {
                        result.AddRange(await GetJobUrls(jobUrl));
                    }
                    return result.Select(url => $"{Consts.CoreUrls[Consts.WebSitesNames.Amountwork]}{url}");
                }
                else
                {
                    Console.WriteLine($"\n[{DateTime.Now}]\nParsing error: can't find amount of pages");
                }
            }
            else
            {
                return (await GetJobUrls(_coreUrl)).Select(url => $"{Consts.CoreUrls[Consts.WebSitesNames.Amountwork]}{url}");
            }
            return null;
        }

        public async Task<JobInfoModel> ParseHtmlPage(string htmlPage)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlPage);

            //Get vacancy data
            HtmlNode nodeWithData = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='vacancy-container']");
            if (nodeWithData != null)
            {
                HtmlNode titleNode = nodeWithData.SelectSingleNode(".//h1[contains(@class, 'h1') and contains(@class,'h1-vacancy')]");
                HtmlNode descriptionNode = nodeWithData.SelectSingleNode(".//div[@class='vacancy-description']");
                HtmlNode locatioтNode = nodeWithData.SelectSingleNode(".//div[@class='company-info']/div[@class='company-info-country']/span[@class='second']");

                string title = titleNode.InnerText;
                string description = descriptionNode.InnerText;
                string loсation = locatioтNode.InnerText;

                IEnumerable<string> phones = PhoneExtractor.Extract(nodeWithData.InnerText);
                IEnumerable<string> emails = EmailExtractor.Extract(nodeWithData.InnerText);

                JobInfoModel resultModel = new()
                {
                    Title = title,
                    Description = description,
                    Location = loсation,
                    Phones = phones,
                    Emails = emails
                };

                return resultModel;
            }
            else
            {
                Console.WriteLine($"\n[{DateTime.Now}]\nParsing error: can't find vacancy data");
            }

            return null;
        }

        private IEnumerable<string> GetPagesUrls(int pagesCount)
        {
            List<string> urls = new List<string>();

            string pattern = @"([?&])page=\d+";
            string cleanedUrl = Regex.Replace(_coreUrl, pattern, "");

            cleanedUrl = cleanedUrl.TrimEnd('&').TrimEnd('?');

            for (int i = 1; i <= pagesCount; i++)
            {
                string separator = cleanedUrl.Contains("?") ? "&" : "?";
                string newUrl = $"{cleanedUrl}/{separator}page={i}";
                urls.Add(newUrl);
            }

            return urls;
        }

        private async Task<IEnumerable<string>> GetJobUrls(string searchPageUrl)
        {
            List<string> jobUrls = new();
            string htmlPage = await HttpHandler.GetHtmlAsync(searchPageUrl);
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlPage);

            HtmlNodeCollection jobNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'vacancies-list')]/div[contains(@class,'vacancies-list-item')]/h3[contains(@class,'vacancies-list-name')]/a[@href]");
            if (jobNodes != null)
            {
                foreach (var jobNode in jobNodes)
                {
                    jobUrls.Add(jobNode.GetAttributeValue("href", string.Empty));
                }

                return jobUrls;
            }
            else
            {
                Console.WriteLine($"\n[{DateTime.Now}]\nError: can't find elements with 'href'");
                return null;
            }

        }
    }
}
