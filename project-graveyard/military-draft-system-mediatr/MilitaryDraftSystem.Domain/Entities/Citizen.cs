using MilitaryDraftSystem.Domain.Interfaces;

namespace MilitaryDraftSystem.Domain.Entities
{
    public class Citizen
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int Age { get; set; }

        public List<Summons> Summonses { get; set; } = new();

        public List<IDomainEvent> DomainEvents { get; } = new();

        public void SendSummons(DateTime date)
        {
            // Validate age before sending summons
            if (Age < 18)
                throw new Exception("Citizen is too young");

            // Create new summons
            var summons = new Summons
            {
                Id = Guid.NewGuid(),
                Date = date,
                IsAttended = false
            };

            // Add summons to collection
            Summonses.Add(summons);

            // Add domain event
            DomainEvents.Add(new SummonsSentDomainEvent(Id, summons.Id));
        }
    }
}
