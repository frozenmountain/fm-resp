using CommandLine;

namespace FM.Resp
{
    [Verb("filter", HelpText = "Filters exported JSON.")]
    class FilterOptions
    {
        [Option('i', "input", Required = true, HelpText = "The input file path.")]
        public string Input { get; set; }

        [Option('o', "output", Required = false, HelpText = "The output file path. If not set, output is directed to the stdout.")]
        public string Output { get; set; }

        [Option('y', Required = false, Default = false, HelpText = "Overwrite the output file path, if present.")]
        public bool Overwrite { get; set; }

        [Option("indented", Required = false, Default = false, HelpText = "Use indented output formatting.")]
        public bool Indented { get; set; }

        [Option("no-simple-strings", Required = false, Default = false, HelpText = "Filter top-level simple strings.")]
        public bool NoSimpleStrings { get; set; }

        [Option("no-errors", Required = false, Default = false, HelpText = "Filter top-level errors.")]
        public bool NoErrors { get; set; }

        [Option("no-integers", Required = false, Default = false, HelpText = "Filter top-level integers.")]
        public bool NoIntegers { get; set; }

        [Option("no-bulk-strings", Required = false, Default = false, HelpText = "Filter top-level bulk strings.")]
        public bool NoBulkStrings { get; set; }

        [Option("no-arrays", Required = false, Default = false, HelpText = "Filter top-level arrays.")]
        public bool NoArrays { get; set; }

        [Option("no-simple-string-values", Required = false, Default = false, HelpText = "Filter top-level simple string values.")]
        public bool NoSimpleStringValues { get; set; }

        [Option("no-error-values", Required = false, Default = false, HelpText = "Filter top-level error values.")]
        public bool NoErrorValues { get; set; }

        [Option("no-integer-values", Required = false, Default = false, HelpText = "Filter top-level integer values.")]
        public bool NoIntegerValues { get; set; }

        [Option("no-bulk-string-values", Required = false, Default = false, HelpText = "Filter top-level bulk string values.")]
        public bool NoBulkStringValues { get; set; }

        [Option("no-array-values", Required = false, Default = false, HelpText = "Filter top-level array values.")]
        public bool NoArrayValues { get; set; }

        [Option("min-simple-string-length", Required = false, HelpText = "Minimum top-level simple string length (inclusive).")]
        public int? MinSimpleStringLength { get; set; }

        [Option("min-error-length", Required = false, HelpText = "Minimum top-level error length (inclusive).")]
        public int? MinErrorLength { get; set; }

        [Option("min-integer", Required = false, HelpText = "Minimum top-level integer (inclusive).")]
        public int? MinInteger { get; set; }

        [Option("min-integer-length", Required = false, HelpText = "Minimum top-level integer length (inclusive).")]
        public int? MinIntegerLength { get; set; }

        [Option("min-bulk-string-length", Required = false, HelpText = "Minimum top-level bulk string length (inclusive).")]
        public int? MinBulkStringLength { get; set; }

        [Option("min-array-length", Required = false, HelpText = "Minimum top-level array length (inclusive).")]
        public int? MinArrayLength { get; set; }

        [Option("max-simple-string-length", Required = false, HelpText = "Maximum top-level simple string length (inclusive).")]
        public int? MaxSimpleStringLength { get; set; }

        [Option("max-error-length", Required = false, HelpText = "Maximum top-level error length (inclusive).")]
        public int? MaxErrorLength { get; set; }

        [Option("max-integer", Required = false, HelpText = "Maximum top-level integer (inclusive).")]
        public int? MaxInteger { get; set; }

        [Option("max-integer-length", Required = false, HelpText = "Maximum top-level integer length (inclusive).")]
        public int? MaxIntegerLength { get; set; }

        [Option("max-bulk-string-length", Required = false, HelpText = "Maximum top-level bulk string length (inclusive).")]
        public int? MaxBulkStringLength { get; set; }

        [Option("max-array-length", Required = false, HelpText = "Maximum top-level array length (inclusive).")]
        public int? MaxArrayLength { get; set; }

        [Option("from-index", Required = false, HelpText = "The index of the first top-level element to include.")]
        public int? FromIndex { get; set; }

        [Option("to-index", Required = false, HelpText = "The index of the last top-level element to include.")]
        public int? ToIndex { get; set; }
    }
}
