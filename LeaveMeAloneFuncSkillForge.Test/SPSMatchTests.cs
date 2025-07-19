using LeaveMeAloneFuncSkillForge.Common;
using LeaveMeAloneFuncSkillForge.Functional;
using LeaveMeAloneFuncSkillForge.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class SPSMatchTests
    {
        [Fact]
        public void PlayGames_ShouldReturnFiveRounds()
        {
            // Arrange
            var rounds = 5;

            // Act
            var result = SPSMatchService.PlayGames(
                myStrategy: Stategies.CounterLastMove,
                theirStraategy: Stategies.MirrorLastMove,
                rounds: rounds,
                resolve: SPSMatchFunc.CalculateMatchResult
            ).ToList();

            // Assert
            Assert.Equal(rounds, result.Count);
        }

        [Fact]
        public void AllResults_ShouldContainValidMoves()
        {
            // Act
            var result = SPSMatchService.PlayGames(
                myStrategy: Stategies.CounterLastMove,
                theirStraategy: Stategies.MirrorLastMove,
                rounds: 5,
                resolve: SPSMatchFunc.CalculateMatchResult
            );

            // Assert
            foreach (var match in result) 
            {
                Assert.IsType<SPS>(match.MyMove);
                Assert.IsType<SPS>(match.OpponentMove);
                Assert.IsType<GameResult>(match.Result);
                Assert.False(string.IsNullOrWhiteSpace(match.Reason));
            }
        }

        [Fact]
        public void FormatHistory_ShouldReturnNonEmptyString()
        {
            // Arrange
            var result = SPSMatchService.PlayGames(
                myStrategy: Stategies.CounterLastMove,
                theirStraategy: Stategies.MirrorLastMove,
                rounds: 3,
                resolve: SPSMatchFunc.CalculateMatchResult
            );

            // Act
            var formatted = SPSMatchFunc.FormatHistory(result);

            // Assert
            Assert.False(string.IsNullOrEmpty(formatted));  
            Assert.Contains("Game 1:", formatted);
        }
    }
}
