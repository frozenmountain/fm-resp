using System;
using System.IO;
using System.Threading.Tasks;

namespace FM.Resp
{
    class Exporter
    {
        public ExportOptions Options { get; private set; }

        public Exporter(ExportOptions options)
        {
            Options = options;
        }

        public async Task<int> Run()
        {
            // sanity check on the file
            if (!File.Exists(Options.Input))
            {
                Console.WriteLine($"File does not exist: {Options.Input}");
                return 1;
            }

            // open the file
            var stream = File.Open(Options.Input, FileMode.Open, FileAccess.Read, FileShare.Read);

            // seek ahead if asked to do so
            if (Options.InputOffset > 0)
            {
                stream.Seek(Options.InputOffset, SeekOrigin.Current);
            }

            // create the parser
            var parser = new Parser(stream);

            // parse the stream
            var result = await parser.Parse();

            //TODO: export

            return 0;
        }
    }
}
