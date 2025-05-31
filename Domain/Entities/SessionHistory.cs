using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SessionHistory
    {
        public int Id { get; set; }

        public int SessionId { get; set; }
        public Guid UserCreate { get; set; }

        public Guid UserHistoryId { get; set; }
        public UserHistory UserHistory { get; set; }
        public int OriginalSlideId { get; set; }
        public int SlideHistoryId { get; set; }
        public SlideHistory SlideHistory { get; set; }

        public DateTime Timestamp { get; set; }

        public string? UserAnswer { get; set; }      // Solo si responde
        public TimeSpan? TimeElapsed { get; set; }   // Tiempo en responder
    }

}
