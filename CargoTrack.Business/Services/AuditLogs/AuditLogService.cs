using CargoTrack.Business.Exceptions;
using CargoTrack.Business.Helpers;
using CargoTrack.Business.Services.CurrentUser;
using CargoTrack.DataAccess.Repositories.AuditLogs;
using CargoTrack.DTO.DTOs.AuditLogDtos;
using CargoTrack.DTO.DTOs.Common;
using CargoTrack.Entity.Entities;

namespace CargoTrack.Business.Services.AuditLogs
{
    public class AuditLogService(IAuditLogRepository _auditLogRepository,
                                 IUserContextService _userContextService) : IAuditLogService
    {
        public async Task<PagedResultDto<ResultAuditLogDto>> GetPagedAsync(AuditLogFilterDto filter)
        {
            EnsureAdmin();

            var (page, pageSize) = filter.Normalize();
            var search = string.IsNullOrWhiteSpace(filter.Search) ? null : filter.Search.Trim();
            var entityName = string.IsNullOrWhiteSpace(filter.EntityName) ? null : filter.EntityName;
            var actionType = filter.ActionType;
            var dateFrom = filter.DateFrom?.Date;
            var dateTo = filter.DateTo?.Date.AddDays(1);

            var paged = await _auditLogRepository.GetPagedAsync(page, pageSize, x =>
                (search == null || x.EntityId.Contains(search) || x.UserName!.Contains(search) || x.Description!.Contains(search))
                && (entityName == null || x.EntityName == entityName)
                && (actionType == null || x.ActionType == actionType)
                && (dateFrom == null || x.CreatedDate >= dateFrom)
                && (dateTo == null || x.CreatedDate < dateTo));

            return paged.ToPagedResult<AuditLog, ResultAuditLogDto>();
        }

        public async Task<List<string>> GetEntityNamesAsync()
        {
            EnsureAdmin();
            return await _auditLogRepository.GetEntityNamesAsync();
        }

        private void EnsureAdmin()
        {
            if (!_userContextService.IsAdmin)
            {
                throw new BusinessException("Audit kayıtlarına yalnızca sistem yöneticisi erişebilir.");
            }
        }
    }
}
