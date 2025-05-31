using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.Repositories;

using Domain.Entities;
using Infrastructrure.Persistence;
using Microsoft.EntityFrameworkCore;
using Application.DTOs;
using System.Reflection;

namespace Infrastructrure.Repositories
{
    public class SessionHistoryRepository : ISessionHistoryRepository
    {
        private readonly ServiceContext _context;

        public SessionHistoryRepository(ServiceContext context)
        {
            _context = context;
        }
        public async Task<SlideStatsDto> GetSlideStatsAsync(int sessionId, int slideId, string correctAnswer)
        {
            return await _context.SessionHistories
            .Where(h => h.SessionId == sessionId && h.OriginalSlideId == slideId && h.UserAnswer != null)
            .GroupBy(_ => 1)
            .Select(g => new SlideStatsDto
            {
                Total = g.Count(),
                Correct = g.Count(x => x.UserAnswer == correctAnswer),
                Incorrect = g.Count(x => x.UserAnswer != correctAnswer)
            })
            .FirstOrDefaultAsync();
        }

        public async Task<List<UserResponseDto>> GetUserResponsesAsync(int sessionId, int slideId, string correctAnswer)
        {
            return await _context.SessionHistories
            .Where(h => h.SessionId == sessionId &&
                        h.OriginalSlideId == slideId &&
                        h.UserAnswer != null)
            .Select(h => new UserResponseDto
            {
                UserId = h.UserHistory.Id,
                UserName = h.UserHistory.Name,
                UserAnswer = h.UserAnswer,
                IsCorrect = h.UserAnswer == correctAnswer,
                TimeElapsed = h.TimeElapsed ?? TimeSpan.Zero, 
                ResponseDate = h.Timestamp,
            })
            .ToListAsync();
        }

        public async Task RegisterSlideChangeForUsersAsync(int sessionId, SlideSnapshotDto slideSnapshot, List<Guid> userIds, Guid UserCreate)
        {

            var existingUsers = await _context.UserHistories
                .Where(u => userIds.Contains(u.Id))
                .Select(u => u.Id)
                .ToListAsync();

            var missingUserIds = userIds.Except(existingUsers).ToList();

            foreach (var missingId in missingUserIds)
            {
                _context.UserHistories.Add(new UserHistory
                {
                    Id = missingId,
                    Name = $"Usuario {missingId}"
                });
            }
            await _context.SaveChangesAsync();

            var timestamp = DateTime.UtcNow;

           
            var slideHistory = new SlideHistory
            {
                OriginalSlideId = slideSnapshot.Id,
                Ask = slideSnapshot.Ask,
                AnswerCorrect = slideSnapshot.AnswerCorrect,
                Options = slideSnapshot.Options.Select(opt => new OptionHistory
                {
                    OptionText = opt
                }).ToList()
            };

            _context.SlideHistories.Add(slideHistory);
            await _context.SaveChangesAsync(); // Para generar SlideHistory.Id
            
            // Crear un registro por cada usuario conectado
            foreach (var userId in userIds)
            {
                var entry = new SessionHistory
                {
                    SessionId = sessionId,
                    UserCreate = UserCreate,
                    UserHistoryId = userId,
                    SlideHistoryId = slideHistory.Id,
                    OriginalSlideId = slideHistory.OriginalSlideId,
                    Timestamp = timestamp
                };

                _context.SessionHistories.Add(entry);
            }

            await _context.SaveChangesAsync();
        }


        public async Task<Guid> RegisterUserAnswerAsync(int sessionId, int slideId, Guid userId, string answer, TimeSpan timeElapsed)
        {
            var entry = await _context.SessionHistories
            .Where(h => h.SessionId == sessionId &&
                        h.OriginalSlideId == slideId &&
                        h.UserHistoryId == userId)
            .OrderByDescending(h => h.Timestamp)
            .FirstOrDefaultAsync();
            Console.WriteLine(entry.OriginalSlideId);
            if (entry != null)
            {
                entry.UserAnswer = answer;
                entry.TimeElapsed = timeElapsed;
            }
            else
            {
                _context.SessionHistories.Add(new SessionHistory
                {
                    SessionId = sessionId,
                    SlideHistoryId = slideId,
                    UserHistoryId = userId,
                    UserAnswer = answer,
                    TimeElapsed = timeElapsed,
                    Timestamp = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return userId;
        }

        
    }
}
