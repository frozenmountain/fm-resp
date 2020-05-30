namespace FM.Resp
{
    class Element
    {
        public DataType Type { get; }
        public object Value { get; }

        public Element(DataType type, object value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            if (Type == DataType.EndOfStream)
            {
                return $"{Type}";
            }
            if (Type == DataType.Array)
            {
                var value = Value as Element[];
                if (value == null)
                {
                    return $"null ({Type})";
                }
                return $"{value.Length} elements ({Type})";
            }
            return $"{Value} ({Type})";
        }
    }
}
