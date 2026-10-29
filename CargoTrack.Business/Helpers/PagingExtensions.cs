using CargoTrack.DataAccess.Common;
using CargoTrack.DTO.DTOs.Common;
using Mapster;

namespace CargoTrack.Business.Helpers
{
    public static class PagingExtensions
    {
        private const int DefaultPageSize = 10;
        private const int MaxPageSize = 50;

        public static (int Page, int PageSize) Normalize(this PageRequestDto request)
        {
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize < 1 ? DefaultPageSize : Math.Min(request.PageSize, MaxPageSize);
            return (page, pageSize);
        }

        public static PagedResultDto<TDestination> ToPagedResult<TSource, TDestination>(this PagedList<TSource> source)
        {
            return new PagedResultDto<TDestination>
            {
                Items = source.Items.Adapt<List<TDestination>>(),
                Page = source.Page,
                PageSize = source.PageSize,
                TotalCount = source.TotalCount
            };
        }
    }
}
