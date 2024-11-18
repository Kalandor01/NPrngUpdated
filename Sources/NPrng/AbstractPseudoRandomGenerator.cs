using System;
using System.Runtime.CompilerServices;

namespace NPrng
{
    public abstract class AbstractPseudoRandomGenerator : IPseudoRandomGenerator
    {
        /// <inheritdoc/>
        public abstract long Generate();

        /// <inheritdoc/>
        public double GenerateDouble()
        {
            const long absMask = ~(unchecked((long)(1UL << 63)) >> 2);

            // We replace first three bits with 0 via absMask, and then divide
            // by absMask+1 to ensure [0,1) interval.
            var generated = Generate() & absMask;
            return (double)generated / (absMask + 1);
        }

        /// <inheritdoc/>
        public virtual long GenerateInRange(long lower, long upper)
        {
            if (lower > upper)
            {
                throw new ArgumentException($"{nameof(lower)} cannot be greater than {nameof(upper)}.");
            }
            else if (lower == upper)
            {
                return lower;
            }

            return lower + InternalGenerateInRange(upper - lower);
        }

        /// <inheritdoc/>
        public virtual long GenerateLessOrEqualTo(long range)
        {
            if (range < 0)
            {
                throw new ArgumentException($"{nameof(range)} has to be nonnegative.");
            }
            else if (range == 0)
            {
                return 0;
            }

            return InternalGenerateInRange(range);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long InternalGenerateInRange(long range)
        {
            var uRange = (ulong)range;
            var bitCount = NumericHelpers.CountBits(uRange);
            var mask = (ulong)((1 << bitCount) - 1);

            ulong result;
            do
            {
                result = (ulong)Generate() & mask;
            }
            while (result > uRange);

            return (long)result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long FastAbs(long value)
        {
            var shifted = value >> 63;
            return (value + shifted) ^ shifted;
        }
    }
}
