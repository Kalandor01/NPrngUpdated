using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NPrng.Generators
{
    public sealed class SplittableRandom : AbstractPseudoRandomGenerator
    {
        private static long DefaultGen = InitialSeed();
        private const ulong GOLDEN_GAMMA = 0x9e3779b97f4a7c15L;

        private static ulong GetAndAdd(ulong value)
        {
            while (true)
            {
                var defaultGen = Volatile.Read(ref DefaultGen);
                var newValue = unchecked(defaultGen + (long)value);
                if (Interlocked.CompareExchange(ref DefaultGen, newValue, defaultGen) == defaultGen)
                {
                    return (ulong)newValue;
                }
            }
        }

        private static long InitialSeed() => DateTime.UtcNow.ToBinary();
        internal ulong Seed { get; private set; }
        internal ulong Gamma { get; }

        internal SplittableRandom(ulong seed, ulong gamma)
        {
            Seed = seed;
            Gamma = gamma;
        }

        public SplittableRandom(ulong seed)
            : this(seed, GOLDEN_GAMMA)
        { }

        public SplittableRandom()
        {
            var s = GetAndAdd(unchecked(2 * GOLDEN_GAMMA));
            Seed = mix64(s);
            Gamma = mixGamma(s + GOLDEN_GAMMA);
        }

        /// <inheritdoc/>
        public override long Generate() => (long)mix64(nextSeed());

        public SplittableRandom Split() => new(
            mix64(nextSeed()),
            mixGamma(nextSeed())
        );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong mix64(ulong z)
        {
            unchecked
            {
                z = (z ^ (z >> 33)) * 0xff51afd7ed558ccdL;
                z = (z ^ (z >> 33)) * 0xc4ceb9fe1a85ec53L;
                return z ^ (z >> 33);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong nextSeed()
        {
            unchecked
            {
                Seed += Gamma;
                return Seed;
            }
        }

        private static ulong mixGamma(ulong z)
        {
            unchecked
            {
                z = mix64variant13(z) | 1L;
                var n = NumericHelpers.CountBits(z ^ (z >> 1));
                if (n >= 24) z ^= 0xaaaaaaaaaaaaaaaaL;
            }
            return z;
        }

        private static ulong mix64variant13(ulong z)
        {
            unchecked
            {
                z = (z ^ (z >> 30)) * 0xbf58476d1ce4e5b9L;
                z = (z ^ (z >> 27)) * 0x94d049bb133111ebL;
                return z ^ (z >> 31);
            }
        }
    }
}
