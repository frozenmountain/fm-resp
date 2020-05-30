using CommandLine;

namespace FM.Resp
{
    [Verb("export", HelpText = "Exports a RESP stream.")]
    class ExportOptions
    {
        [Option('i', "input", Required = true, HelpText = "The input file path.")]
        public string Input { get; set; }

        [Option("input-offset", Required = false, Default = 0, HelpText = "The offset into the input file at which to start reading.")]
        public int InputOffset { get; set; }
    }
}
