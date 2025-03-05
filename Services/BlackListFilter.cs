using WebParser.Interfaces;
using WebParser.Models;

namespace WebParser.Services
{
    class BlackListFilter:IJobFilter
    {
        private readonly List<string> _blackList;

        public BlackListFilter(IEnumerable<string> blackList)
        {
            _blackList = blackList.ToList();
        }

        public async Task<(string stats, IEnumerable<JobInfoModel> passeedModels)> ApplyFiltration(IEnumerable<JobInfoModel> vacancies)
        {
            var passeedModels = vacancies.Where(v => !v.Contains(_blackList));
            string stats = $"Black list filtration statistics:\nBlack list items count: {_blackList.Count}\tIncome model count: {vacancies.Count()}\tOutcome model count: {passeedModels.Count()}";

            return (stats, passeedModels);
        }
    }
}
