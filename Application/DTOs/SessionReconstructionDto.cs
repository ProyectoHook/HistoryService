using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class SessionReconstructionDto
    {
        public Guid SessionId { get; set; }
        public List<SlideReconstructionDto> Slides { get; set; }
        public double? TotalAccuracyPercentage { get; set; } // Porcentaje total de aciertos en la sesión
       
    }
}
