namespace FM.Resp
{
    class Element
    {
        public DataType Type { get; }
        public object Value { get; }
        public int Index { get; }

        public Element(DataType type, object value, int index)
        {
            Type = type;
            Value = value;
            Index = index;
        }

        public override string ToString()
        {
            if (Type == DataType.EndOfStream)
            {
                return $"[{Index}] {Type}";
            }
            if (Type == DataType.Array)
            {
                var value = Value as Element[];
                if (value == null)
                {
                    return $"[{Index}] null ({Type})";
                }
                return $"[{Index}] {value.Length} elements ({Type})";
            }
            return $"[{Index}] {Value} ({Type})";
        }
    }
}
