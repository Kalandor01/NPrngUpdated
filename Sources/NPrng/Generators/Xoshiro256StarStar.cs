using System.Runtime.CompilerServices;

namespace NPrng.Generators
{
    public sealed class Xoshiro256StarStar : AbstractPseudoRandomGenerator
    {
        internal ulong S0 { get; private set; }
        internal ulong S1 { get; private set; }
        internal ulong S2 { get; private set; }
        internal ulong S3 { get; private set; }

        internal Xoshiro256StarStar(ulong s0, ulong s1, ulong s2, ulong s3)
        {
            S0 = s0;
            S1 = s1;
            S2 = s2;
            S3 = s3;
        }

        public Xoshiro256StarStar(IPseudoRandomGenerator seeder)
        {
            S0 = (ulong)seeder.Generate();
            S1 = (ulong)seeder.Generate();
            S2 = (ulong)seeder.Generate();
            S3 = (ulong)seeder.Generate();
        }

        public Xoshiro256StarStar(ulong seed)
            : this(new LinearCongruentGenerator(seed))
        { }

        /// <inheritdoc/>
        public override long Generate()
        {
            unchecked
            {
                var result = rol64(S1 * 5, 7) * 9;
                var t = S1 << 17;
                S2 ^= S0;
                S3 ^= S1;
                S1 ^= S2;
                S0 ^= S3;
                S2 ^= t;
                S3 ^= rol64(S3, 45);
                return (long)result;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong rol64(ulong x, int k) => unchecked((x << k) | (x >> (64 - k)));
    }
}
