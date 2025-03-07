using CommandLine;
using WebParser.Config;

namespace WebParser
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args);

            await result
                .WithParsedAsync(Configuration.RunWithOptions);

            result.WithNotParsed(Configuration.HandleParseError);
        }
    }
}
