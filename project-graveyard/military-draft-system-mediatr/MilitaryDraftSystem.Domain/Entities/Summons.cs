namespace MilitaryDraftSystem.Domain.Entities
{
    public class Summons
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public bool IsAttended { get; set; }
    }
}
