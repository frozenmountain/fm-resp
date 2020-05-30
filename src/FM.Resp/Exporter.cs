using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FM.Resp
{
    class Exporter
    {
        public ExportOptions Options { get; private set; }

        public Exporter(ExportOptions options)
        {
            Options = options;
        }

        public async Task<int> Run()
        {
            // sanity check on the file
            if (!File.Exists(Options.Input))
            {
                Console.WriteLine($"File does not exist: {Options.Input}");
                return 1;
            }

            // open the file
            var stream = File.Open(Options.Input, FileMode.Open, FileAccess.Read, FileShare.Read);

            // create the parser
            var parser = new Parser(stream);

            // parse the stream
            var result = await parser.Parse();

            //TODO: export
            if (Options.OutputFormat == FileFormat.Json)
            {
                File.WriteAllText(Options.Output, JsonConvert.SerializeObject(result.Elements));
            }

            return 0;
        }
    }
}
