using System;

namespace FM.Resp
{
    public class ValidateOptionsException : Exception
    {
        public ValidateOptionsException(string message)
            : base(message)
        { }
    }
}
