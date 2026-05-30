using MediatR;
using Microsoft.AspNetCore.Mvc;
using MilitaryDraftSystem.Application.Draft.Commands.SendSummons;

namespace MilitaryDraftSystem.API.Controllers
{
    [ApiController]
    [Route("draft")]
    public class DraftController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DraftController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("summons")]
        public async Task<IActionResult> SendSummons(SendSummonsCommand command)
        {
            // send summons command to the application layer
            await _mediator.Send(command);

            return Ok();
        }
    }
}
