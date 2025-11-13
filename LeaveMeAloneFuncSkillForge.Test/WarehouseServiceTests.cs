using LeaveMeAloneFuncSkillForge.DiscriminatedUnions;
using LeaveMeAloneFuncSkillForge.Services;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class WarehouseServiceTests
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
            var result = WarehouseService.ProcessTasks(tasks);

            // Assert
            // Check that only complex tasks remain
            Assert.Contains("Z33", result);
            Assert.Contains("XYZ123", result);
            Assert.DoesNotContain("A1", result);    // filtered out

            var output = consoleOutput.ToString();
            Assert.Contains("[Tap] After complexity estimation: 4 tasks", output);
            Assert.Contains("[Tap] After filtering: 3 complex tasks", output);
        }

        [Fact]
        public void LoadPallet_ShouldReturnCompleted()
        {
            // Arrange
            var task = new LoadPallet(Guid.NewGuid(), "A1");

            // Act
            var result = WarehouseService.ExecuteTask(task);

            // Assert
            var completed = Assert.IsType<TaskCompleted>(result);
            Assert.Contains("loaded", completed.Message);
        }

        [Fact]
        public void PickOrder_WithPositiveQuantity_ShouldReturnCompleted()
        {
            // Arrange
            var task = new PickOrder(Guid.NewGuid(), 5, "PRD-123");

            // Act
            var result = WarehouseService.ExecuteTask(task);

            // Assert
            var completed = Assert.IsType<TaskCompleted>(result);
            Assert.Contains("Picked", completed.Message);
        }

        [Fact]
        public void PickOrder_WithZeroQuantity_ShouldReturnFailed()
        {
            // Arrange
            var task = new PickOrder(Guid.NewGuid(), 0, "PRD-123");

            // Act
            var result = WarehouseService.ExecuteTask(task);

            // Assert
            var failed = Assert.IsType<TaskFailed>(result);
            Assert.Equal("Quantity must be greater than zero", failed.Reason);
        }

        [Fact]
        public void InventoryCheck_ShouldReturnCompleted()
        {
            // Arrange
            var task = new InventoryCheck("Section-7", DateTime.UtcNow);

            // Act
            var result = WarehouseService.ExecuteTask(task);

            // Assert
            var completed = Assert.IsType<TaskCompleted>(result);
            Assert.Contains("Inventory checked", completed.Message);
        }

        [Fact]
        public void ExecuteTask_WithException_ShouldReturnTaskError()
        {
            // Arrange
            var task = new BrokenTask(); 

            // Act
            var result = WarehouseService.ExecuteTask(task);

            // Assert
            var error = Assert.IsType<TaskError>(result);
            Assert.IsType<InvalidOperationException>(error.Error);
        }

        // Error test stub
        private record BrokenTask : WarehouseTask;
    }
}
