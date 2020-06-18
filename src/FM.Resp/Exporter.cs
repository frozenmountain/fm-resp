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
                Console.WriteLine($"Input file does not exist: {Options.Input}");
                return 1;
            }

            // open the file
            using var stream = File.Open(Options.Input, FileMode.Open, FileAccess.Read, FileShare.Read);

            // create the parser
            var parser = new Parser(stream);

            // parse the stream
            var result = await parser.Parse();

            if (Options.OutputFormat == FileFormat.Json)
            {
                if (!Options.Overwrite && File.Exists(Options.Output))
                {
                    throw new Exception($"Output file already exists (use -y to overwrite): {Options.Output}");
                }

                Console.Write("Serializing to JSON...");
                var json = JsonConvert.SerializeObject(result.Elements, Options.Indented ? Formatting.Indented : Formatting.None);
                Console.WriteLine(" done.");

                Console.Write("Writing to output file...");
                File.WriteAllText(Options.Output, json);
                Console.WriteLine(" done.");
            }

            return 0;
        }
    }
}
