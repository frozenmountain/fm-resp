using CommandLine;

namespace FM.Resp
{
    [Verb("analyze", HelpText = "Analyzes an RESP stream.")]
    class AnalyzeOptions
    {
        [Option('i', "input", Required = true, HelpText = "The input file path.")]
        public string Input { get; set; }

        [Option("input-offset", Default = 0, HelpText = "The input file offset.")]
        public int InputOffset { get; set; }
    }
}
