﻿using LeaveMeAloneFuncSkillForge.Domain.DiscriminatedUnions;

namespace LeaveMeAloneFuncSkillForge.Services
{
    public static class WarehouseProcessor
    {
        /// <summary>
        /// Calculates task complexity based on type and properties
        /// </summary>
        public static int EstimateTaskComplexity(WarehouseTask task) =>
            task switch
            {
                LoadPallet lp => lp.Location.Length * 2,
                PickOrder po => po.Quantity * po.ProductCode.Length,
                InventoryCheck ic => ic.Section.Length * 3,
                _ => 1
            };

        public static string ProcessTasks(IEnumerable<WarehouseTask> tasks)
        {
            Func<IEnumerable<WarehouseTask>, IEnumerable<(WarehouseTask Task, int Complexity)>> transformer =
                ts => ts.Select(task =>
                    (task, Complexity : EstimateTaskComplexity(task)))
                .Tap(list => Console.WriteLine($"[Tap] After complexity estimation: {list.Count()} tasks"))
                .Where(tc => tc.Complexity > 5)
                .Tap(list => Console.WriteLine($"[Tap] After filtering: {list.Count()} complex tasks"));

            Func<IEnumerable<(WarehouseTask Task, int Complexity)>, string> aggregator =
                list => string.Join("; ",
                list.Select(tc =>
                    tc.Task switch
                    {
                        LoadPallet lp => $"LoadPallet {lp.PalletId} at {lp.Location} (Complexity {tc.Complexity})",
                        PickOrder po => $"PickOrder {po.OrderId} qty={po.Quantity} code={po.ProductCode} (Complexity {tc.Complexity})",
                        InventoryCheck ic => $"InventoryCheck {ic.Section} at {ic.CheckedAt:yyyy-MM-dd} (Complexity {tc.Complexity})",
                        _ => "Unknown task"
                    }
                )
            );

            return tasks.Transduce(transformer, aggregator);
        }
    }
}
