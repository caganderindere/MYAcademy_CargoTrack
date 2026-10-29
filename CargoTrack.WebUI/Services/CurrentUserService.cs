using CargoTrack.Business.Services.CurrentUser;
using System.Security.Claims;

namespace CargoTrack.WebUI.Services
{
    public class CurrentUserService(IHttpContextAccessor _httpContextAccessor) : ICurrentUserService
    {
        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public Guid? UserId
        {
            get
            {
                var value = User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.TryParse(value, out var userId) ? userId : null;
            }
        }

        public bool IsInRole(string role)
        {
            return User?.IsInRole(role) ?? false;
        }
    }
}
