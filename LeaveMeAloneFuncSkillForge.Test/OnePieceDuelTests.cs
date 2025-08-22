using LeaveMeAloneFuncSkillForge.Common;
using LeaveMeAloneFuncSkillForge.DTOs;
using LeaveMeAloneFuncSkillForge.Services;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class OnePieceDuelTests
    {
        [Fact]
        public void Battle_ShouldEndWithWinner()
        {
            // Arrange
            var luffy = new OnePieceCharacterDto
            {
                Name = "Luffy",
                Damage = 15,
                CritChance = 30,
                DodgeChance = 10,
                SpecialMove = "Gomu Gomu no Pistol",
                Rarity = "Legendary",
                DevilFruit = "GomuGomu",
                Bounty = 1_500_000_000,
                HP = 1000000
            };

            var zoro = new OnePieceCharacterDto
            {
                Name = "Zoro",
                Damage = 12,
                CritChance = 20,
                DodgeChance = 5,
                SpecialMove = "Santoryu",
                Rarity = "Epic",
                Bounty = 600_000_000,
                HP = 1000000
            };

            var initialState = new State
            {
                Player = luffy,
                Enemy = zoro
            };

            // Act
            var finalState = initialState.AggregateUntil(
                s => s.Player.HP <= 0 || s.Enemy.HP <= 0,
                OnePieceDuelGameLogic.BattleTurn
            );

            // Assert
            Assert.True(finalState.Player.HP <= 0 || finalState.Enemy.HP <= 0, "Battle should end when a character reaches 0 HP");
            Assert.NotEmpty(finalState.Log); // There should be a battle log
            Assert.Contains(finalState.Player.Name, finalState.Log.First() + finalState.Log.Last()); // The log contains the names of the participants
            Assert.True(finalState.Player.HP > 0 || finalState.Enemy.HP > 0, "There must be a winner");
        }
    }
}
