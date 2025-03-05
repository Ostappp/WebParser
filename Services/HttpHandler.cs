namespace WebParser.Services
{
    class HttpHandler
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

        /// <summary>
        /// Performs an HTTP POST request to the specified URL with the provided form data 
        /// and returns the result as a string.
        /// </summary>
        /// <param name="url">The URL to which the POST request is made.</param>
        /// <param name="formData">Form data in the form of StringContent.</param>
        /// <returns>The result is a string.</returns>
        public static async Task<string> GetFormResultAsync(string url, StringContent formData)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:91.0) Gecko/20100101 Firefox/91.0");

                    var postResponse = await client.PostAsync(url, formData);
                    var postResult = await postResponse.Content.ReadAsStringAsync();

                    return postResult;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"\nException Caught!\n[{DateTime.Now}]");
                    Console.WriteLine($"Message: {e.Message}");
                    Console.WriteLine($"Target: {e.TargetSite}");
                    Console.WriteLine($"Status Code: {e.StatusCode}");
                    Console.WriteLine($"Request Message: {e.HttpRequestError}");
                    Console.WriteLine($"Stack Trace: {e.StackTrace}\n");
                    return string.Empty;
                }
                catch(UriFormatException e)
                {
                    Console.WriteLine($"\nException Caught!\n[{DateTime.Now}]");
                    Console.WriteLine($"Message: {e.Message}");
                    Console.WriteLine($"Target: {e.TargetSite}");
                    Console.WriteLine($"Stack Trace: {e.StackTrace}\n");
                    return string.Empty;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"\nException Caught![{DateTime.Now}]\nMessage: {e.Message}");
                    Console.WriteLine($"Stack Trace: {e.StackTrace}\n");
                    return string.Empty;
                }
            }
        }
    }
}