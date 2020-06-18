using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FM.Resp
{
    class Filterer
    {
        public FilterOptions Options { get; private set; }

        public Filterer(FilterOptions options)
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

            await Task.Yield();

            if (Options.Format == FileFormat.Json)
            {
                // open the file
                using var stream = File.Open(Options.Input, FileMode.Open, FileAccess.Read, FileShare.Read);

                // deserialize
                Console.Write("Deserializing from JSON...");
                using var streamReader = new StreamReader(stream);
                using var jsonReader = new JsonTextReader(streamReader);
                var elements = new JsonSerializer().Deserialize<IEnumerable<Element>>(jsonReader);
                Console.WriteLine(" done.");

                if (Options.FromIndex != null)
                {
                    Console.Write("Filtering elements before 'from' index...");
                    elements = Filter(elements, x => x.Index < Options.FromIndex, out var removeCount);
                    Console.WriteLine(" done.");
                    Console.WriteLine($"{removeCount} element(s) removed.");
                }

                if (Options.ToIndex != null)
                {
                    Console.Write("Filtering elements after 'to' index...");
                    elements = Filter(elements, x => x.Index > Options.ToIndex, out var removeCount);
                    Console.WriteLine(" done.");
                    Console.WriteLine($"{removeCount} element(s) removed.");
                }

                if (Options.NoSimpleStrings)
                {
                    Console.Write("Filtering simple strings...");
                    elements = Filter(elements, x => x.Type == DataType.SimpleString, out var removeCount);
                    Console.WriteLine(" done.");
                    Console.WriteLine($"{removeCount} simple string(s) removed.");
                }
                if (Options.MinSimpleStringLength != null)
                {
                    Console.Write($"Filtering simple strings with length less than {Options.MinSimpleStringLength}...");
                    elements = Filter(elements, x => x.Type == DataType.SimpleString && x.Length < Options.MinSimpleStringLength, out var removeCount);
                    Console.WriteLine(" done.");
                    Console.WriteLine($"{removeCount} simple string(s) removed.");
                }
                if (Options.MaxSimpleStringLength != null)
                {
                    Console.Write($"Filtering simple strings with length greater than {Options.MaxSimpleStringLength}...");
                    elements = Filter(elements, x => x.Type == DataType.SimpleString && x.Length > Options.MaxSimpleStringLength, out var removeCount);
                    Console.WriteLine(" done.");
                    Console.WriteLine($"{removeCount} simple string(s) removed.");
                }

                if (Options.NoErrors)
                {
                    Console.Write("Filtering errors...");
                    elements = Filter(elements, x => x.Type == DataType.Error, out var removeCount);
                    Console.WriteLine(" done.");
                    Console.WriteLine($"{removeCount} error(s) removed.");
                }
                if (Options.MinErrorLength != null)
                {
                    Console.Write($"Filtering errors with length less than {Options.MinErrorLength}...");
                    elements = Filter(elements, x => x.Type == DataType.Error && x.Length < Options.MinErrorLength, out var removeCount);
                    Console.WriteLine(" done.");
                    Console.WriteLine($"{removeCount} error(s) removed.");
                }
                if (Options.MaxErrorLength != null)
                {
                    Console.Write($"Filtering errors with length greater than {Options.MaxErrorLength}...");
                    elements = Filter(elements, x => x.Type == DataType.Error && x.Length > Options.MaxErrorLength, out var removeCount);
                    Console.WriteLine(" done.");
                    Console.WriteLine($"{removeCount} error(s) removed.");
                }

                if (Options.NoIntegers)
                {
                    Console.Write("Filtering integers...");
                    elements = Filter(elements, x => x.Type == DataType.Integer, out var removeCount);
                    Console.WriteLine(" done.");
                    Console.WriteLine($"{removeCount} integer(s) removed.");
                }
                if (Options.MinInteger != null)
                {
                    Console.Write($"Filtering integers less than {Options.MinInteger}...");
                    elements = Filter(elements, x => x.Type == DataType.Integer && (long)x.Value < Options.MinInteger, out var removeCount);
                    Console.WriteLine(" done.");
                    Console.WriteLine($"{removeCount} integer(s) removed.");
                }
                if (Options.MaxInteger != null)
                {
                    Console.Write($"Filtering integers greater than {Options.MaxInteger}...");
                    elements = Filter(elements, x => x.Type == DataType.Integer && (long)x.Value > Options.MaxInteger, out var removeCount);
                    Console.WriteLine(" done.");
                    Console.WriteLine($"{removeCount} integer(s) removed.");
                }
                if (Options.MinIntegerLength != null)
                {
                    Console.Write($"Filtering integers with length less than {Options.MinIntegerLength}...");
                    elements = Filter(elements, x => x.Type == DataType.Integer && x.Length < Options.MinIntegerLength, out var removeCount);
                    Console.WriteLine(" done.");
                    Console.WriteLine($"{removeCount} integer(s) removed.");
                }
                if (Options.MaxIntegerLength != null)
                {
                    Console.Write($"Filtering integers with length greater than {Options.MaxIntegerLength}...");
                    elements = Filter(elements, x => x.Type == DataType.Integer && x.Length > Options.MaxIntegerLength, out var removeCount);
                    Console.WriteLine(" done.");
                    Console.WriteLine($"{removeCount} integer(s) removed.");
                }

                if (Options.NoBulkStrings)
                {
                    Console.Write("Filtering bulk strings...");
                    elements = Filter(elements, x => x.Type == DataType.BulkString, out var removeCount);
                    Console.WriteLine(" done.");
                    Console.WriteLine($"{removeCount} bulk string(s) removed.");
                }
                if (Options.MinBulkStringLength != null)
                {
                    Console.Write($"Filtering bulk strings with length less than {Options.MinBulkStringLength}...");
                    elements = Filter(elements, x => x.Type == DataType.BulkString && x.Length < Options.MinBulkStringLength, out var removeCount);
                    Console.WriteLine(" done.");
                    Console.WriteLine($"{removeCount} bulk string(s) removed.");
                }
                if (Options.MaxBulkStringLength != null)
                {
                    Console.Write($"Filtering bulk strings with length greater than {Options.MaxBulkStringLength}...");
                    elements = Filter(elements, x => x.Type == DataType.BulkString && x.Length > Options.MaxBulkStringLength, out var removeCount);
                    Console.WriteLine(" done.");
                    Console.WriteLine($"{removeCount} bulk string(s) removed.");
                }

                if (Options.NoArrays)
                {
                    Console.Write("Filtering arrays...");
                    elements = Filter(elements, x => x.Type == DataType.Array, out var removeCount);
                    Console.WriteLine(" done.");
                    Console.WriteLine($"{removeCount} array(s) removed.");
                }
                if (Options.MinArrayLength != null)
                {
                    Console.Write($"Filtering arrays with length less than {Options.MinArrayLength}...");
                    elements = Filter(elements, x => x.Type == DataType.Array && x.Length < Options.MinArrayLength, out var removeCount);
                    Console.WriteLine(" done.");
                    Console.WriteLine($"{removeCount} array(s) removed.");
                }
                if (Options.MaxArrayLength != null)
                {
                    Console.Write($"Filtering arrays with length greater than {Options.MaxArrayLength}...");
                    elements = Filter(elements, x => x.Type == DataType.Array && x.Length > Options.MaxArrayLength, out var removeCount);
                    Console.WriteLine(" done.");
                    Console.WriteLine($"{removeCount} array(s) removed.");
                }

                foreach (var element in elements)
                {
                    switch (element.Type)
                    {
                        case DataType.SimpleString:
                            element.NoValue = Options.NoSimpleStringValues;
                            break;
                        case DataType.Error:
                            element.NoValue = Options.NoErrorValues;
                            break;
                        case DataType.Integer:
                            element.NoValue = Options.NoIntegerValues;
                            break;
                        case DataType.BulkString:
                            element.NoValue = Options.NoBulkStringValues;
                            break;
                        case DataType.Array:
                            element.NoValue = Options.NoArrayValues;
                            break;
                    }
                }

                if (!Options.Overwrite && File.Exists(Options.Output))
                {
                    throw new Exception($"Output file already exists (use -y to overwrite): {Options.Output}");
                }

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

        private IEnumerable<Element> Filter(IEnumerable<Element> elements, Func<Element, bool> condition, out int removeCount)
        {
            var removeElements = elements.Where(condition);
            removeCount = removeElements.Count();
            return elements.Except(removeElements);
        }
    }
}
