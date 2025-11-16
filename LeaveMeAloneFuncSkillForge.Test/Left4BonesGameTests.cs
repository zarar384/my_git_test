using LeaveMeAloneFuncSkillForge.Models;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class Left4BonesGameTests
    {
        [Fact]
        public void PlayerStartsAlive()
        {
            var player = new Player();
            Assert.True(player.IsAlive);
            Assert.Equal(100, player.Health);
            Assert.Equal(10, player.Damage);
            Assert.Equal(0, player.Gold);
        }

        [Fact]
        public void PlayerDiesWhenHealthZero()
        {
            var player = new Player();
            player.Health = 0;
            Assert.False(player.IsAlive);
        }

        [Fact]
        public void PlayerReceivesGoldAfterFight()
        {
            var player = new Player();
            player.Gold = 0;
            player.Gold += 20;
            Assert.Equal(20, player.Gold);
        }

        [Fact]
        public void UpgradeDamageReducesGold()
        {
            var player = new Player();
            player.Gold = 50;
            player.Damage += 5;
            player.Gold -= 50;

            Assert.Equal(15, player.Damage);
            Assert.Equal(0, player.Gold);
        }
    }
}
