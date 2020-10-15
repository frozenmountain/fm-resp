using CommandLine;

namespace FM.Resp
{
    [Verb("export", HelpText = "Exports an RESP stream to JSON.")]
    class ExportOptions : OutputOptions
    { }
}
