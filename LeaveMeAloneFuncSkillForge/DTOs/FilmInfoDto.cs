using LeaveMeAloneFuncSkillForge.DiscriminatedUnions;

namespace LeaveMeAloneFuncSkillForge.DTOs
{
    public class FilmInfoDto
    {
        public string Title { get; }
        public double Revenue { get; }

        public FilmInfoDto(string title, double revenue)
        {
            Title = title;
            Revenue = revenue;
        }
    }
}
