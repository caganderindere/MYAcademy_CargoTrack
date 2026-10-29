using CargoTrack.DTO.DTOs.Cargos;
using CargoTrack.DTO.DTOs.Common;
using CargoTrack.DTO.DTOs.DashboardDtos;

namespace CargoTrack.Business.Services.Dashboards
{
    public interface IDashboardService
    {
        Task<AdminSummaryDto> GetAdminSummaryAsync();
        Task<ManagerSummaryDto> GetManagerSummaryAsync();
        Task<UserSummaryDto> GetUserSummaryAsync();
        Task<PublicStatsDto> GetPublicStatsAsync();
        Task<List<DailyCountDto>> GetDailyTrafficAsync(int days);
        Task<List<StatusCountDto>> GetStatusDistributionAsync();
        Task<List<NamedCountDto>> GetTopBranchesAsync(int take);
        Task<List<ResultCargoDto>> GetRecentCargosAsync(int take);
        Task<List<ResultCargoMovementDto>> GetRecentMovementsAsync(int take);
        Task<CargoDetailDto?> GetLastCargoAsync();
    }
}
