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
                Console.Error.WriteLine($"Input file does not exist: {Options.Input}");
                return 1;
            }

            if (Options.Output != null && !Options.Overwrite && File.Exists(Options.Output))
            {
                Console.Error.WriteLine($"Output file already exists (use -y to overwrite): {Options.Output}");
                return 1;
            }

            // open the file
            using var stream = File.Open(Options.Input, FileMode.Open, FileAccess.Read, FileShare.Read);

            // create the parser
            var parser = new Parser(stream);

            // parse the stream
            var result = await parser.Parse();

            var elements = result.Elements.Where(x => x.Type != DataType.EndOfStream);

            Console.Error.Write("Serializing to JSON...");
            var json = JsonConvert.SerializeObject(elements, Options.Indented ? Formatting.Indented : Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            Console.Error.WriteLine(" done.");

            foreach (var type in new[] { DataType.SimpleString, DataType.Error, DataType.Integer, DataType.BulkString, DataType.Array })
            {
                Console.Error.WriteLine($"{type} count: {elements.Count(x => x.Type == type)}");
            }

            if (Options.Output != null)
            {
                Console.Error.Write("Writing to output file...");
                File.WriteAllText(Options.Output, json);
                Console.Error.WriteLine(" done.");
            }
            else
            {
                Console.WriteLine(json);
            }

            return 0;
        }
    }
}
