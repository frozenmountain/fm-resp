using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
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

                var elements = result.Elements.Where(x => x.Type != DataType.EndOfStream);

                Console.Write("Serializing to JSON...");
                var json = JsonConvert.SerializeObject(elements, Options.Indented ? Formatting.Indented : Formatting.None, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                Console.WriteLine(" done.");

                Console.Write("Writing to output file...");
                File.WriteAllText(Options.Output, json);
                Console.WriteLine(" done.");

                foreach (var type in new[] { DataType.SimpleString, DataType.Error, DataType.Integer, DataType.BulkString, DataType.Array })
                {
                    Console.WriteLine($"{type} count: {elements.Count(x => x.Type == type)}");
                }
            }

            return 0;
        }
    }
}
