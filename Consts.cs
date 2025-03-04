namespace WebParser
{
    class Consts
    {
        public const string LogFilePath = "/app/out.log";
        public const string JsonVacanciesPath = "/app/vacancies.json";
        public const string CsvVacanciesPath = "/app/vacancies.csv";
        public static readonly Dictionary<WebSitesNames, string> CoreUrls = new()
        {
            {WebSitesNames.Amountwork, "https://amountwork.com/"},
        };
        public enum WebSitesNames
        {
            Amountwork,
        }
    }
}
