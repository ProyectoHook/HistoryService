using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using MediatR;

namespace Application.Querys
{
    public class GetUsersInSessionQuery : IRequest<List<UserInSessionDto>>
    {
        public Guid SessionId { get; set; }
    }
}
