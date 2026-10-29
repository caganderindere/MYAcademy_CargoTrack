using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Models.Reports;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CargoTrack.DataAccess.Repositories.Reports
{
    using Cargo = CargoTrack.Entity.Entities.Cargo;

    public class ReportRepository(AppDbContext _context) : IReportRepository
    {
        private static readonly CargoStatus[] ClosedStatuses =
        {
            CargoStatus.Delivered,
            CargoStatus.ReturnInProgress,
            CargoStatus.ReturnedToSender
        };

        private static readonly CargoStatus[] ReturnStatuses =
        {
            CargoStatus.ReturnInProgress,
            CargoStatus.ReturnedToSender
        };

        public async Task<List<DailyCount>> GetDailyCargoCountsAsync(DateTime from, Expression<Func<Cargo, bool>> scope)
        {
            var rows = await _context.Cargos
                .Where(scope)
                .Where(x => x.CreatedDate >= from)
                .GroupBy(x => x.CreatedDate.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync();

            return rows.Select(x => new DailyCount(x.Date, x.Count)).ToList();
        }

        public async Task<List<StatusCount>> GetStatusDistributionAsync(Expression<Func<Cargo, bool>> scope)
        {
            var rows = await _context.Cargos
                .Where(scope)
                .GroupBy(x => x.CargoStatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            return rows.Select(x => new StatusCount(x.Status, x.Count)).ToList();
        }

        public async Task<List<NamedCount>> GetTopOriginBranchesAsync(int take, DateTime from, DateTime to)
        {
            var rows = await _context.Cargos
                .Where(x => x.CreatedDate >= from && x.CreatedDate < to)
                .GroupBy(x => x.OriginBranch.Name)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(take)
                .ToListAsync();

            return rows.Select(x => new NamedCount(x.Name, x.Count)).ToList();
        }

        public async Task<List<NamedCount>> GetTopDeliveringBranchesAsync(int take, DateTime from, DateTime to)
        {
            var rows = await _context.Deliveries
                .Where(x => x.DeliveredDate >= from && x.DeliveredDate < to)
                .GroupBy(x => x.Cargo.DestinationBranch.Name)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(take)
                .ToListAsync();

            return rows.Select(x => new NamedCount(x.Name, x.Count)).ToList();
        }

        public async Task<List<NamedCount>> GetDelayedBranchesAsync(int take, DateTime now)
        {
            var rows = await _context.Cargos
                .Where(x => (!ClosedStatuses.Contains(x.CargoStatus) && x.EstimatedDeliveryDate < now)
                         || (x.DeliveredDate != null && x.DeliveredDate > x.EstimatedDeliveryDate))
                .GroupBy(x => x.DestinationBranch.Name)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(take)
                .ToListAsync();

            return rows.Select(x => new NamedCount(x.Name, x.Count)).ToList();
        }

        public async Task<List<BranchPerformanceRow>> GetBranchPerformanceAsync(DateTime from, DateTime to, DateTime now)
        {
            var branches = await _context.Branches
                .Select(x => new { x.Id, x.Name, CityName = x.City.Name })
                .ToListAsync();

            var movements = await _context.CargoMovements
                .Where(x => x.BranchId != null && x.MovementDate >= from && x.MovementDate < to)
                .GroupBy(x => x.BranchId!.Value)
                .Select(g => new { BranchId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.BranchId, x => x.Count);

            var delivered = await _context.Deliveries
                .Where(x => x.DeliveredDate >= from && x.DeliveredDate < to)
                .GroupBy(x => x.Cargo.DestinationBranchId)
                .Select(g => new { BranchId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.BranchId, x => x.Count);

            var failed = await _context.DeliveryExceptions
                .Where(x => x.OccurredDate >= from && x.OccurredDate < to)
                .GroupBy(x => x.Cargo.DestinationBranchId)
                .Select(g => new { BranchId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.BranchId, x => x.Count);

            var delayed = await _context.Cargos
                .Where(x => (!ClosedStatuses.Contains(x.CargoStatus) && x.EstimatedDeliveryDate < now)
                         || (x.DeliveredDate != null && x.DeliveredDate > x.EstimatedDeliveryDate))
                .GroupBy(x => x.DestinationBranchId)
                .Select(g => new { BranchId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.BranchId, x => x.Count);

            return branches
                .Select(b => new BranchPerformanceRow(
                    b.Id,
                    b.Name,
                    b.CityName,
                    movements.GetValueOrDefault(b.Id),
                    delivered.GetValueOrDefault(b.Id),
                    failed.GetValueOrDefault(b.Id),
                    delayed.GetValueOrDefault(b.Id)))
                .OrderByDescending(x => x.MovementCount)
                .ToList();
        }

        public async Task<List<EmployeePerformanceRow>> GetEmployeePerformanceAsync(DateTime from, DateTime to)
        {
            var employees = await _context.Employees
                .Select(x => new
                {
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.Position,
                    UnitName = x.Branch != null ? x.Branch.Name : x.TransferCenter != null ? x.TransferCenter.Name : null
                })
                .ToListAsync();

            var deliveries = await _context.Deliveries
                .Where(x => x.DeliveredDate >= from && x.DeliveredDate < to)
                .GroupBy(x => x.EmployeeId)
                .Select(g => new { EmployeeId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.EmployeeId, x => x.Count);

            var failed = await _context.DeliveryExceptions
                .Where(x => x.EmployeeId != null && x.OccurredDate >= from && x.OccurredDate < to)
                .GroupBy(x => x.EmployeeId!.Value)
                .Select(g => new { EmployeeId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.EmployeeId, x => x.Count);

            var movements = await _context.CargoMovements
                .Where(x => x.EmployeeId != null && x.MovementDate >= from && x.MovementDate < to)
                .GroupBy(x => x.EmployeeId!.Value)
                .Select(g => new { EmployeeId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.EmployeeId, x => x.Count);

            return employees
                .Select(e => new EmployeePerformanceRow(
                    e.Id,
                    $"{e.FirstName} {e.LastName}",
                    e.UnitName,
                    e.Position,
                    deliveries.GetValueOrDefault(e.Id),
                    failed.GetValueOrDefault(e.Id),
                    movements.GetValueOrDefault(e.Id)))
                .OrderByDescending(x => x.DeliveryCount)
                .ThenByDescending(x => x.MovementCount)
                .ToList();
        }

        public async Task<DeliveryStatsRow> GetDeliveryStatsAsync(DateTime from, DateTime to)
        {
            var cargos = _context.Cargos.Where(x => x.CreatedDate >= from && x.CreatedDate < to);

            var total = await cargos.CountAsync();
            var delivered = await cargos.CountAsync(x => x.DeliveredDate != null);
            var onTime = await cargos.CountAsync(x => x.DeliveredDate != null && x.DeliveredDate <= x.EstimatedDeliveryDate);
            var returned = await cargos.CountAsync(x => ReturnStatuses.Contains(x.CargoStatus));

            var averageMinutes = await cargos
                .Where(x => x.DeliveredDate != null)
                .Select(x => (double?)EF.Functions.DateDiffMinute(x.CreatedDate, x.DeliveredDate!.Value))
                .AverageAsync();

            return new DeliveryStatsRow(total, delivered, onTime, returned, averageMinutes);
        }
    }
}
