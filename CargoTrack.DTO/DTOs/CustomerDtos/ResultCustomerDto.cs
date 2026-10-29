namespace CargoTrack.DTO.DTOs.CustomerDtos
{
    public class ResultCustomerDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? AppUserId { get; set; }
        public bool HasAccount => AppUserId.HasValue;
        public string FullName => $"{FirstName} {LastName}";
    }
}
