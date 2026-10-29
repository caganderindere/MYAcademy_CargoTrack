namespace CargoTrack.DTO.DTOs.DashboardDtos
{
    public class UserSummaryDto
    {
        public string FullName { get; set; }
        public int ActiveCargos { get; set; }
        public int IncomingActive { get; set; }
        public int OutgoingActive { get; set; }
        public int DeliveredCargos { get; set; }
        public int TotalCargos { get; set; }
    }
}
