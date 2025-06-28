using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Querys.Handler
{
    class GetUsersInSessionHandler : IRequestHandler<GetUsersInSessionQuery, List<UserInSessionDto>>
    {
        private readonly ISessionHistoryRepository _repository;

        public GetUsersInSessionHandler(ISessionHistoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<UserInSessionDto>> Handle(GetUsersInSessionQuery request, CancellationToken cancellationToken)
        {
            var users = await _repository.GetUsersInSessionAsync(request.SessionId);

            return users.Select(u => new UserInSessionDto
            {
                UserId = u.Id,
                Name = u.Name
            }).ToList();
        }
    }
}
