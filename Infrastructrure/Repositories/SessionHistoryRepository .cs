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

            // Validar si ya existe este slide en esta sesión
            var slideAlreadyExists = await _context.SessionHistories
                .AnyAsync(sh => sh.SessionId == sessionId && sh.OriginalSlideId == slideSnapshot.Id);

            if (slideAlreadyExists)
            {
                return; // Evitar duplicados
            }

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

        public async Task<SessionReconstructionDto> ReconstructSessionAsync(Guid sessionId)
        {
            // Primero obtener solo los IDs de los slides distintos
            var distinctSlideIds = await _context.SessionHistories
                .Where(sh => sh.SessionId == sessionId)
                .Select(sh => sh.SlideHistoryId)
                .Distinct()
                .ToListAsync();

            // Luego obtener la información completa de cada slide
            var slides = new List<dynamic>();
            foreach (var slideId in distinctSlideIds)
            {
                var slide = await _context.SlideHistories
                    .Where(s => s.Id == slideId)
                    .Select(s => new
                    {
                        SlideHistoryId = s.Id,
                        s.OriginalSlideId,
                        s.Ask,
                        s.AnswerCorrect,
                        Options = s.Options.Select(o => o.OptionText).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (slide != null)
                {
                    slides.Add(slide);
                }
            }

            // Obtener todos los usuarios únicos que participaron en la sesión
            var users = await _context.SessionHistories
                .Where(sh => sh.SessionId == sessionId)
                .Select(sh => new
                {
                    sh.UserHistoryId,
                    sh.UserHistory.Name
                })
                .Distinct()
                .ToListAsync();

            // Obtener todas las respuestas de los usuarios para cada slide
            var userResponses = await _context.SessionHistories
                .Where(sh => sh.SessionId == sessionId)
                .Select(sh => new
                {
                    sh.SlideHistoryId,
                    sh.SlideHistory.OriginalSlideId,
                    sh.UserHistoryId,
                    sh.UserAnswer,
                    sh.TimeElapsed,
                    IsCorrect = sh.UserAnswer != null && sh.UserAnswer == sh.SlideHistory.AnswerCorrect
                })
                .ToListAsync();

            // Calcular estadísticas por slide
            var slideStats = slides.Select(s => {
                var responsesForSlide = userResponses.Where(ur => ur.SlideHistoryId == s.SlideHistoryId).ToList();
                var totalResponses = responsesForSlide.Count(ur => ur.UserAnswer != null);
                var correctResponses = responsesForSlide.Count(ur => ur.IsCorrect);

                double? accuracy = totalResponses > 0 ? (double)correctResponses / totalResponses * 100 : null;

                return new
                {
                    s.SlideHistoryId,
                    Accuracy = accuracy
                };
            }).ToList();

            // Calcular estadísticas totales
            var allResponses = userResponses.Where(ur => ur.UserAnswer != null).ToList();
            var totalQuestions = slides.Count;
            var totalCorrect = allResponses.Count(ur => ur.IsCorrect);
            var totalAnswered = allResponses.Count;

            double? totalAccuracy = totalAnswered > 0 ? (double)totalCorrect / totalAnswered * 100 : null;
            

            // Construir el objeto de resultado
            var result = new SessionReconstructionDto
            {
                SessionId = sessionId,
                Slides = slides.Select(s => new SlideReconstructionDto
                {
                    SlideHistoryId = s.SlideHistoryId,
                    OriginalSlideId = s.OriginalSlideId,
                    Question = s.Ask,
                    CorrectAnswer = s.AnswerCorrect,
                    Options = s.Options,
                    AccuracyPercentage = slideStats.FirstOrDefault(ss => ss.SlideHistoryId == s.SlideHistoryId)?.Accuracy,
                    UserResponses = users.Select(u => new UserResponsesDto
                    {
                        UserId = u.UserHistoryId,
                        UserName = u.Name,
                        Answer = userResponses
                            .FirstOrDefault(ur => ur.SlideHistoryId == s.SlideHistoryId && ur.UserHistoryId == u.UserHistoryId)?
                            .UserAnswer,
                        TimeElapsed = userResponses
                            .FirstOrDefault(ur => ur.SlideHistoryId == s.SlideHistoryId && ur.UserHistoryId == u.UserHistoryId)?
                            .TimeElapsed,
                        IsCorrect = userResponses
                            .FirstOrDefault(ur => ur.SlideHistoryId == s.SlideHistoryId && ur.UserHistoryId == u.UserHistoryId)?
                            .IsCorrect
                    }).ToList()
                }).ToList(),
                TotalAccuracyPercentage = totalAccuracy
            };

            return result;
        }


    }
}
