using WebParser.Models;

namespace WebParser.Interfaces
{
    interface IHtmlParser
    {
        /// <summary>
        /// Returns all urls of vacancies from search pages. If you send page '1' or '10' of 'x' to parse, this code will return vacancy urls from pages '1' to 'x'.
        /// </summary>
        /// <param name="htmlSearchPage">Html page written in string object. Note: this method requires a html page, not url!</param>
        /// <returns>Returns IEnumerable<string> that contains urls of finded vacancies</returns>
        Task<IEnumerable<string>> GetJobsUrls(string htmlSearchPage);

        /// <summary>
        /// Parses a single html-page of vacancy.
        /// </summary>
        /// <param name="htmlPage">Html page written in string object. Note: this method requires a html page, not url!</param>
        /// <returns>Returns JobInfoModel that contains all required fields (Title, description, phones, emails, location)</returns>
        Task<JobInfoModel> ParseHtmlPage(string htmlPage);
        
    }
}
