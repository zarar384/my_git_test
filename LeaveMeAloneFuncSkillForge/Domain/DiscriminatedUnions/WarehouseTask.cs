namespace LeaveMeAloneFuncSkillForge.Domain.DiscriminatedUnions
{
    public abstract record WarehouseTask
    {
        public Guid TaskId { get; init; } = Guid.NewGuid();
    }

    public record LoadPallet(Guid PalletId, string Location) : WarehouseTask;

    public record PickOrder(Guid OrderId, int Quantity, string ProductCode) : WarehouseTask;

    public record InventoryCheck(string Section, DateTime CheckedAt) : WarehouseTask;
}
