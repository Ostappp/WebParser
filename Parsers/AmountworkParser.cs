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
            Console.WriteLine($"{DateTime.Now}\tAmountwork parser starting...");

            Console.WriteLine($"{DateTime.Now}\t[AmountworkParser]\tGetting data from {_urlToParse}...");
            string htmlPage = await HttpHandler.GetHtmlAsync(_urlToParse);

            AmountworkHtmlParser amountworkParser = new AmountworkHtmlParser(_urlToParse);
            var jobUrls = await amountworkParser.GetJobsUrls(htmlPage);
            Console.WriteLine($"{DateTime.Now}\t[AmountworkParser]\tReceived {jobUrls.Count()} urls");

            List<JobInfoModel> models = new();
            foreach (var url in jobUrls)
            {
                htmlPage = await HttpHandler.GetHtmlAsync(url);
                models.Add(await amountworkParser.ParseHtmlPage(htmlPage));
            }
            models.RemoveAll(m => m == null); // removes empty models (nulls)
            Console.WriteLine($"{DateTime.Now}\t[AmountworkParser]\tCreated {models.Count} objects");

            Console.WriteLine($"{DateTime.Now}\t[AmountworkParser]\tApplying filtration...");

            List<JobInfoModel> filtradedJobs = [.. models];
            foreach (var filter in _filters)
            {
                var filtration = await filter.ApplyFiltration(filtradedJobs);
                filtradedJobs = filtration.passeedModels.ToList();
                Console.WriteLine($"{DateTime.Now}\t[AmountworkParser]\t{filtration.stats}");
            }
            Console.WriteLine($"{DateTime.Now}\t[AmountworkParser]\tFiltration completed. Filtration statistics:" +
                $"\n\tIncome items: {models.Count}\tOutcome items: {filtradedJobs.Count}\tFiltered items: {models.Count - filtradedJobs.Count}");


            return filtradedJobs;
        }
    }
}
