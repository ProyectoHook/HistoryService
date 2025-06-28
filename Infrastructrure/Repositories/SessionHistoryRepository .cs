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
        public async Task<SlideStatsDto> GetSlideStatsAsync(Guid sessionId, int slideId, string correctAnswer)
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

        public async Task<IEnumerable<SessionHistory>> GetUserResponsesAsync(Guid sessionId, int slideId, string correctAnswer)
        {
            return await _context.SessionHistories
            .Where(h => h.SessionId == sessionId &&
                        h.OriginalSlideId == slideId &&
                        h.UserAnswer != null)
            
            .ToListAsync();
        }

        public async Task RegisterSlideChangeForUsersAsync(Guid sessionId, SlideSnapshotDto slideSnapshot, List<UserInSessionDto> users, Guid UserCreate)
        {
            var userIds = users.Select(u => u.UserId).ToList();
            var existingUsers = await _context.UserHistories
                .Where(u => userIds.Contains(u.Id))
                .Select(u => u.Id)
                .ToListAsync();

            var missingUsers = users.Where(u => !existingUsers.Contains(u.UserId)).ToList();

            foreach (var missingUser in missingUsers)
            {
                _context.UserHistories.Add(new UserHistory
                {
                    Id = missingUser.UserId,
                    Name = missingUser.Name
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
            foreach (var userId in users)
            {
                var entry = new SessionHistory
                {
                    SessionId = sessionId,
                    UserCreate = UserCreate,
                    UserHistoryId = userId.UserId,
                    SlideHistoryId = slideHistory.Id,
                    OriginalSlideId = slideHistory.OriginalSlideId,
                    Timestamp = timestamp
                };

                _context.SessionHistories.Add(entry);
            }

            await _context.SaveChangesAsync();
        }


        public async Task<Guid> RegisterUserAnswerAsync(Guid sessionId, int slideId, Guid userId, string answer, TimeSpan timeElapsed)
        {
            var entry = await _context.SessionHistories
            .Where(h => h.SessionId == sessionId &&
                        h.OriginalSlideId == slideId &&
                        h.UserHistoryId == userId)
            .OrderByDescending(h => h.Timestamp)
            .FirstOrDefaultAsync();
            //Console.WriteLine(entry.OriginalSlideId);
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

        public async Task<IEnumerable<UserHistory>> GetUsersInSessionAsync(Guid sessionId)
        {
            return await _context.SessionHistories
                .Where(sh => sh.SessionId == sessionId)
                .Select(sh => sh.UserHistory)
                .Distinct()
                
                .ToListAsync();
        }


    }
}
