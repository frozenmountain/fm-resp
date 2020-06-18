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
        private long _StreamPosition;
        private int _NextIndex = 0;

        public Reader(Stream stream)
        {
            _Stream = stream;
            _StreamPosition = stream.Position;
            _NextIndex = 0;
        }

        public Element Read()
        {
            return Task.Run(() => ReadAsync()).GetAwaiter().GetResult();
        }

        public Task<Element> ReadAsync()
        {
            var type = ReadType();
            try
            {
                return type switch
                {
                    DataType.EndOfStream => Task.FromResult(new Element(type, null, _NextIndex++)),
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
            return new Element(DataType.SimpleString, ReadAscii("simple string"), _NextIndex++);
        }

        private Element ReadError()
        {
            return new Element(DataType.Error, ReadAscii("error"), _NextIndex++);
        }

        private Element ReadInteger()
        {
            return new Element(DataType.Integer, int.Parse(ReadAscii("integer")), _NextIndex++);
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
                return new Element(DataType.BulkString, null, _NextIndex++);
            }

            var bytes = new byte[length];
            await _Stream.ReadAsync(bytes);
            _StreamPosition += bytes.Length;

            var c = (char)_Stream.ReadByte();
            
            // special case where +OK gets mixed into the end of a large payload
            if (c == '+' && _Stream.ReadByte() == 'O' && _Stream.ReadByte() == 'K')
            {
                var x = (char)_Stream.ReadByte();
                if (x == '\r')
                {
                    if (_Stream.ReadByte() == '\n')
                    {
                        c = (char)_Stream.ReadByte();
                        _StreamPosition += 5;
                    }
                    else
                    {
                        throw new Exception($"Stream is corrupt. Unexpected character '{c}' while reading first byte of bulk string termination at position {_StreamPosition}.");
                    }
                }
                else if (x == '\n')
                {
                    c = (char)_Stream.ReadByte();
                    _StreamPosition += 4;
                }
                else
                {
                    throw new Exception($"Stream is corrupt. Unexpected character '{c}' while reading first byte of bulk string termination at position {_StreamPosition}.");
                }
            }

            // special case where +OK gets mixed into the middle of a large payload
            if (c != '\r' && c != '\n')
            {
                for (var i = 0; i < bytes.Length; i++)
                {
                    if (i + 2 < bytes.Length && bytes[i] == '+' && bytes[i + 1] == 'O' && bytes[i + 2] == 'K')
                    {
                        var spliceLength = -1;
                        if (i + 4 < bytes.Length && bytes[i + 3] == '\r' && bytes[i + 4] == '\n')
                        {
                            spliceLength = 5;
                        }
                        else if (i + 3 < bytes.Length && bytes[i + 3] == '\n')
                        {
                            spliceLength = 4;
                        }

                        if (spliceLength != -1)
                        {
                            Buffer.BlockCopy(bytes, i + spliceLength, bytes, i, bytes.Length - i - spliceLength);

                            var newBytes = new byte[spliceLength - 1];
                            await _Stream.ReadAsync(newBytes);
                            _StreamPosition += newBytes.Length;

                            bytes[bytes.Length - newBytes.Length] = (byte)c;
                            Buffer.BlockCopy(newBytes, 0, bytes, bytes.Length - newBytes.Length, newBytes.Length);

                            c = (char)_Stream.ReadByte();

                            i--;
                        }
                    }
                }
            }

            if (c != '\r' && c != '\n')
            {
                char x = '\0';
                var shiftLength = -1;
                if (bytes.Length > 0 && bytes[0] == '\n')
                {
                    x = (char)_Stream.ReadByte();
                    if (x == '\n')
                    {
                        shiftLength = 1;
                    }
                }
                else if (bytes.Length > 1 && bytes[0] == '\r' && bytes[1] == '\n')
                {
                    x = (char)_Stream.ReadByte();
                    if (x == '\r')
                    {
                        shiftLength = 2;
                    }
                }

                if (shiftLength != -1)
                {
                    Buffer.BlockCopy(bytes, 1, bytes, 0, bytes.Length - 1);
                    bytes[bytes.Length - 1] = (byte)c;

                    c = x;
                }
            }

            if (c == '\r')
            {
                _StreamPosition++;
                if (_Stream.ReadByte() == '\n')
                {
                    _StreamPosition++;
                    return new Element(DataType.BulkString, Encoding.UTF8.GetString(bytes), _NextIndex++);
                }
                throw new Exception($"Stream is corrupt. Unexpected character '{c}' while reading second byte of bulk string termination at position {_StreamPosition}.");
            }
            else if (c == '\n')
            {
                _StreamPosition++;
                return new Element(DataType.BulkString, Encoding.UTF8.GetString(bytes), _NextIndex++);
            }
            else
            {
                throw new Exception($"Stream is corrupt. Unexpected character '{c}' while reading first byte of bulk string termination at position {_StreamPosition}.");
            }
        }

        private async Task<Element> ReadArrayAsync()
        {
            var count = int.Parse(ReadAscii("array size"));
            if (count == -1)
            {
                return new Element(DataType.Array, null, _NextIndex++);
            }

            var currentNextIndex = _NextIndex;
            _NextIndex = 0;
            var array = new Element[count];
            for (var i = 0; i < count; i++)
            {
                array[i] = await ReadAsync();
            }
            _NextIndex = currentNextIndex;
            return new Element(DataType.Array, array, _NextIndex++);
        }
    }
}
