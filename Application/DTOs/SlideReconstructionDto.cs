using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class SlideReconstructionDto
    {
        public int SlideHistoryId { get; set; }
        public int OriginalSlideId { get; set; }
        public string Question { get; set; }
        public string CorrectAnswer { get; set; }
        public List<string> Options { get; set; }
        public List<UserResponsesDto> UserResponses { get; set; }
        public double? AccuracyPercentage { get; set; } // Porcentaje de aciertos para esta pregunta
    }
}
