using WebParser.Models;

namespace WebParser.Interfaces
{
    interface IJobFilter
    {
        Task<(FiltrationStatsModel stats, IEnumerable<JobInfoModel> passeedModels)> ApplyFiltration(IEnumerable<JobInfoModel> vacancies);

    }
}
