using WebParser.Interfaces;
using WebParser.Models;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Buffers.Text;

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
                Console.WriteLine($"Wrong url address. Parser can not perform its task when working on '{coreUrl}'");
                Environment.Exit(1);
            }
        }

        public async Task<IEnumerable<string>> GetJobsUrls(string htmlSearchPage)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlSearchPage);
            

            //Get pages count
            HtmlNodeCollection nodesWithPageCounts = htmlDoc.DocumentNode.SelectNodes("//ul[@class='pagination']/li/a");
            if(nodesWithPageCounts != null)
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
                        result.AddRange(await GetJobsUrls(jobUrl));
                    }
                    return result;
                }
                else
                {
                    Console.WriteLine("Parsing error: can't find amount of pages");
                }
            }
            else
            {
                return await GetJobUrls(_coreUrl);
            }
            return null;
        }

        public async Task<JobInfoModel> ParseUrl(string htmlPage)
        {


            return null;
        }

        private IEnumerable<string> GetPagesUrls(int pagesCount)
        {
            List<string> urls = new List<string>();

            // Регулярний вираз для видалення параметру 'page'
            string pattern = @"([?&])page=\d+";
            string cleanedUrl = Regex.Replace(_coreUrl, pattern, "");

            // Видалення зайвого '&' чи '?' в кінці
            cleanedUrl = cleanedUrl.TrimEnd('&').TrimEnd('?');

            for (int i = 1; i <= pagesCount; i++)
            {
                string separator = cleanedUrl.Contains("?") ? "&" : "?";
                string newUrl = $"{cleanedUrl}{separator}page={i}";
                urls.Add(newUrl);
            }

            return urls;
        }
        private async Task< IEnumerable<string>> GetJobUrls(string searchPageUrl)
        {
            List<string> jobUrls = new();
            string htmlPage = await HtmlLoader.GetHtmlAsync(searchPageUrl);
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlPage);

            HtmlNodeCollection jobNodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='vacancies-list-item']/a[@href]");
            if(jobNodes != null)
            {
                foreach (var node in jobNodes)
                {
                    jobUrls.Add(node.GetAttributeValue("href", string.Empty));
                }
                return jobUrls;
            }
            else
            {
                Console.WriteLine("Error: can't find elements with 'href'");
                return null;
            }
            
        }
    }
}
