using CargoTrack.DTO.DTOs.Common;

namespace CargoTrack.DTO.DTOs.ReportDtos
{
    public class PerformanceReportDto
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int DayCount { get; set; }
        public int TotalCargos { get; set; }
        public int DeliveredCargos { get; set; }
        public double? AverageDeliveryHours { get; set; }
        public double OnTimeRate { get; set; }
        public double ReturnRate { get; set; }
        public NamedCountDto? TopSendingBranch { get; set; }
        public NamedCountDto? TopDeliveringBranch { get; set; }
        public List<NamedCountDto> TopSendingBranches { get; set; } = new();
        public List<NamedCountDto> DelayedBranches { get; set; } = new();
        public List<BranchPerformanceDto> BranchPerformances { get; set; } = new();
        public List<EmployeePerformanceDto> EmployeePerformances { get; set; } = new();
    }
}
