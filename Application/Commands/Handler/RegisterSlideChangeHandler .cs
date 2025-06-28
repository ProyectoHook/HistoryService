using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using MediatR;

namespace Application.Commands.Handler
{
    class RegisterSlideChangeHandler : IRequestHandler<RegisterSlideChangeCommand,Unit>
    {
        private readonly ISessionHistoryRepository _sessionHistoryRepository;
        private readonly IJwtService _jwtService;

        public RegisterSlideChangeHandler(ISessionHistoryRepository sessionHistoryRepository, IJwtService jwtService)
        {
            _sessionHistoryRepository = sessionHistoryRepository;
            _jwtService = jwtService;
        }

        public async Task<Unit> Handle(RegisterSlideChangeCommand request, CancellationToken cancellationToken)
        {
            await _sessionHistoryRepository.RegisterSlideChangeForUsersAsync(
                request.SessionId,
                request.SlideSnapshot,
                request.ConnectedUserIds,
                Guid.Parse("D0E1D9FA-CF43-4673-8B1B-E6A5C2E05129"));

            return Unit.Value;
        }
    }
}
