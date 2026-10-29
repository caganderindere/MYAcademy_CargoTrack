namespace CargoTrack.DTO.DTOs.BranchDtos
{
    public class ResultBranchDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public Guid CityId { get; set; }
        public string CityName { get; set; }
        public Guid? TransferCenterId { get; set; }
        public string? TransferCenterName { get; set; }
    }
}
