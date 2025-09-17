namespace LeaveMeAloneFuncSkillForge.Functional
{
    public static class TaskFuncs
    {
        public static string GetResponsible(this TaskData task) =>
            task.Alt(
                t => !string.IsNullOrWhiteSpace(t.AssignedDeveloper) ? t.AssignedDeveloper : null,
                t => !string.IsNullOrWhiteSpace(t.BackupDeveloper) ? t.BackupDeveloper : null,
                _ => "Unassigned"
            );
    }
}
