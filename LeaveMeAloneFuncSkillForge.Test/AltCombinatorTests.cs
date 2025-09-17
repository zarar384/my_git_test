using LeaveMeAloneFuncSkillForge.Domain;
using LeaveMeAloneFuncSkillForge.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class AltCombinatorTests
    {
        [Fact]
        public void Should_Return_AssignedDeveloper_When_Present()
        {
            // Arrange
            var task = new TaskData
            {
                AssignedDeveloper = "Alice",
                BackupDeveloper = "Bob"
            };

            // Act
            var result = task.GetResponsible();

            // Assert
            Assert.Equal("Alice", result);
        }

        [Fact]
        public void Should_Fallback_To_BackupDeveloper_When_Assigned_IsMissing()
        {
            // Arrange
            var task = new TaskData
            {
                AssignedDeveloper = null,
                BackupDeveloper = "Bob"
            };

            // Act
            var result = task.GetResponsible();

            // Assert
            Assert.Equal("Bob", result);
        }

        [Fact]
        public void Should_Return_Unassigned_When_BothMissing()
        {
            // Arrange
            var task = new TaskData
            {
                AssignedDeveloper = "",
                BackupDeveloper = ""
            };
            
            // Act
            var result = task.GetResponsible();

            // Assert
            Assert.Equal("Unassigned", result);
        }

        [Fact]
        public void Should_Handle_Null_Assigned_And_Backup()
        {
            // Arrange
            var task = new TaskData
            {
                AssignedDeveloper = null,
                BackupDeveloper = null
            };

            // Act
            var result = task.GetResponsible();

            // Assert
            Assert.Equal("Unassigned", result);
        }
    }
}
