using System;
using System.IO;
using System.Threading.Tasks;

namespace FM.Resp
{
    abstract class InputVerb<TOptions>
        where TOptions : InputOptions
    {
        public TOptions Options { get; private set; }

        protected InputVerb(TOptions options)
        {
            Options = options;
        }

        public async Task<int> Run()
        {
            // validate the options
            try
            {
                Options.Validate();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 1;
            }

            // yield the UI thread
            await Task.Yield();

            // open the file
            using var stream = File.Open(Options.Input, FileMode.Open, FileAccess.Read, FileShare.Read);

            // seek ahead if asked to do so
            if (Options.InputOffset > 0)
            {
                stream.Seek(Options.InputOffset, SeekOrigin.Current);
            }

            // process the stream
            return await ProcessStream(stream).ConfigureAwait(false);
        }

        protected abstract Task<int> ProcessStream(Stream stream);
    }
}
