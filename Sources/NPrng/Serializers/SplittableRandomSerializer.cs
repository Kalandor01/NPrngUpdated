using NPrng.Generators;
using System;
using System.IO;

namespace NPrng.Serializers
{
    public sealed class SplittableRandomSerializer : AbstractPseudoRandomGeneratorSerializer<SplittableRandom>
    {
        private const int BufferSize = 2 * sizeof(ulong);

        /// <inheritdoc/>
        public override int GetExpectedBufferSize(SplittableRandom generator) => BufferSize;

        /// <inheritdoc/>
        public override SplittableRandom ReadFromBuffer(ArraySegment<byte> buffer, out int read)
        {
            buffer = SerializationHelpers.ReadFromBuffer(buffer, out var seed);
            buffer = SerializationHelpers.ReadFromBuffer(buffer, out var gamma);
            read = BufferSize;
            return new SplittableRandom(seed, gamma);
        }

        /// <inheritdoc/>
        public override SplittableRandom ReadFromStream(Stream source)
        {
            SerializationHelpers.ReadFromStream(source, out var seed);
            SerializationHelpers.ReadFromStream(source, out var gamma);
            return new SplittableRandom(seed, gamma);
        }

        /// <inheritdoc/>
        public override int WriteToBuffer(SplittableRandom generator, ArraySegment<byte> buffer)
        {
            buffer = SerializationHelpers.WriteToBuffer(generator.Seed, buffer);
            buffer = SerializationHelpers.WriteToBuffer(generator.Gamma, buffer);
            return BufferSize;
        }

        /// <inheritdoc/>
        public override void WriteToStream(SplittableRandom generator, Stream target)
        {
            SerializationHelpers.WriteToStream(generator.Seed, target);
            SerializationHelpers.WriteToStream(generator.Gamma, target);
        }
    }
}
