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
    [Route("api/session")]
    [Authorize]
    public class SessionHistoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SessionHistoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // 1. Registrar cambio de slide
        [HttpPost("{sessionId}/slide")]
        public async Task<IActionResult> RegisterSlideChange(int sessionId, [FromBody] SlideSnapshotRequest request)
        {
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
                }
            };

            await _mediator.Send(command);
            return Ok();
        }

        // 2. Registrar respuesta del usuario
        [HttpPost("{sessionId}/slide/{slideId}/answer")]
        public async Task<IActionResult> RegisterUserAnswer([FromBody] RegisterUserAnswerCommand command)
        {

            Guid IdUser = await _mediator.Send(command);
            return Ok(new{messsage = "Respuesta registrada del usuario con id" +IdUser });
        }

        // 3. Obtener estaditicas de un slide
        [HttpGet("{sessionId}/slide/{slideId}/stats")]
        public async Task<ActionResult<SlideStatsDto>> GetSlideStats(int sessionId, int slideId)
        {
            var stats = await _mediator.Send(new GetSlideResponseStatsQuery { SessionId =sessionId, SlideId=slideId });
            return Ok(stats);
        }

        // 4. Obtener respuestas de usuarios por slide
        [HttpGet("{sessionId}/slide/{slideId}/responses")]
        public async Task<ActionResult<List<UserResponseDto>>> GetUserResponses(int sessionId, int slideId)
        {
            var responses = await _mediator.Send(new GetSlideUserResponsesQuery{ SessionId = sessionId, SlideId = slideId });
            return Ok(responses);
        }

        

    }
}
