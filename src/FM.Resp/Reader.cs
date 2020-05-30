using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FM.Resp
{
    class Reader
    {
        public event Action<DataType> OnReadType;

        private readonly Stream _Stream;
        private long _StreamPosition = 0;

        public Reader(Stream stream)
        {
            _Stream = stream;
            _StreamPosition = stream.Position;
        }

        public Element Read()
        {
            var type = ReadType();
            try
            {
                return type switch
                {
                    DataType.EndOfStream => new Element(type, null),
                    DataType.SimpleString => ReadSimpleString(),
                    DataType.Error => ReadError(),
                    DataType.Integer => ReadInteger(),
                    DataType.BulkString => ReadBulkString(),
                    DataType.Array => ReadArray(),
                    _ => throw new Exception($"Unsupported type '{type}'."),
                };
            }
            finally
            {
                OnReadType?.Invoke(type);
            }
        }

        public Task<Element> ReadAsync()
        {
            var type = ReadType();
            try
            {
                return type switch
                {
                    DataType.EndOfStream => Task.FromResult(new Element(type, null)),
                    DataType.SimpleString => Task.FromResult(ReadSimpleString()),
                    DataType.Error => Task.FromResult(ReadError()),
                    DataType.Integer => Task.FromResult(ReadInteger()),
                    DataType.BulkString => ReadBulkStringAsync(),
                    DataType.Array => ReadArrayAsync(),
                    _ => throw new Exception($"Unsupported type '{type}'."),
                };
            }
            finally
            {
                OnReadType?.Invoke(type);
            }
        }

        private DataType ReadType()
        {
            var b = _Stream.ReadByte();
            if (b == -1)
            {
                return DataType.EndOfStream;
            }
            try
            {
                var c = (char)b;
                if (c == '\r' || c == '\n')
                {
                    // ignore extra newline character
                    return ReadType();
                }
                return c switch
                {
                    '+' => DataType.SimpleString,
                    '-' => DataType.Error,
                    ':' => DataType.Integer,
                    '$' => DataType.BulkString,
                    '*' => DataType.Array,
                    _ => throw new Exception($"Stream is corrupt. Unexpected character '{c}' while reading type at position {_StreamPosition}."),
                };
            }
            finally
            {
                _StreamPosition++;
            }
        }

        private string ReadAscii(string label)
        {
            var bytes = new List<byte>();
            while (true)
            {
                var b = (byte)_Stream.ReadByte();
                if (b == '\r')
                {
                    _StreamPosition++;
                    if (_Stream.ReadByte() == '\n')
                    {
                        _StreamPosition++;
                        return Encoding.ASCII.GetString(bytes.ToArray());
                    }
                    throw new Exception($"Stream is corrupt. Unexpected character '{b}' while reading second byte of {label} termination at position {_StreamPosition}.");
                }
                else if (b == '\n')
                {
                    _StreamPosition++;
                    return Encoding.ASCII.GetString(bytes.ToArray());
                }
                else
                {
                    _StreamPosition++;
                    bytes.Add(b);
                }
            }
        }

        private Element ReadSimpleString()
        {
            return new Element(DataType.SimpleString, ReadAscii("simple string"));
        }

        private Element ReadError()
        {
            return new Element(DataType.Error, ReadAscii("error"));
        }

        private Element ReadInteger()
        {
            return new Element(DataType.Integer, int.Parse(ReadAscii("integer")));
        }

        private Element ReadBulkString()
        {
            var length = int.Parse(ReadAscii("bulk string length"));
            if (length == -1)
            {
                return new Element(DataType.BulkString, null);
            }

            var bytes = new byte[length];
            _Stream.Read(bytes);
            _StreamPosition += length;

            var c = (char)_Stream.ReadByte();
            if (c == '\r')
            {
                _StreamPosition++;
                if (_Stream.ReadByte() == '\n')
                {
                    _StreamPosition++;
                    return new Element(DataType.BulkString, Encoding.UTF8.GetString(bytes));
                }
                throw new Exception($"Stream is corrupt. Unexpected character '{c}' while reading second byte of bulk string termination at position {_StreamPosition}.");
            }
            else if (c == '\n')
            {
                _StreamPosition++;
                return new Element(DataType.BulkString, Encoding.UTF8.GetString(bytes));
            }
            else
            {
                throw new Exception($"Stream is corrupt. Unexpected character '{c}' while reading first byte of bulk string termination at position {_StreamPosition}.");
            }
        }

        private async Task<Element> ReadBulkStringAsync()
        {
            var lengthPosition = _StreamPosition;
            var lengthAscii = ReadAscii("bulk string length");
            if (!int.TryParse(lengthAscii, out var length))
            {
                throw new Exception($"Stream is corrupt. Could not parse integer '{lengthAscii}' at position {lengthPosition}.");
            }
            if (length == -1)
            {
                return new Element(DataType.BulkString, null);
            }

            var bytes = new byte[length];
            await _Stream.ReadAsync(bytes);
            _StreamPosition += length;

            var c = (char)_Stream.ReadByte();
            if (c == '\r')
            {
                _StreamPosition++;
                if (_Stream.ReadByte() == '\n')
                {
                    _StreamPosition++;
                    return new Element(DataType.BulkString, Encoding.UTF8.GetString(bytes));
                }
                throw new Exception($"Stream is corrupt. Unexpected character '{c}' while reading second byte of bulk string termination at position {_StreamPosition}.");
            }
            else if (c == '\n')
            {
                _StreamPosition++;
                return new Element(DataType.BulkString, Encoding.UTF8.GetString(bytes));
            }
            else
            {
                throw new Exception($"Stream is corrupt. Unexpected character '{c}' while reading first byte of bulk string termination at position {_StreamPosition}.");
            }
        }

        private Element ReadArray()
        {
            var count = int.Parse(ReadAscii("array size"));
            if (count == -1)
            {
                return new Element(DataType.Array, null);
            }
            var array = new Element[count];
            for (var i = 0; i < count; i++)
            {
                array[i] = Read();
            }
            return new Element(DataType.Array, array);
        }

        private async Task<Element> ReadArrayAsync()
        {
            var count = int.Parse(ReadAscii("array size"));
            if (count == -1)
            {
                return new Element(DataType.Array, null);
            }
            var array = new Element[count];
            for (var i = 0; i < count; i++)
            {
                array[i] = await ReadAsync();
            }
            return new Element(DataType.Array, array);
        }
    }
}
