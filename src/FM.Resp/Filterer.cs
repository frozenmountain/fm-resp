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
                Console.Error.WriteLine($"Input file does not exist: {Options.Input}");
                return 1;
            }

            if (Options.Output != null && !Options.Overwrite && File.Exists(Options.Output))
            {
                Console.Error.WriteLine($"Output file already exists (use -y to overwrite): {Options.Output}");
                return 1;
            }

            await Task.Yield();

            if (Options.Format == FileFormat.Json)
            {
                // open the file
                using var stream = File.Open(Options.Input, FileMode.Open, FileAccess.Read, FileShare.Read);

                // deserialize
                Console.Error.Write("Deserializing from JSON...");
                using var streamReader = new StreamReader(stream);
                using var jsonReader = new JsonTextReader(streamReader);
                var elements = new JsonSerializer().Deserialize<IEnumerable<Element>>(jsonReader);
                Console.Error.WriteLine(" done.");

                if (Options.FromIndex != null)
                {
                    Console.Error.Write("Filtering elements before 'from' index...");
                    elements = Filter(elements, x => x.Index < Options.FromIndex, out var removeCount);
                    Console.Error.WriteLine(" done.");
                    Console.Error.WriteLine($"{removeCount} element(s) removed.");
                }

                if (Options.ToIndex != null)
                {
                    Console.Error.Write("Filtering elements after 'to' index...");
                    elements = Filter(elements, x => x.Index > Options.ToIndex, out var removeCount);
                    Console.Error.WriteLine(" done.");
                    Console.Error.WriteLine($"{removeCount} element(s) removed.");
                }

                if (Options.NoSimpleStrings)
                {
                    Console.Error.Write("Filtering simple strings...");
                    elements = Filter(elements, x => x.Type == DataType.SimpleString, out var removeCount);
                    Console.Error.WriteLine(" done.");
                    Console.Error.WriteLine($"{removeCount} simple string(s) removed.");
                }
                if (Options.MinSimpleStringLength != null)
                {
                    Console.Error.Write($"Filtering simple strings with length less than {Options.MinSimpleStringLength}...");
                    elements = Filter(elements, x => x.Type == DataType.SimpleString && x.Length < Options.MinSimpleStringLength, out var removeCount);
                    Console.Error.WriteLine(" done.");
                    Console.Error.WriteLine($"{removeCount} simple string(s) removed.");
                }
                if (Options.MaxSimpleStringLength != null)
                {
                    Console.Error.Write($"Filtering simple strings with length greater than {Options.MaxSimpleStringLength}...");
                    elements = Filter(elements, x => x.Type == DataType.SimpleString && x.Length > Options.MaxSimpleStringLength, out var removeCount);
                    Console.Error.WriteLine(" done.");
                    Console.Error.WriteLine($"{removeCount} simple string(s) removed.");
                }

                if (Options.NoErrors)
                {
                    Console.Error.Write("Filtering errors...");
                    elements = Filter(elements, x => x.Type == DataType.Error, out var removeCount);
                    Console.Error.WriteLine(" done.");
                    Console.Error.WriteLine($"{removeCount} error(s) removed.");
                }
                if (Options.MinErrorLength != null)
                {
                    Console.Error.Write($"Filtering errors with length less than {Options.MinErrorLength}...");
                    elements = Filter(elements, x => x.Type == DataType.Error && x.Length < Options.MinErrorLength, out var removeCount);
                    Console.Error.WriteLine(" done.");
                    Console.Error.WriteLine($"{removeCount} error(s) removed.");
                }
                if (Options.MaxErrorLength != null)
                {
                    Console.Error.Write($"Filtering errors with length greater than {Options.MaxErrorLength}...");
                    elements = Filter(elements, x => x.Type == DataType.Error && x.Length > Options.MaxErrorLength, out var removeCount);
                    Console.Error.WriteLine(" done.");
                    Console.Error.WriteLine($"{removeCount} error(s) removed.");
                }

                if (Options.NoIntegers)
                {
                    Console.Error.Write("Filtering integers...");
                    elements = Filter(elements, x => x.Type == DataType.Integer, out var removeCount);
                    Console.Error.WriteLine(" done.");
                    Console.Error.WriteLine($"{removeCount} integer(s) removed.");
                }
                if (Options.MinInteger != null)
                {
                    Console.Error.Write($"Filtering integers less than {Options.MinInteger}...");
                    elements = Filter(elements, x => x.Type == DataType.Integer && (long)x.Value < Options.MinInteger, out var removeCount);
                    Console.Error.WriteLine(" done.");
                    Console.Error.WriteLine($"{removeCount} integer(s) removed.");
                }
                if (Options.MaxInteger != null)
                {
                    Console.Error.Write($"Filtering integers greater than {Options.MaxInteger}...");
                    elements = Filter(elements, x => x.Type == DataType.Integer && (long)x.Value > Options.MaxInteger, out var removeCount);
                    Console.Error.WriteLine(" done.");
                    Console.Error.WriteLine($"{removeCount} integer(s) removed.");
                }
                if (Options.MinIntegerLength != null)
                {
                    Console.Error.Write($"Filtering integers with length less than {Options.MinIntegerLength}...");
                    elements = Filter(elements, x => x.Type == DataType.Integer && x.Length < Options.MinIntegerLength, out var removeCount);
                    Console.Error.WriteLine(" done.");
                    Console.Error.WriteLine($"{removeCount} integer(s) removed.");
                }
                if (Options.MaxIntegerLength != null)
                {
                    Console.Error.Write($"Filtering integers with length greater than {Options.MaxIntegerLength}...");
                    elements = Filter(elements, x => x.Type == DataType.Integer && x.Length > Options.MaxIntegerLength, out var removeCount);
                    Console.Error.WriteLine(" done.");
                    Console.Error.WriteLine($"{removeCount} integer(s) removed.");
                }

                if (Options.NoBulkStrings)
                {
                    Console.Error.Write("Filtering bulk strings...");
                    elements = Filter(elements, x => x.Type == DataType.BulkString, out var removeCount);
                    Console.Error.WriteLine(" done.");
                    Console.Error.WriteLine($"{removeCount} bulk string(s) removed.");
                }
                if (Options.MinBulkStringLength != null)
                {
                    Console.Error.Write($"Filtering bulk strings with length less than {Options.MinBulkStringLength}...");
                    elements = Filter(elements, x => x.Type == DataType.BulkString && x.Length < Options.MinBulkStringLength, out var removeCount);
                    Console.Error.WriteLine(" done.");
                    Console.Error.WriteLine($"{removeCount} bulk string(s) removed.");
                }
                if (Options.MaxBulkStringLength != null)
                {
                    Console.Error.Write($"Filtering bulk strings with length greater than {Options.MaxBulkStringLength}...");
                    elements = Filter(elements, x => x.Type == DataType.BulkString && x.Length > Options.MaxBulkStringLength, out var removeCount);
                    Console.Error.WriteLine(" done.");
                    Console.Error.WriteLine($"{removeCount} bulk string(s) removed.");
                }

                if (Options.NoArrays)
                {
                    Console.Error.Write("Filtering arrays...");
                    elements = Filter(elements, x => x.Type == DataType.Array, out var removeCount);
                    Console.Error.WriteLine(" done.");
                    Console.Error.WriteLine($"{removeCount} array(s) removed.");
                }
                if (Options.MinArrayLength != null)
                {
                    Console.Error.Write($"Filtering arrays with length less than {Options.MinArrayLength}...");
                    elements = Filter(elements, x => x.Type == DataType.Array && x.Length < Options.MinArrayLength, out var removeCount);
                    Console.Error.WriteLine(" done.");
                    Console.Error.WriteLine($"{removeCount} array(s) removed.");
                }
                if (Options.MaxArrayLength != null)
                {
                    Console.Error.Write($"Filtering arrays with length greater than {Options.MaxArrayLength}...");
                    elements = Filter(elements, x => x.Type == DataType.Array && x.Length > Options.MaxArrayLength, out var removeCount);
                    Console.Error.WriteLine(" done.");
                    Console.Error.WriteLine($"{removeCount} array(s) removed.");
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
