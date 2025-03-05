using System;
using System.Collections.Concurrent;
using WebParser.Interfaces;
using WebParser.Models;
using WebParser.Services;

namespace WebParser.Parsers
{
    class AmountworkParser
    {
        private readonly IEnumerable<IJobFilter> _filters;
        private readonly string _urlToParse;

        public AmountworkParser(IEnumerable<IJobFilter> filters, string urlToParse)
        {
            _filters = filters;
            _urlToParse = urlToParse;
        }

        public async Task<IEnumerable<JobInfoModel>> ParseAsync()
        {
            return await Task.Run(Parse);
        }

        private async Task<IEnumerable<JobInfoModel>> Parse()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine($"{DateTime.Now}\tAmountwork parser ({threadId}) starting...");
            List<JobInfoModel> result = new();

            Console.WriteLine($"{DateTime.Now}\t[AmountworkParser-{threadId}]\tGetting data from {_urlToParse}...");
            string htmlPage = await HttpHandler.GetHtmlAsync(_urlToParse);

            Console.WriteLine($"{DateTime.Now}\t[AmountworkParser-{threadId}]\tReceiving job urls...");
            AmountworkHtmlParser amountworkParser = new AmountworkHtmlParser(_urlToParse);
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
            var htnlPages = new List<string>(); 
            foreach (var url in jobUrls) 
            {
                htnlPages.Add(await HttpHandler.GetHtmlAsync(url));
            }

            Console.WriteLine($"{DateTime.Now}\t[AmountworkParser-{threadId}]\tPages received. Start parsing...");
            await Parallel.ForEachAsync(htnlPages, async (htmlPage, token) =>
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
                var filtration = await filter.ApplyFiltration(result);
                result = filtration.passeedModels.ToList();
                Console.WriteLine($"{DateTime.Now}\t[AmountworkParser-{threadId}]\t{filtration.stats}");
            }
            Console.WriteLine($"{DateTime.Now}\t[AmountworkParser-{threadId}]\tFiltration completed. Filtration statistics:" +
                $"\n\tIncome items: {models.Count}\tOutcome items: {result.Count}\tFiltered items: {models.Count - result.Count}");


            return result;
        }
    }
}
