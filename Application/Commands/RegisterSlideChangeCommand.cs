using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using MediatR;

namespace Application.Commands
{
    public class RegisterSlideChangeCommand : IRequest<Unit>
    {
        public Guid SessionId { get; set; }
        public SlideSnapshotDto SlideSnapshot { get; set; }
        public List<UserInSessionDto> ConnectedUserIds { get; set; }
    }
}
