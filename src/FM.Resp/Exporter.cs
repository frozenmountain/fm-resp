using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FM.Resp
{
    class Exporter : OutputVerb<ExportOptions>
    {
        public Exporter(ExportOptions options)
            : base(options)
        { }

        protected override async Task<int> ProcessStream(Stream stream)
        {
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
