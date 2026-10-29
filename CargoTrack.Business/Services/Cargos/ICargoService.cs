using CargoTrack.DTO.DTOs.Cargos;

namespace CargoTrack.Business.Services.Cargos
{
    public interface ICargoService
    {
        Task<List<ResultCargoDto>> GetListAsync();
        Task<string> CreateAsync(CreateCargoDto createCargoDto);
        Task ChangeStatusAsync(ChangeCargoStatusDto changeCargoStatusDto);
        Task DeliverAsync(DeliverCargoDto deliverCargoDto);
        Task ReportFailedDeliveryAsync(FailedDeliveryDto failedDeliveryDto);
    }
}
