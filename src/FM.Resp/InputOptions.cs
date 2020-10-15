using CommandLine;
using System.IO;

namespace FM.Resp
{
    class InputOptions
    {
        [Option('i', "input", Required = true, HelpText = "The input file path.")]
        public string Input { get; set; }

        [Option("input-offset", Default = 0, HelpText = "The input file offset.")]
        public int InputOffset { get; set; }

        public virtual void Validate()
        {
            // file must exist
            if (!File.Exists(Input))
            {
                throw new ValidateOptionsException($"Input file does not exist: {Input}");
            }

            // offset must be non-negative
            if (InputOffset < 0)
            {
                throw new ValidateOptionsException($"Input file offset must be a non-negative integer: {InputOffset}");
            }
        }
    }
}
