using LeaveMeAloneFuncSkillForge.Domain.DiscriminatedUnions;
using LeaveMeAloneFuncSkillForge.Services;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class WarehouseProcessorTests
    {
        [Fact]
        public void TestProcessTasks()
        {
            // Arrange
            var tasks = new WarehouseTask[]
            {
                new LoadPallet(Guid.NewGuid(), "A1"),
                new LoadPallet(Guid.NewGuid(), "Z33"),        // Complexity > 5
                new PickOrder(Guid.NewGuid(), 10, "XYZ123"),  // Complexity > 5
                new InventoryCheck("B2", DateTime.Now)
            };

            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            var result = WarehouseProcessor.ProcessTasks(tasks);

            // Assert
            // Check that only complex tasks remain
            Assert.Contains("Z33", result);
            Assert.Contains("XYZ123", result);
            Assert.DoesNotContain("A1", result);    // filtered out

            var output = consoleOutput.ToString();
            Assert.Contains("[Tap] After complexity estimation: 4 tasks", output);
            Assert.Contains("[Tap] After filtering: 3 complex tasks", output);
        }
    }
}
