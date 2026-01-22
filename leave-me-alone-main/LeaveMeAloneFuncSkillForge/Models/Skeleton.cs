namespace LeaveMeAloneFuncSkillForge.Models
{
    public class Skeleton
    {
        private static Random _rnd = new Random();
        public int Health { get; set; } = _rnd.Next(20, 50);
        public int Damage { get; set; } = _rnd.Next(5, 15);
    }
}
