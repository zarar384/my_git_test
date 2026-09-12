using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilitaryDraftSystem.Application.Draft.Commands.DraftCitizen;
using MilitaryDraftSystem.Application.Draft.Commands.OfficerLifecycle;
using MilitaryDraftSystem.Application.Draft.Queries.GetRecruitmentOfficerStats;
using MilitaryDraftSystem.Application.Draft.Queries.ListRecruitmentOfficers;

namespace MilitaryDraftSystem.API.Controllers
{
    [ApiController]
    [Route("draft")]
    [Authorize]
    public class DraftController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DraftController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Manually drafts a citizen on behalf of a human recruitment officer.
        /// </summary>
        [HttpPost("citizens/{citizenId:guid}/officers/{recruitmentOfficerId:guid}")]
        public async Task<IActionResult> DraftCitizen(
            Guid citizenId,
            Guid recruitmentOfficerId,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new DraftCitizenCommand(citizenId, recruitmentOfficerId), cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Retrieves the draft history, morale, and retirement status of a recruitment officer.
        /// </summary>
        [HttpGet("officers/{recruitmentOfficerId:guid}/stats")]
        public async Task<IActionResult> GetRecruitmentOfficerStats(
            Guid recruitmentOfficerId,
            CancellationToken cancellationToken)
        {
            var stats = await _mediator.Send(new GetRecruitmentOfficerStatsQuery(recruitmentOfficerId), cancellationToken);

            return Ok(stats);
        }

        /// <summary>
        /// Lists every recruitment officer, including those whose careers have
        /// ended, along with their current lifecycle status.
        /// </summary>
        [HttpGet("officers")]
        public async Task<IActionResult> ListRecruitmentOfficers(CancellationToken cancellationToken)
        {
            var officers = await _mediator.Send(new ListRecruitmentOfficersQuery(), cancellationToken);

            return Ok(officers);
        }

        /// <summary>
        /// Starts a new game for a player by creating a brand new recruitment
        /// officer. Fails if the player already has an officer whose career
        /// has not ended.
        /// </summary>
        [HttpPost("players/{playerId:guid}/officers")]
        public async Task<IActionResult> StartOfficerCareer(
            Guid playerId,
            [FromBody] StartOfficerCareerRequest request,
            CancellationToken cancellationToken)
        {
            var officerId = await _mediator.Send(
                new StartOfficerCareerCommand(playerId, request.FullName, request.Department),
                cancellationToken);

            return Ok(officerId);
        }

        /// <summary>
        /// Sends a recruitment officer on temporary leave.
        /// </summary>
        [HttpPost("officers/{recruitmentOfficerId:guid}/leave")]
        public async Task<IActionResult> GoOnLeave(Guid recruitmentOfficerId, CancellationToken cancellationToken)
        {
            await _mediator.Send(new GoOnLeaveCommand(recruitmentOfficerId), cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Returns a recruitment officer to active duty from leave.
        /// </summary>
        [HttpPost("officers/{recruitmentOfficerId:guid}/return-from-leave")]
        public async Task<IActionResult> ReturnFromLeave(Guid recruitmentOfficerId, CancellationToken cancellationToken)
        {
            await _mediator.Send(new ReturnFromLeaveCommand(recruitmentOfficerId), cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Ends a recruitment officer's career by resignation.
        /// </summary>
        [HttpPost("officers/{recruitmentOfficerId:guid}/resign")]
        public async Task<IActionResult> Resign(Guid recruitmentOfficerId, CancellationToken cancellationToken)
        {
            await _mediator.Send(new ResignCommand(recruitmentOfficerId), cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Ends a recruitment officer's career by voluntary retirement.
        /// </summary>
        [HttpPost("officers/{recruitmentOfficerId:guid}/retire")]
        public async Task<IActionResult> Retire(Guid recruitmentOfficerId, CancellationToken cancellationToken)
        {
            await _mediator.Send(new RetireCommand(recruitmentOfficerId), cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Ends a recruitment officer's career by dismissal.
        /// </summary>
        [HttpPost("officers/{recruitmentOfficerId:guid}/fire")]
        public async Task<IActionResult> Fire(Guid recruitmentOfficerId, CancellationToken cancellationToken)
        {
            await _mediator.Send(new FireOfficerCommand(recruitmentOfficerId), cancellationToken);

            return Ok();
        }
    }

    /// <summary>
    /// Request body for starting a new officer career.
    /// </summary>
    public sealed record StartOfficerCareerRequest(string FullName, string Department);
}

