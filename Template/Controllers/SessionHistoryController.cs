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
                }
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
            var stats = await _mediator.Send(new GetSlideResponseStatsQuery { SessionId =sessionId, SlideId=slideId });
            return Ok(stats);
        }

        // 4. Obtener respuestas de usuarios por slide
        [HttpGet("{sessionId}/slide/{slideId}/responses")]
        public async Task<ActionResult<List<UserResponseDto>>> GetUserResponses(Guid sessionId, int slideId)
        {
            var responses = await _mediator.Send(new GetSlideUserResponsesQuery{ SessionId = sessionId, SlideId = slideId });
            return Ok(responses);
        }

        [HttpGet("{sessionId}/users")]
        public async Task<ActionResult<List<UserInSessionDto>>> GetUsersInSession(Guid sessionId)
        {
            var users = await _mediator.Send(new GetUsersInSessionQuery{SessionId = sessionId});
            return Ok(users);
        }



    }
}
