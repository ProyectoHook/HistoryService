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
    class GetSlideResponseStatsHandler : IRequestHandler<GetSlideResponseStatsQuery, SlideStatsDto>
    {
        private readonly ISessionHistoryRepository _sessionHistoryRepository;
        private readonly ISlideRepository _slideRepository;

        public GetSlideResponseStatsHandler(
            ISessionHistoryRepository sessionHistoryRepository,
            ISlideRepository slideRepository)
        {
            _sessionHistoryRepository = sessionHistoryRepository;
            _slideRepository = slideRepository;
        }

        public async Task<SlideStatsDto> Handle(GetSlideResponseStatsQuery request, CancellationToken cancellationToken)
        {
            var correctAnswer = await _slideRepository.GetCorrectAnswerAsync(request.SlideId);
            var stats = await _sessionHistoryRepository.GetSlideStatsAsync(request.SessionId, request.SlideId, correctAnswer);

            return stats ?? new SlideStatsDto();
        }
    }
}
