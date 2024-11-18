namespace NPrng
{
    public interface IPseudoRandomGenerator
    {
        /// <summary>Generates a pseudo random 64-bit integer.</summary>
        long Generate();

        /// <summary>Generates a pseudo random non-negative 64-bit integer up to range.</summary>
        long GenerateLessOrEqualTo(long range);

        /// <summary>Generates a pseudo random 64-bit integer in [lower, upper] range.</summary>
        long GenerateInRange(long lower, long upper);

        /// <summary>Generates a pseudo random 64-bit double in [0, 1) range.</summary>
        double GenerateDouble();
    }
}
