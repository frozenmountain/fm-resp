using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FM.Resp
{
    class Analyzer : InputVerb<AnalyzeOptions>
    {
        public Analyzer(AnalyzeOptions options)
            : base(options)
        { }

        protected override async Task<int> ProcessStream(Stream stream)
        {
            // create the parser
            var parser = new Parser(stream);

            // parse the stream
            var result = await parser.Parse();

            // analyze the results
            foreach (var type in result.Types)
            {
                var elements = result.GetElements(type);
                Console.Error.WriteLine();
                Console.Error.WriteLine($"{type}:");
                Console.Error.WriteLine($"  Count: {elements.Length}");
                if (elements.Length == 0)
                {
                    continue;
                }
                if (type == DataType.SimpleString ||
                    type == DataType.Error ||
                    type == DataType.BulkString)
                {
                    var strings = elements.Select(x => x.Value).Cast<string>();
                    var lengths = strings.Select(x => x?.Length ?? 0);
                    Console.Error.WriteLine($"  Length Average: {lengths.Average()}");
                    Console.Error.WriteLine($"  Length Minimum: {lengths.Min()}");
                    Console.Error.WriteLine($"  Length Maximum: {lengths.Max()}");
                }
                else if (type == DataType.Integer)
                {
                    var integers = elements.Select(x => x.Value).Cast<int>();
                    Console.Error.WriteLine($"  Value Average: {integers.Average()}");
                    Console.Error.WriteLine($"  Value Minimum: {integers.Min()}");
                    Console.Error.WriteLine($"  Value Maximum: {integers.Max()}");
                }
                else if (type == DataType.Array)
                {
                    var arrays = elements.Select(x => x.Value).Cast<Element[]>();
                    var counts = arrays.Select(x => x?.Length ?? 0);
                    Console.Error.WriteLine($"  Count Average: {counts.Average()}");
                    Console.Error.WriteLine($"  Count Minimum: {counts.Min()}");
                    Console.Error.WriteLine($"  Count Maximum: {counts.Max()}");
                }
            }

            return 0;
        }
    }
}
