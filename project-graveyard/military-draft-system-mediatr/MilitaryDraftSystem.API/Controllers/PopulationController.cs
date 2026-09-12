using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilitaryDraftSystem.Application.Population.Queries.GetCemeteryRecords;
using MilitaryDraftSystem.Application.Population.Queries.GetLifecycleStatistics;

namespace MilitaryDraftSystem.API.Controllers
{
    [ApiController]
    [Route("population")]
    [Authorize]
    public class PopulationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PopulationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lists the cemetery: every citizen and recruitment officer who has
        /// died, regardless of whether their original record still exists.
        /// </summary>
        [HttpGet("cemetery")]
        public async Task<IActionResult> GetCemeteryRecords(CancellationToken cancellationToken)
        {
            var records = await _mediator.Send(new GetCemeteryRecordsQuery(), cancellationToken);

            return Ok(records);
        }

        /// <summary>
        /// Retrieves historical death and worker-lifecycle statistics.
        /// </summary>
        [HttpGet("lifecycle-statistics")]
        public async Task<IActionResult> GetLifecycleStatistics(CancellationToken cancellationToken)
        {
            var statistics = await _mediator.Send(new GetLifecycleStatisticsQuery(), cancellationToken);

            return Ok(statistics);
        }
    }
}
