namespace WebParser
{
    class Consts
    {
        public const string LogFilePath = "/app/out.log";
        public static readonly Dictionary<WebSitesNames, string> CoreUrls = new()
        {
            {WebSitesNames.Amountwork, "://amountwork.com/"},
        };
        public enum WebSitesNames
        {
            Amountwork,
        }
    }
}
