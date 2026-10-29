using CargoTrack.DataAccess.Models.Reports;
using CargoTrack.Entity.Entities;
using System.Linq.Expressions;

namespace CargoTrack.DataAccess.Repositories.Reports
{
    using Cargo = CargoTrack.Entity.Entities.Cargo;

    public interface IReportRepository
    {
        Task<List<DailyCount>> GetDailyCargoCountsAsync(DateTime from, Expression<Func<Cargo, bool>> scope);
        Task<List<StatusCount>> GetStatusDistributionAsync(Expression<Func<Cargo, bool>> scope);
        Task<List<NamedCount>> GetTopOriginBranchesAsync(int take, DateTime from, DateTime to);
        Task<List<NamedCount>> GetTopDeliveringBranchesAsync(int take, DateTime from, DateTime to);
        Task<List<NamedCount>> GetDelayedBranchesAsync(int take, DateTime now);
        Task<List<BranchPerformanceRow>> GetBranchPerformanceAsync(DateTime from, DateTime to, DateTime now);
        Task<List<EmployeePerformanceRow>> GetEmployeePerformanceAsync(DateTime from, DateTime to);
        Task<DeliveryStatsRow> GetDeliveryStatsAsync(DateTime from, DateTime to);
    }
}
