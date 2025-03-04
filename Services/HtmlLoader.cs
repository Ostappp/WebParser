namespace WebParser.Services
{
    class HtmlLoader
    {
        public static async Task<string> GetHtmlAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode(); 

                    string htmlContent = await response.Content.ReadAsStringAsync();
                    return htmlContent;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message: {0} ", e.Message);
                    return string.Empty;
                }

            }
        }
    }
}