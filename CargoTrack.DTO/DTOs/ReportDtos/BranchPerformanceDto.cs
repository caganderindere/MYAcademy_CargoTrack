namespace CargoTrack.DTO.DTOs.ReportDtos
{
    public class BranchPerformanceDto
    {
        public string BranchName { get; set; }
        public string CityName { get; set; }
        public int MovementCount { get; set; }
        public double DailyMovementAverage { get; set; }
        public int DeliveredCount { get; set; }
        public int FailedCount { get; set; }
        public int DelayedCount { get; set; }
        public double SuccessRate { get; set; }
    }
}
