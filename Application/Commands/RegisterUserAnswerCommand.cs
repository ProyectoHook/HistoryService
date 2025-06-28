using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MediatR;

namespace Application.Commands
{
    public class RegisterUserAnswerCommand : IRequest<Guid>
    {
        public Guid SessionId { get; set; }
        public int SlideId { get; set; }
        public Guid UserId { get; set; }
        public string Answer { get; set; }
        [JsonIgnore]
        public TimeSpan TimeElapsed { get; set; } = TimeSpan.Zero;
    }
}
