using System.Collections.Concurrent;
using WebParser.Config;
using WebParser.Interfaces;
using WebParser.Models;
using WebParser.Services;
using WebParser.Services.HtmlParsers;

namespace WebParser.Parsers
{
    class AmountworkParser: IParser
    {
        private readonly IEnumerable<IJobFilter> _filters;
        private static readonly Consts.WebSitesNames _tergetSite = Consts.WebSitesNames.Amountwork;
        public AmountworkParser(IEnumerable<IJobFilter> filters)
        {
            _filters = filters;
        }

        public Consts.WebSitesNames GetParserTarget { get => _tergetSite; }

        public async Task<IEnumerable<JobInfoModel>> ParseAsync(string urlToParse)
        {
            return await Task.Run(() => Parse(urlToParse));
        }

        private async Task<IEnumerable<JobInfoModel>> Parse(string urlToParse)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine($"{DateTime.Now}\tAmountwork parser ({threadId}) starting...");
            List<JobInfoModel> result = new();

            Console.WriteLine($"{DateTime.Now}\t[AmountworkParser-{threadId}]\tGetting data from {urlToParse}...");
            string htmlPage = await HttpHandler.GetHtmlAsync(urlToParse);

            Console.WriteLine($"{DateTime.Now}\t[AmountworkParser-{threadId}]\tReceiving job urls...");
            AmountworkHtmlParser amountworkParser = new AmountworkHtmlParser(urlToParse);
            var jobUrls = await amountworkParser.GetJobsUrls(htmlPage);
            Console.WriteLine($"{DateTime.Now}\t[AmountworkParser-{threadId}]\tReceived {jobUrls.Count()} urls");
            // if there is mistake in parsing or web site is invalid, end processing
            if (!jobUrls.Any())
            {
                Console.WriteLine($"{DateTime.Now}\t[AmountworkParser-{threadId}]\tEmergency stop.");
                return result;
            }

            var parallelModels = new ConcurrentBag<JobInfoModel>();

            Console.WriteLine($"{DateTime.Now}\t[AmountworkParser-{threadId}]\tReceiving html pages...");
            var htmlPages = new List<string>(); 
            foreach (var url in jobUrls) 
            {
                htmlPages.Add(await HttpHandler.GetHtmlAsync(url));
            }

            Console.WriteLine($"{DateTime.Now}\t[AmountworkParser-{threadId}]\tPages received. Start parsing...");
            await Parallel.ForEachAsync(htmlPages, async (htmlPage, token) =>
            {
                parallelModels.Add(await amountworkParser.ParseHtmlPage(htmlPage));
            });

            var models = parallelModels.ToList();           
            models.RemoveAll(m => m == null); // removes empty models (nulls)
            Console.WriteLine($"{DateTime.Now}\t[AmountworkParser-{threadId}]\tCreated {models.Count} objects");

            Console.WriteLine($"{DateTime.Now}\t[AmountworkParser-{threadId}]\tApplying filtration...");

            result = [.. models];
            foreach (var filter in _filters)
            {
                var filtrationResult = await filter.ApplyFiltration(result);
                result = filtrationResult.passeedModels.ToList();
                Console.WriteLine($"{DateTime.Now}\t[AmountworkParser-{threadId}]\t{filtrationResult.stats}");
            }
            Console.WriteLine($"{DateTime.Now}\t[AmountworkParser-{threadId}]\tFiltration completed. Filtration statistics:" +
                $"\n\tIncome items: {models.Count}\tOutcome items: {result.Count}\tFiltered items: {models.Count - result.Count}");


            return result;
        }
    }
}
