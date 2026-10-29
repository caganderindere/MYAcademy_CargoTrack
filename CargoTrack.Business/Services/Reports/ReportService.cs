using CargoTrack.Business.Exceptions;
using CargoTrack.Business.Services.CurrentUser;
using CargoTrack.DataAccess.Repositories.Reports;
using CargoTrack.DTO.DTOs.Common;
using CargoTrack.DTO.DTOs.ReportDtos;
using Mapster;

namespace CargoTrack.Business.Services.Reports
{
    public class ReportService(IReportRepository _reportRepository,
                               IUserContextService _userContextService) : IReportService
    {
        private const int DefaultPeriodDays = 30;
        private const int TopListSize = 5;

        public async Task<PerformanceReportDto> GetPerformanceReportAsync(DateTime? dateFrom, DateTime? dateTo)
        {
            if (!_userContextService.IsAdmin)
            {
                throw new BusinessException("Performans raporlarına yalnızca sistem yöneticisi erişebilir.");
            }

            var to = (dateTo ?? DateTime.Today).Date;
            var from = (dateFrom ?? to.AddDays(1 - DefaultPeriodDays)).Date;

            if (from > to)
            {
                throw new BusinessException("Başlangıç tarihi bitiş tarihinden sonra olamaz.");
            }

            var toExclusive = to.AddDays(1);
            var dayCount = (to - from).Days + 1;

            var stats = await _reportRepository.GetDeliveryStatsAsync(from, toExclusive);
            var topSending = await _reportRepository.GetTopOriginBranchesAsync(TopListSize, from, toExclusive);
            var topDelivering = await _reportRepository.GetTopDeliveringBranchesAsync(1, from, toExclusive);
            var delayed = await _reportRepository.GetDelayedBranchesAsync(TopListSize, DateTime.Now);
            var branches = await _reportRepository.GetBranchPerformanceAsync(from, toExclusive, DateTime.Now);
            var employees = await _reportRepository.GetEmployeePerformanceAsync(from, toExclusive);

            return new PerformanceReportDto
            {
                DateFrom = from,
                DateTo = to,
                DayCount = dayCount,
                TotalCargos = stats.TotalCargos,
                DeliveredCargos = stats.DeliveredCount,
                AverageDeliveryHours = stats.AverageDeliveryMinutes is null ? null : Math.Round(stats.AverageDeliveryMinutes.Value / 60, 1),
                OnTimeRate = Percentage(stats.OnTimeCount, stats.DeliveredCount),
                ReturnRate = Percentage(stats.ReturnedCount, stats.TotalCargos),
                TopSendingBranch = topSending.FirstOrDefault()?.Adapt<NamedCountDto>(),
                TopDeliveringBranch = topDelivering.FirstOrDefault()?.Adapt<NamedCountDto>(),
                TopSendingBranches = topSending.Adapt<List<NamedCountDto>>(),
                DelayedBranches = delayed.Adapt<List<NamedCountDto>>(),
                BranchPerformances = branches.Select(x => new BranchPerformanceDto
                {
                    BranchName = x.BranchName,
                    CityName = x.CityName,
                    MovementCount = x.MovementCount,
                    DailyMovementAverage = Math.Round(x.MovementCount / (double)dayCount, 1),
                    DeliveredCount = x.DeliveredCount,
                    FailedCount = x.FailedCount,
                    DelayedCount = x.DelayedCount,
                    SuccessRate = Percentage(x.DeliveredCount, x.DeliveredCount + x.FailedCount)
                }).ToList(),
                EmployeePerformances = employees.Select(x => new EmployeePerformanceDto
                {
                    FullName = x.FullName,
                    UnitName = x.UnitName,
                    Position = x.Position,
                    DeliveryCount = x.DeliveryCount,
                    FailedCount = x.FailedCount,
                    MovementCount = x.MovementCount,
                    DailyMovementAverage = Math.Round(x.MovementCount / (double)dayCount, 1),
                    SuccessRate = Percentage(x.DeliveryCount, x.DeliveryCount + x.FailedCount)
                }).ToList()
            };
        }

        private static double Percentage(int part, int total)
        {
            return total == 0 ? 0 : Math.Round(part * 100.0 / total, 1);
        }
    }
}
