using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class SlideSnapshotDto
    {
        public int Id { get; set; } // Id del slide original
        public string? Ask { get; set; }
        public string? AnswerCorrect { get; set; }
        public List<string> Options { get; set; }
    }
}
