using WebParser.Interfaces;
using WebParser.Models;

namespace WebParser.Services.Filters
{
    class BlackListFilter:IJobFilter
    {
        private readonly List<string> _blackList;

        public BlackListFilter(IEnumerable<string> blackList)
        {
            _blackList = blackList.ToList();
        }

        public async Task<(FiltrationStatsModel stats, IEnumerable<JobInfoModel> passeedModels)> ApplyFiltration(IEnumerable<JobInfoModel> vacancies)
        {
            var passeedModels = vacancies.Where(v => !v.Contains(_blackList));
            FiltrationStatsModel stats = new()
            {
                Filter = typeof(BlackListFilter),
                Message = "Filter excludes elements, that contains one or more blacklist items.",
                FilerItemsCount = _blackList.Count,
                IncomeItemsCount = vacancies.Count(),
                OutcomeItemsCount = passeedModels.Count(),
            };
            
            return (stats, passeedModels);
        }
    }
}
