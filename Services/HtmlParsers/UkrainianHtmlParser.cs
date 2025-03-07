using HtmlAgilityPack;
using System.Text.RegularExpressions;
using WebParser.Config;
using WebParser.Interfaces;
using WebParser.Models;
using WebParser.Services.ObjExtractors;

namespace WebParser.Services.HtmlParsers
{
    class UkrainianHtmlParser : IHtmlParser
    {
        private readonly string _coreUrl;
        public UkrainianHtmlParser(string coreUrl)
        {
            if (coreUrl.Contains(Consts.CoreUrls[Consts.WebSitesNames.Ukrainian]))
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
            List<string> result = new List<string>();

            if (string.IsNullOrEmpty(_coreUrl))
                return result;

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlSearchPage);

            // check if there is more than one page
            HtmlNode lastPage = htmlDoc.DocumentNode.SelectSingleNode("//ul[@class='pagination']/li[@class='last']/a");
            if (lastPage != null)
            {
                // there is more than one page

                int pages = 0;
                string lastPageUrl = lastPage.GetAttributeValue("href", string.Empty);

                string pattern = @"[?&]page=(\d+)";

                Match match = Regex.Match(lastPageUrl, pattern);
                if (match.Success)
                {
                    // url to the last page has been found

                    string pageValue = match.Groups[1].Value;
                    pages = Convert.ToInt32(pageValue);

                    // get url for each page
                    IEnumerable<string> urlsWithJobs = GetPagesUrls(pages);

                    // gets a urls to the vacancies from each page
                    foreach (var jobUrl in urlsWithJobs)
                    {
                        result.AddRange(await GetJobUrls(jobUrl));
                    }
                    return result;
                }
                else
                {
                    Console.WriteLine($"{DateTime.Now}\tParsing error: can't find amount of pages");
                }
            }
            else
            {
                // there is only one page

                return await GetJobUrls(_coreUrl);
            }

            return result;
        }

        public async Task<JobInfoModel> ParseHtmlPage(string htmlPage)
        {

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlPage);

            HtmlNode nodeWithData = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='details']");
            if (nodeWithData != null)
            {
                HtmlNode titleNode = nodeWithData.SelectSingleNode(".//h1");
                HtmlNode descriptionNode = nodeWithData.SelectSingleNode(".//div[@class='details__info']");

                // get details (location, price, type ...)
                var locationNode = nodeWithData.SelectNodes(".//ul[@class='details__list']/li");
                string loсation = string.Empty;
                foreach (var node in locationNode)
                {
                    if(node.SelectSingleNode(".//*[class='details__item-name']").InnerText == "City")
                    {
                        loсation = node.SelectSingleNode(".//*[@class='details__item-value'").InnerText;
                    }

                }

                string title = titleNode.InnerText;
                string description = descriptionNode.InnerText;

                IEnumerable<string> phones = PhoneExtractor.Extract(nodeWithData.InnerText);
                phones = await PhoneValidator.GetUniqueVerifiedNumbersAsync(phones);

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

            // urls may have contain page language
            // makes urls only for english language
            urls = urls.Select(u => u.Replace($"{Consts.CoreUrls[Consts.WebSitesNames.Ukrainian]}/ua", $"{Consts.CoreUrls[Consts.WebSitesNames.Ukrainian]}")).ToList();


            return urls;
        }

        private async Task<IEnumerable<string>> GetJobUrls(string searchPageUrl)
        {
            List<string> jobUrls = new();
            
            // get html page from url
            string htmlPage = await HttpHandler.GetHtmlAsync(searchPageUrl);
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlPage);

            // remove page

            // get html elements with link to vacancy
            HtmlNodeCollection jobNodes = htmlDoc.DocumentNode.SelectNodes("//*[@id=\"container\"]//div[contains(@class, \"card-work\")]/a[@href]");
            if (jobNodes != null)
            {
                // get uri link to all vacancies
                foreach (var jobNode in jobNodes)
                {
                    jobUrls.Add(jobNode.GetAttributeValue("href", string.Empty));
                }
                // clear invalid elements
                jobUrls.RemoveAll(string.IsNullOrEmpty);

                // create url from uri
                jobUrls.Select(uri => $"{Consts.CoreUrls[Consts.WebSitesNames.Ukrainian]}{uri}"
                    // in case we have '//' instead '/' after website domain
                    .Replace($"{Consts.CoreUrls[Consts.WebSitesNames.Ukrainian]}/", Consts.CoreUrls[Consts.WebSitesNames.Ukrainian]));

                return jobUrls;
            }
            else
            {
                Console.WriteLine($"\n[{DateTime.Now}]\nError: can't find elements with 'href'");
                return jobUrls;
            }

        }
    }
}
