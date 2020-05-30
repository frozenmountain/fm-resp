using System;
using System.Collections.Generic;
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
                Console.WriteLine($"File does not exist: {Options.Input}");
                return 1;
            }

            // open the file
            var stream = File.Open(Options.Input, FileMode.Open, FileAccess.Read, FileShare.Read);

            // seek ahead if asked to do so
            if (Options.InputOffset > 0)
            {
                stream.Seek(Options.InputOffset, SeekOrigin.Current);
            }

            var reader = new Reader(stream);

            // display progress in a reasonable efficient manner
            var displayingProgress = false;
            reader.OnReadType += (type) =>
            {
                // if we are currently updating the display, ignore this
                if (!displayingProgress)
                {
                    // make sure subsequent display attempt bail out
                    displayingProgress = true;
                    Task.Run(() =>
                    {
                        try
                        {
                            // display the progress
                            DisplayProgress(stream, true);
                        }
                        finally
                        {
                            // allow the next read to display again
                            displayingProgress = false;
                        }
                    });
                }
            };

            // initialize collection
            var types = Enum.GetValues(typeof(DataType)).Cast<DataType>();
            var allElements = new List<Element>();
            var elementsByType = new Dictionary<DataType, List<Element>>();
            foreach (var type in types)
            {
                elementsByType[type] = new List<Element>();
            }

            // start reading
            Element element;
            while ((element = await reader.ReadAsync()).Type != DataType.EndOfStream)
            {
                allElements.Add(element);
                elementsByType[element.Type].Add(element);
            }

            // one final update (should be 100%)
            while (displayingProgress)
            {
                await Task.Delay(1);
            }
            DisplayProgress(stream, false);
            Console.WriteLine();

            // analyze the results
            foreach (var type in types)
            {
                var elements = elementsByType[type];
                Console.WriteLine($"{type}");
                Console.WriteLine($"  Count: {elements.Count}");
                if (elements.Count == 0)
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

        private void DisplayProgress(Stream stream, bool resetCursorPosition)
        {
            var streamPosition = stream.Position;
            var consoleCursorTop = Console.CursorTop;
            var consoleCursorLeft = Console.CursorLeft;
            Console.WriteLine($"Reading stream... {streamPosition} / {stream.Length} ({(float)streamPosition / stream.Length:P})");
            if (resetCursorPosition)
            {
                Console.SetCursorPosition(consoleCursorLeft, consoleCursorTop);
            }
        }
    }
}
