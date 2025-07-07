using Application.DTOs;
using MediatR;

namespace Application.Querys
{
    public record GetSessions(int presentationId) : IRequest<SessionsResponse>;
}
