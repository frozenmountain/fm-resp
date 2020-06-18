using CommandLine;
using CommandLine.Text;
using System;
using System.Threading.Tasks;

namespace FM.Resp
{
    class Program
    {
        static void Main(string[] args)
        {
            using var parser = new CommandLine.Parser((settings) =>
            {
                settings.CaseInsensitiveEnumValues = true;
                settings.HelpWriter = null;
            });

            var result = parser.ParseArguments<
                AnalyzeOptions,
                ExportOptions,
                FilterOptions
            >(args);

            result.MapResult(
                (AnalyzeOptions options) =>
                {
                    return Task.Run(async () =>
                    {
                        return await new Analyzer(options).Run();
                    }).GetAwaiter().GetResult();
                },
                (ExportOptions options) =>
                {
                    return Task.Run(async () =>
                    {
                        return await new Exporter(options).Run();
                    }).GetAwaiter().GetResult();
                },
                (FilterOptions options) =>
                {
                    return Task.Run(async () =>
                    {
                        return await new Filterer(options).Run();
                    }).GetAwaiter().GetResult();
                },
                errors =>
                {
                    var helpText = HelpText.AutoBuild(result);
                    helpText.Copyright = "Copyright (C) 2020 Frozen Mountain Software Ltd.";
                    helpText.AddEnumValuesToHelpText = true;
                    Console.Write(helpText);
                    return 1;
                });
        }
    }
}
