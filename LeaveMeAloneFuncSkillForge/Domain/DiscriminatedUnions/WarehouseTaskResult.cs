using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveMeAloneFuncSkillForge.Domain.DiscriminatedUnions
{
    public abstract record WarehouseTaskResult
    {
        public Guid TaskId { get; init; }
    }

    public record TaskCompleted(Guid TaskId, string Message) : WarehouseTaskResult;
    public record TaskFailed(Guid TaskId, string Reason) : WarehouseTaskResult;
    public record TaskError(Guid TaskId, Exception Error) : WarehouseTaskResult;
}
