using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FM.Resp
{
    class Parser
    {
        private readonly Stream _Stream;

        public Parser(Stream stream)
        {
            _Stream = stream;
        }

        public async Task<ParseResult> Parse()
        {
            var reader = new Reader(_Stream);

            // display progress in a reasonable efficient manner
            var displayingProgress = false;
            reader.OnReadType += (type) =>
            {
                // if we are currently updating the display, ignore this
                if (!displayingProgress)
                {
                    // make sure subsequent display attempt bail out
                    displayingProgress = true;
                    Task.Run(() =>
                    {
                        try
                        {
                            // display the progress
                            DisplayProgress(_Stream, true);
                        }
                        finally
                        {
                            // allow the next read to display again
                            displayingProgress = false;
                        }
                    });
                }
            };

            var result = new ParseResult();

            // initialize collection
            var allElements = new List<Element>();
            var elementsByType = new Dictionary<DataType, List<Element>>();
            foreach (var type in result.Types)
            {
                elementsByType[type] = new List<Element>();
            }

            // start reading
            while (true)
            {
                var element = await reader.ReadAsync();
                result.AddElement(element);
                if (element.Type == DataType.EndOfStream)
                {
                    break;
                }
            }

            // one final update (should be 100%)
            while (displayingProgress)
            {
                await Task.Delay(1);
            }
            DisplayProgress(_Stream, false);

            return result;
        }

        private void DisplayProgress(Stream stream, bool resetCursorPosition)
        {
            var streamPosition = stream.Position;
            var consoleCursorTop = Console.CursorTop;
            var consoleCursorLeft = Console.CursorLeft;
            Console.WriteLine($"Reading stream... {streamPosition} / {stream.Length} ({(float)streamPosition / stream.Length:P})");
            if (resetCursorPosition)
            {
                Console.SetCursorPosition(consoleCursorLeft, consoleCursorTop);
            }
        }
    }
}
