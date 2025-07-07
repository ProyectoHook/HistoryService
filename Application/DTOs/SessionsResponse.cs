using Microsoft.Extensions.Options;

namespace Application.DTOs
{
    public class SessionsResponse
    {
        public required List<SessionDate> Sessions { get; set; }
    }
}
