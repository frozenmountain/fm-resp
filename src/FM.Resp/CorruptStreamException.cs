using System;

namespace FM.Resp
{
    internal class CorruptStreamException : Exception
    {
        public CorruptStreamException(string message)
            : base(message)
        { }
    }
}
