using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FM.Resp
{
    class Exporter : OutputVerb<ExportOptions>
    {
        public Exporter(ExportOptions options)
            : base(options)
        { }

        protected override async Task<IEnumerable<Element>> ProcessStreamIntoElements(Stream stream)
        {
            // create the parser
            var parser = new Parser(stream);

            // parse the stream
            var result = await parser.Parse();

            return result.Elements.Where(x => x.Type != DataType.EndOfStream);
        }
    }
}
