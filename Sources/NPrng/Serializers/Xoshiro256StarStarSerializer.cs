using NPrng.Generators;
using System;
using System.IO;

namespace NPrng.Serializers
{
    public sealed class Xoshiro256StarStarSerializer : AbstractPseudoRandomGeneratorSerializer<Xoshiro256StarStar>
    {
        private const int BufferSize = 4 * sizeof(ulong);

        /// <inheritdoc/>
        public override int GetExpectedBufferSize(Xoshiro256StarStar generator) => BufferSize;

        /// <inheritdoc/>
        public override Xoshiro256StarStar ReadFromBuffer(ArraySegment<byte> buffer, out int read)
        {
            buffer = SerializationHelpers.ReadFromBuffer(buffer, out var s0);
            buffer = SerializationHelpers.ReadFromBuffer(buffer, out var s1);
            buffer = SerializationHelpers.ReadFromBuffer(buffer, out var s2);
            buffer = SerializationHelpers.ReadFromBuffer(buffer, out var s3);
            read = BufferSize;
            return new Xoshiro256StarStar(s0, s1, s2, s3);
        }

        /// <inheritdoc/>
        public override Xoshiro256StarStar ReadFromStream(Stream source)
        {
            SerializationHelpers.ReadFromStream(source, out var s0);
            SerializationHelpers.ReadFromStream(source, out var s1);
            SerializationHelpers.ReadFromStream(source, out var s2);
            SerializationHelpers.ReadFromStream(source, out var s3);
            return new Xoshiro256StarStar(s0, s1, s2, s3);
        }

        /// <inheritdoc/>
        public override int WriteToBuffer(Xoshiro256StarStar generator, ArraySegment<byte> buffer)
        {
            buffer = SerializationHelpers.WriteToBuffer(generator.S0, buffer);
            buffer = SerializationHelpers.WriteToBuffer(generator.S1, buffer);
            buffer = SerializationHelpers.WriteToBuffer(generator.S2, buffer);
            buffer = SerializationHelpers.WriteToBuffer(generator.S3, buffer);
            return BufferSize;
        }

        /// <inheritdoc/>
        public override void WriteToStream(Xoshiro256StarStar generator, Stream target)
        {
            SerializationHelpers.WriteToStream(generator.S0, target);
            SerializationHelpers.WriteToStream(generator.S1, target);
            SerializationHelpers.WriteToStream(generator.S2, target);
            SerializationHelpers.WriteToStream(generator.S3, target);
        }
    }
}
