using System;
using System.Runtime.CompilerServices;

namespace NPrng.Generators
{
    public sealed class ChaCha20Generator : AbstractPseudoRandomGenerator
    {
        private const int Rounds = 20;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ROTL(uint a, int b) => unchecked((a << b) | (a >> (32 - b)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void QR(ref uint a, ref uint b, ref uint c, ref uint d)
        {
            unchecked
            {
                a += b; 
                d ^= a;
                d = ROTL(d, 16);
                c += d;
                b ^= c;
                b = ROTL(b,12);
                a += b;
                d ^= a;
                d = ROTL(d, 8);
                c += d;
                b ^= c;
                b = ROTL(b, 7);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ChaChaBlock(uint[] input, out uint[] output)
        {
            const int blockSize = 16;

            unchecked
            {
                var x = new uint[blockSize];
                Array.Copy(input, 0, x, 0, blockSize);

                for (var i = 0; i < (Rounds/2); i++)
                {
                    // Odd round
                    QR(ref x[0], ref x[4], ref x[ 8], ref x[12]);
                    QR(ref x[1], ref x[5], ref x[ 9], ref x[13]);
                    QR(ref x[2], ref x[6], ref x[10], ref x[14]);
                    QR(ref x[3], ref x[7], ref x[11], ref x[15]);

                    // Even round
                    QR(ref x[0], ref x[5], ref x[10], ref x[15]);
                    QR(ref x[1], ref x[6], ref x[11], ref x[12]);
                    QR(ref x[2], ref x[7], ref x[ 8], ref x[13]);
                    QR(ref x[3], ref x[4], ref x[ 9], ref x[14]);
                }

                for (var i = 0; i < blockSize; i++)
                {
                    x[i] += input[i];
                }
            
                output = x;
            }
        }

        internal uint[] Key { get; private set; }
        private uint[] Cache;

        internal ChaCha20Generator(uint[] key)
        {
            Key = key;
            Cache = Array.Empty<uint>();
            
            // Key[12] serves two purposes: as a ChaCha block counter
            // and as a pointer to Cache.
            var counter = Key[12];
            if (counter % 8 != 0)
            {
                Key[12] = counter / 8;
                ChaChaBlock(Key, out Cache);
                Key[12] = counter;
            }
        }

        public ChaCha20Generator(ulong seed)
        {
            Key = GenerateInitialKey(seed);
            Cache = Array.Empty<uint>();
        }

        private static uint[] GenerateInitialKey(ulong seed)
        {
            unchecked
            {
                if (seed == 0)
                {
                    seed = 0x2654633dc37cc394;
                }
                var key = new uint[16];

                // Fill constants
                key[0] = 0x61707865;
                key[1] = 0x3320646e;
                key[2] = 0x79622d32;
                key[3] = 0x6b206574;

                // Fill key
                for (var i = 0; i < 4; i++)
                {
                    key[4+2*i] = (uint)(seed & 0xffffffff);
                    key[5+2*i] = (uint)((seed >> 32) & 0xffffffff);
                }

                // Initialize block/cache counter to 0
                key[12] = 0;

                // Fill nonce
                FillNonce(new ArraySegment<uint>(key, 13, 3), seed);
                return key;
            }
        }

        private static void FillNonce(ArraySegment<uint> arraySegment, ulong seed)
        {
            unchecked
            {
                var rng = new LinearCongruentGenerator(seed);
                var array = arraySegment.Array;
                var count = arraySegment.Offset + arraySegment.Count;
                for (var i = arraySegment.Offset; i < count; i++)
                {
                    array[i] = (uint)rng.Generate();
                }
            }
        }

        /// <inheritdoc/>
        public override long Generate()
        {
            unchecked
            {
                var mod = Key[12] % 8;
                if (mod == 0)
                {
                    Key[12] /= 8;
                    ChaChaBlock(Key, out Cache);
                    Key[12] *= 8;
                }
                Key[12]++;

                var first = Cache[2*mod];
                var second = Cache[2*mod + 1];
                var result = ((ulong)first << 32) + second;
                if (Key[12] >= (1 << 16) + 7)
                {
                    Key = GenerateInitialKey(result);
                    Cache = Array.Empty<uint>();
                }
                return (long)result;
            }
        }
    }
}
