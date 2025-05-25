using LeaveMeAloneFuncSkillForge.Data;
using LeaveMeAloneFuncSkillForge.Domain;

namespace LeaveMeAloneFuncSkillForge.Repositories
{
    public class TaskRepository
    {
        private List<TaskData> _tasks;

        public TaskRepository(int initialCount = 10)
        {
            _tasks = FakeDatabase.TaskFaker.Generate(initialCount);
        }

        public IEnumerable<TaskData> GetTasks() => _tasks;
    }
}
