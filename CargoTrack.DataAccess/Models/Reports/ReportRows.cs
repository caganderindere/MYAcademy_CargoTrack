using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DataAccess.Models.Reports
{
    public record NamedCount(string Name, int Count);

    public record DailyCount(DateTime Date, int Count);

    public record StatusCount(CargoStatus Status, int Count);

    public record BranchPerformanceRow(Guid BranchId, string BranchName, string CityName, int MovementCount, int DeliveredCount, int FailedCount, int DelayedCount);

    public record EmployeePerformanceRow(Guid EmployeeId, string FullName, string? UnitName, EmployeePosition Position, int DeliveryCount, int FailedCount, int MovementCount);

    public record DeliveryStatsRow(int TotalCargos, int DeliveredCount, int OnTimeCount, int ReturnedCount, double? AverageDeliveryMinutes);
}
