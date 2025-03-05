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
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:91.0) Gecko/20100101 Firefox/91.0");

                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode(); 

                    string htmlContent = await response.Content.ReadAsStringAsync();
                    return htmlContent;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"\nException Caught!\n[{DateTime.Now}]");
                    Console.WriteLine($"Message: {e.Message}");
                    Console.WriteLine($"Target: {e.TargetSite}");
                    Console.WriteLine($"Status Code: {e.StatusCode}");
                    Console.WriteLine($"Request Message: {e.HttpRequestError}");
                    Console.WriteLine($"Stack Trace: {e.StackTrace}");
                    return string.Empty;
                }
                catch(Exception e)
                {
                    Console.WriteLine($"[{DateTime.Now}]\nMessage: {e.Message}");
                    Console.WriteLine($"Stack Trace: {e.StackTrace}");
                    return string.Empty;
                }
            }
        }
    }
}