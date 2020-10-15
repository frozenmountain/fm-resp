using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FM.Resp
{
    abstract class OutputVerb<TOptions> : InputVerb<TOptions>
        where TOptions : OutputOptions
    {
        protected OutputVerb(TOptions options)
            : base(options)
        { }

        protected override async Task<int> ProcessStream(Stream stream)
        {
            var elements = await ProcessStreamIntoElements(stream).ConfigureAwait(false);

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
                await File.WriteAllTextAsync(Options.Output, json);
                Console.Error.WriteLine(" done.");
            }
            else
            {
                Console.WriteLine(json);
            }

            return 0;
        }

        protected abstract Task<IEnumerable<Element>> ProcessStreamIntoElements(Stream stream);
    }
}
