namespace LeaveMeAloneFuncSkillForge.Functional
{
    public static class FilmFilters
    {
        public static IEnumerable<Film> GetFilmsByGenre(
            IEnumerable<Film> source,
            string genre) =>
            source.Where(x => x.Genre == genre);
    }
}
