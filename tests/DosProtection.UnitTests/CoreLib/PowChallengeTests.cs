using DosProtection.ProofOfWork;

namespace DosProtection.UnitTests.CoreLib
{
    public class PowChallengeTests
    {
        [Fact]
        public void Test_GenerateChallenge_ReturnsValidStatement()
        {
            var config = new PowChallengeConfig { Difficulty = 10 };
            var statement = PowChallenge.GenerateChallenge(config);
            
            Assert.Equal(10, statement.Difficulty);
            Assert.False(string.IsNullOrWhiteSpace(statement.Challenge));
        }

        [Fact]
        public void Test_SolveChallenge_ReturnsExpectedSolution()
        {
            var statement = new PowChallengeStatement { Challenge = "test-challenge", Difficulty = 10 };
            var solution = PowChallenge.SolveChallenge(statement);
            
            Assert.Equal(statement.Challenge, solution.Challenge);
            Assert.True(PowChallenge.ValidateChallenge(statement, solution));
        }

        [Fact]
        public void Test_ValidateChallenge_WithCorrectSolution_ReturnsTrue()
        {
            var statement = new PowChallengeStatement { Challenge = "valid-challenge", Difficulty = 10 };
            var solution = PowChallenge.SolveChallenge(statement);
            
            Assert.True(PowChallenge.ValidateChallenge(statement, solution));
        }

        [Fact]
        public void Test_ValidateChallenge_WithIncorrectSolution_ReturnsFalse()
        {
            var statement = new PowChallengeStatement { Challenge = "invalid-challenge", Difficulty = 10 };
            var solution = new PowChallengeSolution { Challenge = statement.Challenge, Nonce = 12345 };
            
            Assert.False(PowChallenge.ValidateChallenge(statement, solution));
        }
    }
}
