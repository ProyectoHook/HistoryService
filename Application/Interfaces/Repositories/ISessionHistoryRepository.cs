using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces.Repositories
{
    public interface ISessionHistoryRepository
    {
        Task RegisterSlideChangeForUsersAsync(int sessionId, SlideSnapshotDto slideSnapshot, List<Guid> userIds, Guid UserCreate);
        Task<Guid> RegisterUserAnswerAsync(int sessionId, int slideId, Guid userId, string answer, TimeSpan timeElapsed);
        Task<SlideStatsDto> GetSlideStatsAsync(int sessionId, int slideId, string correctAnswer);
        Task<List<UserResponseDto>> GetUserResponsesAsync(int sessionId, int slideId, string correctAnswer);
        
    }
}
