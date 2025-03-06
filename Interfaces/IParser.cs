using WebParser.Config;
using WebParser.Models;

namespace WebParser.Interfaces
{
    interface IParser
    {
        Consts.WebSitesNames GetParserTarget { get; }
        Task<IEnumerable<JobInfoModel>> ParseAsync(string urlToParse);
    }
}
