namespace LeaveMeAloneFuncSkillForge.Models
{
    public class Player
    {
        public int Health { get; set; } = 100;
        public int Damage { get; set; } = 10;
        public int Gold { get; set; } = 0;

        public bool IsAlive => Health > 0;
    }

}
