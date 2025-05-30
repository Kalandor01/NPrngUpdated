using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NPrng
{
    internal static class SerializationHelpers
    {
        private static readonly ThreadLocal<byte[]> ReadBuffer
            = new(() => new byte[sizeof(long)]);

        private static readonly ThreadLocal<byte[]> WriteBuffer
            = new(() => new byte[sizeof(long)]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegment<byte> WriteToBuffer(ulong value, ArraySegment<byte> buffer)
        {
            var array = buffer.Array!;
            var offset = buffer.Offset;

            for (var i = sizeof(long) - 1; i >= 0; i--)
            {
                array[offset + i] = (byte)(value & 0xFF);
                value >>= 8;
            }

            return new ArraySegment<byte>(array, offset + sizeof(ulong), buffer.Count - sizeof(ulong));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteToStream(ulong value, Stream stream)
        {
            var buffer = WriteBuffer.Value!;
            WriteToBuffer(value, new ArraySegment<byte>(buffer));
            stream.Write(buffer, 0, sizeof(ulong));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegment<byte> ReadFromBuffer(ArraySegment<byte> buffer, out ulong value)
        {
            var array = buffer.Array!;
            var offset = buffer.Offset;

            value = 0;
            for (var i = 0; i < sizeof(ulong); i++)
            {
                value <<= 8;
                value += array[offset + i];
            }

            return new ArraySegment<byte>(array, offset + sizeof(ulong), buffer.Count - sizeof(ulong));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReadFromStream(Stream stream, out ulong value)
        {
            var buffer = ReadBuffer.Value!;
            var missing = sizeof(ulong);
            var offset = 0;
            while (missing > 0)
            {
                var justRead = stream.Read(buffer, offset, missing-offset);
                missing -= justRead;
                offset += justRead;
            }
            ReadFromBuffer(new ArraySegment<byte>(buffer), out value);
        }
    }
}
