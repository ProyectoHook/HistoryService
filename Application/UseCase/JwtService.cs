using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Http;

namespace Application.UseCase
{
    public class JwtService : IJwtService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public Guid GetUserIdFromToken()
        {
            
            var httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext no disponible");

            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("Token inválido o sin userId");
            return Guid.Parse(userId);
        }
    }
}
