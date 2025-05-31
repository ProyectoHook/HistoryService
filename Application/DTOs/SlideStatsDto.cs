using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class SlideStatsDto
    {
        public int Total { get; set; }  // Total de respuestas recibidas
        public int Correct { get; set; } // Numero de respuestas correctas
        public int Incorrect { get; set; } // Numero de respuestas incorrectas
        public double CorrectPercentage => Total > 0
            ? Math.Round((Correct / (double)Total) * 100, 2)
            : 0; // Porcentaje de aciertos
    }
}
