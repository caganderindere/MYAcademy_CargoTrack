using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DTO.DTOs.ReportDtos
{
    public class EmployeePerformanceDto
    {
        public string FullName { get; set; }
        public string? UnitName { get; set; }
        public EmployeePosition Position { get; set; }
        public int DeliveryCount { get; set; }
        public int FailedCount { get; set; }
        public int MovementCount { get; set; }
        public double DailyMovementAverage { get; set; }
        public double SuccessRate { get; set; }
    }
}
