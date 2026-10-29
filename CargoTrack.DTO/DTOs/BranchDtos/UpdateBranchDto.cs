namespace CargoTrack.DTO.DTOs.BranchDtos
{
    public class UpdateBranchDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public Guid CityId { get; set; }
        public Guid? TransferCenterId { get; set; }
    }
}
