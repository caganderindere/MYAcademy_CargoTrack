using CargoTrack.DTO.DTOs.Cargos;
using CargoTrack.DTO.DTOs.Common;

namespace CargoTrack.Business.Services.Cargos
{
    public interface ICargoService
    {
        Task<PagedResultDto<ResultCargoDto>> GetPagedAsync(CargoFilterDto filter);
        Task<CargoDetailDto> GetDetailAsync(Guid id);
        Task<PublicTrackingDto> GetPublicTrackingAsync(string trackCode);
        Task<PagedResultDto<ResultCargoMovementDto>> GetMovementsAsync(MovementFilterDto filter);
        Task<CargoCreateContextDto> GetCreateContextAsync();
        Task<CargoQuoteDto> QuoteAsync(CreateCargoDto createCargoDto);
        Task<string> CreateAsync(CreateCargoDto createCargoDto);
        Task ChangeStatusAsync(ChangeCargoStatusDto changeCargoStatusDto);
        Task DeliverAsync(DeliverCargoDto deliverCargoDto);
        Task ReportFailedDeliveryAsync(FailedDeliveryDto failedDeliveryDto);
    }
}
