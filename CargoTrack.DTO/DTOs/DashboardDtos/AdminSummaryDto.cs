namespace CargoTrack.DTO.DTOs.DashboardDtos
{
    public class AdminSummaryDto
    {
        public int TotalCargos { get; set; }
        public int ActiveCargos { get; set; }
        public int DelayedCargos { get; set; }
        public int DeliveredCargos { get; set; }
        public double DeliveryRate { get; set; }
        public int ActiveBranches { get; set; }
        public int ActiveEmployees { get; set; }
        public int TotalCustomers { get; set; }
        public int TodayCargos { get; set; }
    }
}
