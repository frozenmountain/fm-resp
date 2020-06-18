using CommandLine;

namespace FM.Resp
{
    [Verb("filter", HelpText = "Filters exported JSON.")]
    class FilterOptions
    {
        [Option('i', "input", Required = true, HelpText = "The input file path.")]
        public string Input { get; set; }

        [Option('o', "output", HelpText = "The output file path. If not set, stdout is used.")]
        public string Output { get; set; }

        [Option('y', HelpText = "Overwrite the output file path, if present.")]
        public bool Overwrite { get; set; }

        [Option("indented", HelpText = "Use indented output formatting.")]
        public bool Indented { get; set; }

        [Option("no-simple-strings", HelpText = "Filter top-level simple strings.")]
        public bool NoSimpleStrings { get; set; }

        [Option("no-errors", HelpText = "Filter top-level errors.")]
        public bool NoErrors { get; set; }

        [Option("no-integers", HelpText = "Filter top-level integers.")]
        public bool NoIntegers { get; set; }

        [Option("no-bulk-strings", HelpText = "Filter top-level bulk strings.")]
        public bool NoBulkStrings { get; set; }

        [Option("no-arrays", HelpText = "Filter top-level arrays.")]
        public bool NoArrays { get; set; }

        [Option("no-simple-string-values", HelpText = "Filter top-level simple string values.")]
        public bool NoSimpleStringValues { get; set; }

        [Option("no-error-values", HelpText = "Filter top-level error values.")]
        public bool NoErrorValues { get; set; }

        [Option("no-integer-values", HelpText = "Filter top-level integer values.")]
        public bool NoIntegerValues { get; set; }

        [Option("no-bulk-string-values", HelpText = "Filter top-level bulk string values.")]
        public bool NoBulkStringValues { get; set; }

        [Option("no-array-values", HelpText = "Filter top-level array values.")]
        public bool NoArrayValues { get; set; }

        [Option("min-simple-string-length", HelpText = "Minimum top-level simple string length (inclusive).")]
        public int? MinSimpleStringLength { get; set; }

        [Option("min-error-length", HelpText = "Minimum top-level error length (inclusive).")]
        public int? MinErrorLength { get; set; }

        [Option("min-integer", HelpText = "Minimum top-level integer (inclusive).")]
        public int? MinInteger { get; set; }

        [Option("min-integer-length", HelpText = "Minimum top-level integer length (inclusive).")]
        public int? MinIntegerLength { get; set; }

        [Option("min-bulk-string-length", HelpText = "Minimum top-level bulk string length (inclusive).")]
        public int? MinBulkStringLength { get; set; }

        [Option("min-array-length", HelpText = "Minimum top-level array length (inclusive).")]
        public int? MinArrayLength { get; set; }

        [Option("max-simple-string-length", HelpText = "Maximum top-level simple string length (inclusive).")]
        public int? MaxSimpleStringLength { get; set; }

        [Option("max-error-length", HelpText = "Maximum top-level error length (inclusive).")]
        public int? MaxErrorLength { get; set; }

        [Option("max-integer", HelpText = "Maximum top-level integer (inclusive).")]
        public int? MaxInteger { get; set; }

        [Option("max-integer-length", HelpText = "Maximum top-level integer length (inclusive).")]
        public int? MaxIntegerLength { get; set; }

        [Option("max-bulk-string-length", HelpText = "Maximum top-level bulk string length (inclusive).")]
        public int? MaxBulkStringLength { get; set; }

        [Option("max-array-length", HelpText = "Maximum top-level array length (inclusive).")]
        public int? MaxArrayLength { get; set; }

        [Option("from-index", HelpText = "The index of the first top-level element to include.")]
        public int? FromIndex { get; set; }

        [Option("to-index", HelpText = "The index of the last top-level element to include.")]
        public int? ToIndex { get; set; }
    }
}
