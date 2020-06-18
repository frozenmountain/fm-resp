using CommandLine;

namespace FM.Resp
{
    [Verb("export", HelpText = "Exports an RESP stream to JSON.")]
    class ExportOptions
    {
        [Option('i', "input", Required = true, HelpText = "The input file path.")]
        public string Input { get; set; }

        [Option('o', "output", Required = false, HelpText = "The output file path. If not set, output is directed to stdout.")]
        public string Output { get; set; }

        [Option('y', Required = false, Default = false, HelpText = "Overwrite the output file path, if present.")]
        public bool Overwrite { get; set; }

        [Option("indented", Required = false, Default = false, HelpText = "Use indented output formatting.")]
        public bool Indented { get; set; }
    }
}
