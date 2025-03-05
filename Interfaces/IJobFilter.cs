using WebParser.Models;

namespace WebParser.Interfaces
{
    interface IJobFilter
    {
        Task<(string stats, IEnumerable<JobInfoModel> passeedModels)> ApplyFiltration(IEnumerable<JobInfoModel> vacancies);

    }
}
