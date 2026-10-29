using CargoTrack.DTO.DTOs.AuditLogDtos;
using CargoTrack.DTO.DTOs.Common;

namespace CargoTrack.Business.Services.AuditLogs
{
    public interface IAuditLogService
    {
        Task<PagedResultDto<ResultAuditLogDto>> GetPagedAsync(AuditLogFilterDto filter);
        Task<List<string>> GetEntityNamesAsync();
    }
}
