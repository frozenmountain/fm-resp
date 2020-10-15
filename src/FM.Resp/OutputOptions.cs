using CommandLine;
using System.IO;

namespace FM.Resp
{
    class OutputOptions : InputOptions
    {
        [Option('o', "output", HelpText = "The output file path. If not set, stdout is used.")]
        public string Output { get; set; }

        [Option('y', HelpText = "Overwrite the output file path, if present.")]
        public bool Overwrite { get; set; }

        [Option("indented", HelpText = "Use indented output formatting.")]
        public bool Indented { get; set; }

        public override void Validate()
        {
            base.Validate();

            if (Output != null && !Overwrite && File.Exists(Output))
            {
                throw new ValidateOptionsException($"Output file already exists (use -y to overwrite): {Output}");
            }
        }
    }
}
