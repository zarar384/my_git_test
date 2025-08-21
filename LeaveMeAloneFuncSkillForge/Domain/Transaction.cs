namespace LeaveMeAloneFuncSkillForge.Domain
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public decimal Amount { get; set; }

        public override string ToString() => $"Transaction {Id}: {Amount:C} at {Time:HH:mm}";
    }
}
