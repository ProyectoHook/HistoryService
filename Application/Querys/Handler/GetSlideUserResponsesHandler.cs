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
    class GetSlideUserResponsesHandler : IRequestHandler<GetSlideUserResponsesQuery, List<UserResponseDto>>
    {
        private readonly ISessionHistoryRepository _sessionHistoryRepository;
        private readonly ISlideRepository _slideRepository;

        public GetSlideUserResponsesHandler(
            ISessionHistoryRepository sessionHistoryRepository,
            ISlideRepository slideRepository)
        {
            _sessionHistoryRepository = sessionHistoryRepository;
            _slideRepository = slideRepository;
        }

        public async Task<List<UserResponseDto>> Handle(GetSlideUserResponsesQuery request, CancellationToken cancellationToken)
        {
            var correctAnswer = await _slideRepository.GetCorrectAnswerAsync(request.SlideId);
            return await _sessionHistoryRepository.GetUserResponsesAsync(request.SessionId, request.SlideId, correctAnswer);
        }
    }
}
