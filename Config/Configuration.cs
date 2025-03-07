using CommandLine;
using Newtonsoft.Json;
using WebParser.Interfaces;
using WebParser.Models;
using WebParser.Parsers;
using WebParser.Services;
using WebParser.Services.Filters;

namespace WebParser.Config
{
    class Options
    {
        [Option("no-cli", Required = false, HelpText = "Disable output of logs to the terminal.")]
        public bool DisableConsoleOutput { get; set; }

        [Option('j', "file-json", Required = false, Separator = ' ', HelpText = "Path to save .json data.")]
        public IEnumerable<string> JsonFilePaths { get; set; }

        [Option('c', "file-csv", Required = false, Separator = ' ', HelpText = "Path to save .csv data.")]
        public IEnumerable<string> CsvFilePaths { get; set; }

        [Option('o', "file-log", Required = false, Separator = ' ', HelpText = "Path to save .log data.")]
        public IEnumerable<string> LogFilePaths { get; set; }

        [Option('a', "url-aw", Required = false, Separator = ' ',  HelpText = "URL link for Amountwork. Url must have a full path (starts with 'https://')")]
        public IEnumerable<string> AmountworkUrls { get; set; }

        [Option('u', "url", Required = false, Separator = ' ', HelpText = "URL link for other websites. Url must have a full path (starts with 'http://' or 'https://')")]
        public IEnumerable<string> Urls { get; set; }

        [Option('l', "url-list", Required = false, Separator = ' ', HelpText = "Path to the file with URL links. The file must contain just a single URL in each line. Url must have a full path (starts with 'http://' or 'https://')")]
        public IEnumerable<string> UrlFilePath { get; set; }

        [Option('b', "black-list", Required = false, Separator = ' ', HelpText = "Path to the JSON file with blacklist collection (the collection of strings).")]
        public IEnumerable<string> BlacklistFilePath { get; set; }

        [Option("a2", Required = false, Separator = ' ', HelpText = "Alpha2 code for country whose mobile numbers to search for.")]
        public IEnumerable<string> Alpha2Codes { get; set; }

        [Option("country-code", Required = false, Separator = ' ', HelpText = "Path to the JSON file with the country codes whose mobile numbers to search for, in alpha 2 format.")]
        public IEnumerable<string> Alpha2CodesFilePath { get; set; }
    }

    internal class Configuration
    {
        private static IEnumerable<string> _jsonPathes;
        private static IEnumerable<string> _csvPathes;
        private static IEnumerable<string> _logPathes;

        private static Dictionary<Consts.WebSitesNames, IEnumerable<string>> _urls;

        private static IEnumerable<string> _blackList;
        private static IEnumerable<string> _alpha2Codes;

        public static IEnumerable<string> GetAlpha2Codes { get => [.. _alpha2Codes]; }

        public static async Task RunWithOptions(Options opt)
        {
            Console.WriteLine($"{DateTime.Now}\tInitializing...");
            await Initialize(opt);

            Console.WriteLine($"{DateTime.Now}\tProgram initialized...");
            if (_urls.Any())
            {
                await RunParsing(opt);
            }
            else
            {
                Console.WriteLine($"{DateTime.Now}\t[0] URLs found... Program closing...");
            }

        }

        public static void HandleParseError(IEnumerable<Error> errs)
        {
            var errorTypes = string.Join(", ", errs.Select(err => err.Tag));
            Console.WriteLine($"{DateTime.Now}\tError parsing arguments:\t{errorTypes}");
        }



        private static async Task Initialize(Options opt)
        {
            SetOutputFiles(opt);
            await SetUrls(opt);
            await SetBlackList(opt);
            await SetSupportedPhoneCodes(opt);
        }

        private static async Task RunParsing(Options opt)
        {
            using (TeeTextWriter teeWriter = new TeeTextWriter(_logPathes, opt.DisableConsoleOutput))
            {
                Console.SetOut(teeWriter);
                Console.SetError(teeWriter);

                Console.WriteLine($"{DateTime.Now}\tProgram configured.");

                var models = new List<JobInfoModel>();
                var filters = new List<IJobFilter>()
                {
                    { new BlackListFilter(_blackList) },
                    { new PhoneFilter()},
                };
                var parsers = new List<IParser>()
                {
                    { new AmountworkParser(filters) },
                };

                IEnumerable<string> urlsToParse = [];
                IEnumerable<Task<IEnumerable<JobInfoModel>>> parseTasks;
                foreach (var parser in parsers)
                {
                    var parserTarget = parser.GetParserTarget;

                    urlsToParse = _urls.Where(kvp => kvp.Key == parserTarget)
                        .SelectMany(kvp => kvp.Value);
                    parseTasks = urlsToParse.Select(parser.ParseAsync);

                    var results = await Task.WhenAll(parseTasks);

                    models.AddRange(results.SelectMany(r => r));
                }

                await FileIO.SaveResults(models, _jsonPathes, _csvPathes);

                Console.WriteLine($"{DateTime.Now}\tParsing completed. Results stored in:\n{string.Join(", ", [.. _jsonPathes, .. _csvPathes])}");


                if (!opt.DisableConsoleOutput)
                {
                    Console.SetOut(Console.Out);
                    Console.SetError(Console.Error);
                }
            }
        }



        private static void SetOutputFiles(Options opt)
        {
            //Set places to save JSON files
            if (opt.JsonFilePaths.Any())
            {
                _jsonPathes = [.. opt.JsonFilePaths];
            }
            else
            {
                _jsonPathes = [Consts.JsonVacanciesPath];
            }

            //Set places to save CVS files
            if (opt.CsvFilePaths.Any())
            {
                _csvPathes = [.. opt.CsvFilePaths];
            }
            else
            {
                _csvPathes = [Consts.CsvVacanciesPath];
            }

            //Set places to save LOG files
            if (opt.LogFilePaths.Any())
            {
                _logPathes = [.. opt.LogFilePaths];
            }
            else
            {
                _logPathes = [Consts.LogFilePath];
            }
        }

        private static async Task SetUrls(Options opt)
        {
            _urls = new();

            //Set the url links for undefined web sites
            if (opt.Urls.Any())
            {
                //Adds only urls, that have the same beginning as the core one.
                _urls.Add(Consts.WebSitesNames.Undefined, [.. opt.Urls]);
            }

            //Set predeterminated urls
            if (opt.AmountworkUrls.Any())
            {
                //Adds only urls, that have the same beginning as the core one.
                _urls.Add(Consts.WebSitesNames.Amountwork, [.. opt.AmountworkUrls
                    .Where(url =>
                        url.StartsWith(Consts.CoreUrls[Consts.WebSitesNames.Amountwork]))]);
            }

            //
            //
            //Insert new supported web sites here
            //
            //

            //Gets urls frrom files
            if (opt.UrlFilePath.Any())
            {
                //From collection of files returns a single colection of urls
                var urlList = (await Task.WhenAll(opt.UrlFilePath.Select(FileIO.ReadAllLinesFromFile)))
                    .SelectMany(urls => urls);

                IEnumerable<string> tmpList;
                var orderedWebSitesNames = Enum.GetValues<Consts.WebSitesNames>()
                                .Cast<Consts.WebSitesNames>()
                                .OrderBy(w => w == Consts.WebSitesNames.Undefined ? 1 : 0);

                foreach (var w in orderedWebSitesNames)
                {
                    tmpList = urlList.Where(url => url.StartsWith(Consts.CoreUrls[w]));
                    urlList = urlList.Except(tmpList);
                    _urls[w] = _urls.ContainsKey(w) ? _urls[w].Concat(tmpList) : tmpList;
                }

            }

        }

        private static async Task SetBlackList(Options opt)
        {
            if (opt.BlacklistFilePath.Any())
            {
                IEnumerable<string> jsonList = await Task.WhenAll(opt.BlacklistFilePath.Select(FileIO.ReadAllTextFromFile));
                jsonList = jsonList.Where(FileIO.IsContentJSON);

                _blackList = jsonList.SelectMany(content =>
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<IEnumerable<string>>(content) ?? [];
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{DateTime.Now}\tError parsing JSON content:\n{e.Message}");
                        return [];
                    }
                });

            }
            else
            {
                _blackList = [];
            }
        }

        private static async Task SetSupportedPhoneCodes(Options opt)
        {
            if (opt.Alpha2Codes.Any())
            {
                _alpha2Codes = [.. opt.Alpha2Codes.Where(Consts.CountryAlpha2Code.Contains)];
            }

            if (opt.Alpha2CodesFilePath.Any())
            {
                IEnumerable<string> jsonList = await Task.WhenAll(opt.Alpha2CodesFilePath.Select(FileIO.ReadAllTextFromFile));
                jsonList = jsonList.Where(FileIO.IsContentJSON);

                var newItems = jsonList.SelectMany(content =>
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<IEnumerable<string>>(content) ?? [];
                    }
                    catch (JsonException e)
                    {
                        Console.WriteLine($"{DateTime.Now}\tError parsing JSON content:\n{e.Message}");
                        return [];
                    }
                });

                _alpha2Codes = _alpha2Codes?.Concat(newItems) ?? newItems;
            }

            if (_alpha2Codes == null || !_alpha2Codes.Any())
            {
                _alpha2Codes = [.. Consts.CountryAlpha2Code];
            }
        }




    }
}
