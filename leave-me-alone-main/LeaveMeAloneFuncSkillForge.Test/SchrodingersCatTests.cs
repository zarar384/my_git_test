using LeaveMeAloneFuncSkillForge.DiscriminatedUnions;
using System;
namespace LeaveMeAloneFuncSkillForge.Test
{
    public class SchrodingersCatTests
    {
        [Fact]
        public void SchrodingersCat_AliveAndDeadStates_ShouldBeDistinct()
        {
            // Arrange
            var aliveCat = new SchrodingersCat.Alive();
            var deadCat = new SchrodingersCat.Dead();
            
            // Assert
            Assert.IsType<SchrodingersCat.Alive>(aliveCat);
            Assert.IsType<SchrodingersCat.Dead>(deadCat);
            Assert.NotEqual<SchrodingersCat>(aliveCat, deadCat);
        }

        [Fact]
        public void SchrodingersCat_PatternMatching_ShouldIdentifyStates()
        {
            // Arrange
            SchrodingersCat cat = new SchrodingersCat.Alive();

            // Act
            string stateDescription = cat switch
            {
                SchrodingersCat.Alive => "The cat is alive.",
                SchrodingersCat.Dead => "The cat is dead.",
                _ => "Unknown state."
            };

            // Assert
            Assert.Equal("The cat is alive.", stateDescription);
        }
    }
}
