namespace Application.DTOs
{
    public class SessionReconstructionDto
    {
        public Guid SessionId { get; set; }
        public List<SlideReconstructionDto> Slides { get; set; }
        public double? TotalAccuracyPercentage { get; set; } // Porcentaje total de aciertos en la sesión
        public int PresentationId { get; set; } // Identificador de la presentación


    }
}
