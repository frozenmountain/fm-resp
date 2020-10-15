﻿using System;
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
        private int _NextIndex;

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
                    DataType.EndOfStream => Task.FromResult(new Element(type, null, _NextIndex++, -1)),
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
                    _ => throw new CorruptStreamException($"Stream is corrupt. Unexpected character '{c}' while reading type at position {_StreamPosition}."),
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
                    throw new CorruptStreamException($"Stream is corrupt. Unexpected character '{b}' while reading second byte of {label} termination at position {_StreamPosition}.");
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
            var value = ReadAscii("simple string");
            return new Element(DataType.SimpleString, value, _NextIndex++, value.Length);
        }

        private Element ReadError()
        {
            var value = ReadAscii("error");
            return new Element(DataType.Error, value, _NextIndex++, value.Length);
        }

        private Element ReadInteger()
        {
            var value = ReadAscii("integer");
            return new Element(DataType.Integer, int.Parse(value), _NextIndex++, value.Length);
        }

        private async Task<Element> ReadBulkStringAsync()
        {
            var lengthPosition = _StreamPosition;
            var lengthAscii = ReadAscii("bulk string length");
            if (!int.TryParse(lengthAscii, out var length))
            {
                throw new CorruptStreamException($"Stream is corrupt. Could not parse integer '{lengthAscii}' at position {lengthPosition}.");
            }
            if (length == -1)
            {
                return new Element(DataType.BulkString, null, _NextIndex++, -1);
            }

            var bytes = new byte[length];
            var readLength = await _Stream.ReadAsync(bytes);
            if (readLength != bytes.Length)
            {
                throw new CorruptStreamException($"Stream is corrupt. Read {readLength} bytes but expected {bytes.Length} bytes at position {_StreamPosition}.");
            }
            _StreamPosition += bytes.Length;

            var c = (char)_Stream.ReadByte();

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
                    var value = Encoding.UTF8.GetString(bytes);
                    return new Element(DataType.BulkString, value, _NextIndex++, value.Length);
                }
                throw new CorruptStreamException($"Stream is corrupt. Unexpected character '{c}' while reading second byte of bulk string termination at position {_StreamPosition}.");
            }
            else if (c == '\n')
            {
                _StreamPosition++;
                var value = Encoding.UTF8.GetString(bytes);
                return new Element(DataType.BulkString, value, _NextIndex++, value.Length);
            }
            else
            {
                throw new CorruptStreamException($"Stream is corrupt. Unexpected character '{c}' while reading first byte of bulk string termination at position {_StreamPosition}.");
            }
        }

        private async Task<Element> ReadArrayAsync()
        {
            var count = int.Parse(ReadAscii("array size"));
            if (count == -1)
            {
                return new Element(DataType.Array, null, _NextIndex++, -1);
            }

            var currentNextIndex = _NextIndex;
            _NextIndex = 0;
            var array = new Element[count];
            for (var i = 0; i < count; i++)
            {
                array[i] = await ReadAsync().ConfigureAwait(false);
            }
            _NextIndex = currentNextIndex;
            return new Element(DataType.Array, array, _NextIndex++, count);
        }
    }
}
