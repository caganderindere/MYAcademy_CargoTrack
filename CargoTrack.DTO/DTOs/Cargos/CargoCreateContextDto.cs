namespace CargoTrack.DTO.DTOs.Cargos
{
    public class CargoCreateContextDto
    {
        public Guid? FixedOriginBranchId { get; set; }
        public string? FixedOriginBranchName { get; set; }
    }
}
