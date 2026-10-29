namespace CargoTrack.DTO.DTOs.DashboardDtos
{
    public class ManagerSummaryDto
    {
        public string UnitName { get; set; }
        public string EmployeeFullName { get; set; }
        public int TodayCreated { get; set; }
        public int TodayIncoming { get; set; }
        public int TodayOutgoing { get; set; }
        public int OutForDelivery { get; set; }
        public int DeliveredToday { get; set; }
        public int DelayedCargos { get; set; }
        public int ProblematicCargos { get; set; }
        public int ActiveCargos { get; set; }
    }
}
