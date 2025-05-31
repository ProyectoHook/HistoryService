using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Commands.Handler
{
    class RegisterUserAnswerHandler : IRequestHandler<RegisterUserAnswerCommand, Guid>
    {
        private readonly ISessionHistoryRepository _sessionHistoryRepository;

        public RegisterUserAnswerHandler(ISessionHistoryRepository sessionHistoryRepository)
        {
            _sessionHistoryRepository = sessionHistoryRepository;
        }

        public async Task<Guid> Handle(RegisterUserAnswerCommand request, CancellationToken cancellationToken)
        {
            Guid IdUser = await _sessionHistoryRepository.RegisterUserAnswerAsync(
                request.SessionId,
                request.SlideId,
                request.UserId,
                request.Answer,
                request.TimeElapsed);

            return IdUser;

        }
    }
}
