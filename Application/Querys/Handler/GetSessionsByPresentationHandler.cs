using Application.DTOs;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Querys.Handler
{
    class GetSessionsByPresentationHandler : IRequestHandler<GetSessions, SessionsResponse>
    {
        private readonly ISessionHistoryRepository _sessionHistoryRepository;

        public GetSessionsByPresentationHandler(ISessionHistoryRepository sessionHistoryRepository)
        {
            _sessionHistoryRepository = sessionHistoryRepository;
        }
        public async Task<SessionsResponse> Handle(GetSessions request, CancellationToken cancellationToken)
        {
           List<SessionDate> responseList = new ();

           List<SessionHistory> list = await _sessionHistoryRepository.GetSessionsByPresentation(request.presentationId);
            foreach (SessionHistory s in list)
            {
                SessionDate session = new()
                {
                    Guid = s.SessionId,
                    DateTime = s.Timestamp,
                };
                responseList.Add(session);
            }
            return new SessionsResponse() { Sessions = responseList };
        }
    }
}
