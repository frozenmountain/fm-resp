using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FM.Resp
{
    class Analyzer
    {
        public AnalyzeOptions Options { get; private set; }

        public Analyzer(AnalyzeOptions options)
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

            // seek ahead if asked to do so
            if (Options.InputOffset > 0)
            {
                stream.Seek(Options.InputOffset, SeekOrigin.Current);
            }

            // create the parser
            var parser = new Parser(stream);

            // parse the stream
            var result = await parser.Parse();

            // analyze the results
            foreach (var type in result.Types)
            {
                var elements = result.GetElements(type);
                Console.WriteLine();
                Console.WriteLine($"{type}:");
                Console.WriteLine($"  Count: {elements.Length}");
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
                    Console.WriteLine($"  Length Average: {lengths.Average()}");
                    Console.WriteLine($"  Length Minimum: {lengths.Min()}");
                    Console.WriteLine($"  Length Maximum: {lengths.Max()}");
                }
                else if (type == DataType.Integer)
                {
                    var integers = elements.Select(x => x.Value).Cast<int>();
                    Console.WriteLine($"  Value Average: {integers.Average()}");
                    Console.WriteLine($"  Value Minimum: {integers.Min()}");
                    Console.WriteLine($"  Value Maximum: {integers.Max()}");
                }
                else if (type == DataType.Array)
                {
                    var arrays = elements.Select(x => x.Value).Cast<Element[]>();
                    var counts = arrays.Select(x => x?.Length ?? 0);
                    Console.WriteLine($"  Count Average: {counts.Average()}");
                    Console.WriteLine($"  Count Minimum: {counts.Min()}");
                    Console.WriteLine($"  Count Maximum: {counts.Max()}");
                }
            }

            return 0;
        }
    }
}
