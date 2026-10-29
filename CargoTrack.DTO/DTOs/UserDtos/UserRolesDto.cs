namespace CargoTrack.DTO.DTOs.UserDtos
{
    public class UserRolesDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public List<RoleAssignDto> Roles { get; set; } = new();
    }
}
