using WebParser.Models;

namespace WebParser.Interfaces
{
    interface IHtmlParser
    {
        Task<IEnumerable<string>> GetJobsUrls(string htmlSearchPage);
        Task<JobInfoModel> ParseUrl(string htmlPage);
        
    }
}
