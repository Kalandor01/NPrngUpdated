using NPrng.Generators;
using System;
using System.IO;

namespace NPrng.Serializers
{
    public sealed class LinearCongruentGeneratorSerializer : AbstractPseudoRandomGeneratorSerializer<LinearCongruentGenerator>
    {
        private const int BufferSize = sizeof(ulong);

        /// <inheritdoc/>
        public override int GetExpectedBufferSize(LinearCongruentGenerator generator) => sizeof(ulong);

        /// <inheritdoc/>
        public override LinearCongruentGenerator ReadFromBuffer(ArraySegment<byte> buffer, out int read)
        {
            SerializationHelpers.ReadFromBuffer(buffer, out var state);
            read = BufferSize;
            return new LinearCongruentGenerator(state);
        }

        /// <inheritdoc/>
        public override LinearCongruentGenerator ReadFromStream(Stream source)
        {
            SerializationHelpers.ReadFromStream(source, out var state);
            return new LinearCongruentGenerator(state);
        }

        /// <inheritdoc/>
        public override int WriteToBuffer(LinearCongruentGenerator generator, ArraySegment<byte> buffer)
        {
            SerializationHelpers.WriteToBuffer(generator.CurrentState, buffer);
            return BufferSize;
        }

        /// <inheritdoc/>
        public override void WriteToStream(LinearCongruentGenerator generator, Stream target)
        {
            SerializationHelpers.WriteToStream(generator.CurrentState, target);
        }
    }
}
