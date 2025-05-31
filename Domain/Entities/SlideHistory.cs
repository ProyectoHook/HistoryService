using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SlideHistory
    {
        public int Id { get; set; }
        public int OriginalSlideId { get; set; }

        public string? Ask { get; set; }           
        public string? AnswerCorrect { get; set; } 

        public ICollection<OptionHistory> Options { get; set; } 
        public ICollection<SessionHistory> SessionHistories { get; set; }
    }
}
