namespace CargoTrack.Business.Services.CurrentUser
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        bool IsInRole(string role);
    }
}
