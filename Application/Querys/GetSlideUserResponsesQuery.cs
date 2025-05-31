using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using MediatR;

namespace Application.Querys
{
    public class GetSlideUserResponsesQuery : IRequest<List<UserResponseDto>>
    {
        public int SessionId { get; set; }
        public int SlideId { get; set; }
    }
}
