using Newtonsoft.Json;

namespace FM.Resp
{
    class Element
    {
        public DataType Type { get; }
        public object Value { get; }
        public int Index { get; }
        public int Length { get; }

        public Element(DataType type, object value, int index, int length)
        {
            Type = type;
            Value = value;
            Index = index;
            Length = length;
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

        [JsonIgnore]
        public bool NoValue { get; set; }

        public bool ShouldSerializeValue()
        {
            return !NoValue;
        }
    }
}
