using LeaveMeAloneFuncSkillForge.Common;
using LeaveMeAloneFuncSkillForge.DTOs;
using LeaveMeAloneFuncSkillForge.Functional;
using LeaveMeAloneFuncSkillForge.Interfaces;
using LeaveMeAloneFuncSkillForge.Services;
using Moq;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class TournamentRunnerTests
    {
        [Fact]
        public void Run_Should_CalculateCorrectResults()
        {
            // Arrange
            var nameA = "StrategyA";
            var nameB = "StrategyB";
            int rounds = 10;

            var fakeResults = new List<MatchResult>
            {
                new MatchResult(new SPS(), new SPS(), GameResult.Win  , "info1" ),
                new MatchResult(new SPS(), new SPS(), GameResult.Win  , "info2" ),
                new MatchResult(new SPS(), new SPS(), GameResult.Lose , "info3" ),
                new MatchResult(new SPS(), new SPS(), GameResult.Draw , "info4" ),
                new MatchResult(new SPS(), new SPS(), GameResult.Draw , "info5" ),
                new MatchResult(new SPS(), new SPS(), GameResult.Win  , "info6" ),
                new MatchResult(new SPS(), new SPS(), GameResult.Lose , "info7" ),
                new MatchResult(new SPS(), new SPS(), GameResult.Draw , "info8" ),
                new MatchResult(new SPS(), new SPS(), GameResult.Lose , "info9" ),
                new MatchResult(new SPS(), new SPS(), GameResult.Win  , "info10"),
            };

            var mockService = new Mock<ISPSMatchServiceWrapper>();
            mockService.Setup(s=>s.PlayGames(It.IsAny<Strategy>(), It.IsAny<Strategy>(), rounds, It.IsAny<Func<SPS, SPS, MatchResult>>()))
                .Returns(fakeResults);

            // Act
            var result = TournamentRunner.Run(nameA, null!, nameB, null!, rounds, SPSMatchFunc.CalculateMatchResult, mockService.Object);

            // Assert
            Assert.Equal(nameA, result.StrategyA);
            Assert.Equal(nameB, result.StrategyB);
            Assert.Equal(4, result.WinsA); // Win x4
            Assert.Equal(3, result.WinsB); // Lose x3
            Assert.Equal(3, result.Draws); // Draw x3

            Assert.Equal(4 / (double)rounds, result.WinRateA);
            Assert.Equal(3 / (double)rounds, result.WinRateB);
        }
    }
}
