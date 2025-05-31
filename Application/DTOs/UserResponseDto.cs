using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class UserResponseDto
    {
        public Guid UserId { get; set; } 
        public string UserName { get; set; } 
        public string UserAnswer { get; set; } 
        public bool IsCorrect { get; set; } 
        public TimeSpan TimeElapsed { get; set; } 
        public DateTime ResponseDate { get; set; } 
    }
}
