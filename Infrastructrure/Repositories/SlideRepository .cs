using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Infrastructrure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructrure.Repositories
{
    public class SlideRepository : ISlideRepository
    {
        private readonly ServiceContext _context;

        public SlideRepository(ServiceContext context)
        {
            _context = context;
        }

        public async Task<string> GetCorrectAnswerAsync(int slideId)
        {
            return await _context.SlideHistories
                .Where(s => s.OriginalSlideId == slideId)
                .Select(s => s.AnswerCorrect)
                .FirstOrDefaultAsync();
        }
    }
}
