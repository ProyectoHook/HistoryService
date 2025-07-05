using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ISessionHistoryRepository
    {
        Task RegisterSlideChangeForUsersAsync(Guid sessionId, SlideSnapshotDto slideSnapshot, List<UserInSessionDto> userIds, Guid UserCreate);
        Task<Guid> RegisterUserAnswerAsync(Guid sessionId, int slideId, Guid userId, string answer, TimeSpan timeElapsed);
        Task<SlideStatsDto> GetSlideStatsAsync(Guid sessionId, int slideId, string correctAnswer);
        Task<IEnumerable<SessionHistory>> GetUserResponsesAsync(Guid sessionId, int slideId, string correctAnswer);
        Task<IEnumerable<UserHistory>> GetUsersInSessionAsync(Guid sessionId);
        Task<SessionReconstructionDto> ReconstructSessionAsync(Guid sessionId);

    }
}
