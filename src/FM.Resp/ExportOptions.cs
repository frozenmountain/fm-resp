using CommandLine;

namespace FM.Resp
{
    [Verb("export", HelpText = "Exports an RESP stream to JSON.")]
    class ExportOptions
    {
        [Option('i', "input", Required = true, HelpText = "The input file path.")]
        public string Input { get; set; }

        [Option('o', "output", HelpText = "The output file path. If not set, stdout is used.")]
        public string Output { get; set; }

        [Option('y', HelpText = "Overwrite the output file path, if present.")]
        public bool Overwrite { get; set; }

        [Option("indented", HelpText = "Use indented output formatting.")]
        public bool Indented { get; set; }
    }
}
