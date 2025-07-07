using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Commands;
using Application.DTOs;
using Application.Querys;
using Application.Request;
using Microsoft.AspNetCore.Authorization;

namespace Template.Controllers
{
    [ApiController]
    [Route("api/History")]

    public class SessionHistoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SessionHistoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // 1. Registrar cambio de slide
        [HttpPost("{sessionId}/SlideChange")]
        public async Task<IActionResult> RegisterSlideChange(Guid sessionId, [FromBody] SlideSnapshotRequest request)
        {
            Console.WriteLine(sessionId);
            Console.WriteLine(request.SlideId);
            Console.WriteLine(request.SlideIndex);
            var command = new RegisterSlideChangeCommand
            {
                SessionId = sessionId,
                ConnectedUserIds = request.ConnectedUserIds,
                SlideSnapshot = new SlideSnapshotDto
                {
                    Id = request.SlideId,
                    Ask = request.Ask,
                    AnswerCorrect = request.AnswerCorrect,
                    Options = request.Options
                },
                UserCreateId = request.UserCreateId,
                PresentationId = request.presentationId

            };

            await _mediator.Send(command);
            return Ok();
        }

        // 2. Registrar respuesta del usuario
        [HttpPost("/answer")]
        public async Task<ActionResult<SlideStatsDto>> RegisterUserAnswer([FromBody] RegisterUserAnswerCommand command)
        {

            Guid IdUser = await _mediator.Send(command);
            return await GetSlideStats(command.SessionId, command.SlideId);
        }

        // 3. Obtener estaditicas de un slide
        [HttpGet("{sessionId}/slide/{slideId}/stats")]
        public async Task<ActionResult<SlideStatsDto>> GetSlideStats(Guid sessionId, int slideId)
        {
            var stats = await _mediator.Send(new GetSlideResponseStatsQuery { SessionId = sessionId, SlideId = slideId });
            return Ok(stats);
        }

        [HttpGet("{sessionId}/metricas")]
        public async Task<ActionResult<SessionReconstructionDto>> ReconstructSession(Guid sessionId)
        {
            var reconstruction = await _mediator.Send(new GetSessionHistoryQuery { SessionId = sessionId });
            return Ok(reconstruction);
        }

        [HttpGet("{presentationId}/sessions")]
        [ProducesResponseType(typeof(SessionsResponse), 200)]
        public async Task<ActionResult<SessionReconstructionDto>> GetSessions(int presentationId)
        {
                return Ok(await _mediator.Send(new GetSessions(presentationId)));
        }
    }
}
