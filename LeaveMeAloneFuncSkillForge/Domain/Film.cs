namespace LeaveMeAloneFuncSkillForge.Domain
{
    public class Film
    {
        public string Title { get; set; }
        public string Genre { get; set; }
        public double BoxOfficeRevenue { get; set; }
    }

    //public sealed record Film(string Title, string Genre, double BoxOfficeRevenue);
}
