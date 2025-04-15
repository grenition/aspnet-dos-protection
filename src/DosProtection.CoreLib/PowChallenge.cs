using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace DosProtection.ProofOfWork
{
    public struct PowChallengeConfig
    {
        public int Difficulty;
    }

    public struct PowChallengeStatement
    {
        public string Challenge;
        public int Difficulty;
    }

    public struct PowChallengeSolution
    {
        public string Challenge;
        public int Nonce;
    }

    public static class PowChallenge
    {
        public static PowChallengeStatement GenerateChallenge(PowChallengeConfig config)
        {
            return new PowChallengeStatement
            {
                Challenge = Guid.NewGuid().ToString(),
                Difficulty = config.Difficulty
            };
        }

        public static PowChallengeSolution SolveChallenge(PowChallengeStatement statement)
        {
            var target = BigInteger.One << (256 - statement.Difficulty);
            var nonce = 0;
            while (true)
            {
                if (ComputeSha256AsBigInt(statement.Challenge + nonce) < target)
                {
                    return new PowChallengeSolution
                    {
                        Challenge = statement.Challenge,
                        Nonce = nonce
                    };
                }
                nonce++;
            }
        }

        public static bool ValidateChallenge(PowChallengeStatement statement, PowChallengeSolution solution)
        {
            var target = BigInteger.One << (256 - statement.Difficulty);
            return ComputeSha256AsBigInt(statement.Challenge + solution.Nonce) < target;
        }

        private static BigInteger ComputeSha256AsBigInt(string input)
        {
            using var sha = SHA256.Create();
            var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return new BigInteger(hashBytes, isUnsigned: true, isBigEndian: true);
        }
    }
}
