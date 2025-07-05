using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Querys.Handler
{
    class GetSessionHistoryHandler : IRequestHandler<GetSessionHistoryQuery, SessionReconstructionDto>
    {
        private readonly ISessionHistoryRepository _sessionHistoryRepository;

        public GetSessionHistoryHandler(ISessionHistoryRepository sessionHistoryRepository)
        {
            _sessionHistoryRepository = sessionHistoryRepository;
        }
        public async Task<SessionReconstructionDto> Handle(GetSessionHistoryQuery request, CancellationToken cancellationToken)
        {
            SessionReconstructionDto sessionHistory = await _sessionHistoryRepository.ReconstructSessionAsync(request.SessionId);
            return sessionHistory;
        }


    }
}
