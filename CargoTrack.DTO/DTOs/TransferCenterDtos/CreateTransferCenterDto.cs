namespace CargoTrack.DTO.DTOs.TransferCenterDtos
{
    public class CreateTransferCenterDto
    {
        public string Name { get; set; }
        public string? Address { get; set; }
        public Guid CityId { get; set; }
    }
}