using System;

namespace FM.Resp
{
    public class CorruptStreamException : Exception
    {
        public CorruptStreamException(string message)
            : base(message)
        { }
    }
}
