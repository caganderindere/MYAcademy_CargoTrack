namespace CargoTrack.DataAccess.Auditing
{
    public interface IAuditUserProvider
    {
        Guid? UserId { get; }
        string? UserName { get; }
    }
}
