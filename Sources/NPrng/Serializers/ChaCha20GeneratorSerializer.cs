using NPrng.Generators;
using System;
using System.IO;

namespace NPrng.Serializers
{
    public sealed class ChaCha20GeneratorSerializer : AbstractPseudoRandomGeneratorSerializer<ChaCha20Generator>
    {
        private const int BufferSize = 16 * sizeof(uint);

        /// <inheritdoc/>
        public override int GetExpectedBufferSize(ChaCha20Generator generator) => BufferSize;

        /// <inheritdoc/>
        public override ChaCha20Generator ReadFromBuffer(ArraySegment<byte> buffer, out int read)
        {
            var key = new uint[16];
            for (var i = 0; i < 8; i++)
            {
                buffer = SerializationHelpers.ReadFromBuffer(buffer, out var value);
                key[2 * i + 1] = (uint)(value & 0xffffffff);
                key[2 * i] = (uint)((value >> 32) & 0xffffffff);
            }
            read = BufferSize;
            return new ChaCha20Generator(key);
        }

        /// <inheritdoc/>
        public override ChaCha20Generator ReadFromStream(Stream source)
        {
            var key = new uint[16];
            for (var i = 0; i < 8; i++)
            {
                SerializationHelpers.ReadFromStream(source, out var value);
                key[2 * i + 1] = (uint)(value & 0xffffffff);
                key[2 * i] = (uint)((value >> 32) & 0xffffffff);
            }
            return new ChaCha20Generator(key);
        }

        /// <inheritdoc/>
        public override int WriteToBuffer(ChaCha20Generator generator, ArraySegment<byte> buffer)
        {
            var key = generator.Key;
            for (var i = 0; i < 8; i++)
            {
                var no = ((ulong)key[2 * i] << 32) + key[2 * i + 1];
                buffer = SerializationHelpers.WriteToBuffer(no, buffer);
            }
            return BufferSize;
        }

        /// <inheritdoc/>
        public override void WriteToStream(ChaCha20Generator generator, Stream target)
        {
            var key = generator.Key;
            for (var i = 0; i < 8; i++)
            {
                var no = ((ulong)key[2 * i] << 32) + key[2 * i + 1];
                SerializationHelpers.WriteToStream(no, target);
            }
        }
    }
}
