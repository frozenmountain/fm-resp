using CommandLine;

namespace FM.Resp
{
    [Verb("export", HelpText = "Exports a RESP stream.")]
    class ExportOptions
    {
        [Option('i', "input", Required = true, HelpText = "The input file path.")]
        public string Input { get; set; }

        [Option('o', "output", Required = true, HelpText = "The output file path.")]
        public string Output { get; set; }

        [Option('f', "format", Required = false, Default = FileFormat.Json, HelpText = "The output file format.")]
        public FileFormat OutputFormat { get; set; }

        [Option('y', Required = false, Default = false, HelpText = "Overwrite the output file path, if present.")]
        public bool Overwrite { get; set; }

        [Option("indented", Required = false, Default = false, HelpText = "Use indented output formatting.")]
        public bool Indented { get; set; }
    }
}
