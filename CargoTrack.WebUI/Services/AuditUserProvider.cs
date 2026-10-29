using CargoTrack.DataAccess.Auditing;
using System.Security.Claims;

namespace CargoTrack.WebUI.Services
{
    public class AuditUserProvider(IHttpContextAccessor _httpContextAccessor) : IAuditUserProvider
    {
        public Guid? UserId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.TryParse(value, out var userId) ? userId : null;
            }
        }

        public string? UserName => _httpContextAccessor.HttpContext?.User.Identity?.Name;
    }
}
