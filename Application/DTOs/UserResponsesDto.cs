using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class UserResponsesDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Answer { get; set; }
        public TimeSpan? TimeElapsed { get; set; }
        public bool? IsCorrect { get; set; }
    }
}
