namespace CargoTrack.DTO.DTOs.TransferCenterDtos
{
    public class UpdateTransferCenterDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
        public Guid CityId { get; set; }
    }
}