namespace CargoTrack.DTO.DTOs.Common
{
    public interface IPagedResult
    {
        int Page { get; }
        int PageSize { get; }
        int TotalCount { get; }
        int TotalPages { get; }
        bool HasPrevious { get; }
        bool HasNext { get; }
    }
}
