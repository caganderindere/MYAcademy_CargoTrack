using Microsoft.AspNetCore.Mvc.Rendering;

namespace CargoTrack.WebUI.Extensions
{
    public static class SelectListExtensions
    {
        public static List<SelectListItem> ToSelectList<T>(this IEnumerable<T> source, Func<T, Guid> value, Func<T, string> text, Guid? selected = null)
        {
            return source
                .Select(x => new SelectListItem
                {
                    Value = value(x).ToString(),
                    Text = text(x),
                    Selected = selected is not null && value(x) == selected
                })
                .ToList();
        }

        public static List<SelectListItem> EnumSelect<T>(T? selected = null) where T : struct, Enum
        {
            return Enum.GetValues<T>()
                .Select(x => new SelectListItem
                {
                    Value = Convert.ToInt32(x).ToString(),
                    Text = x.GetDisplayName(),
                    Selected = selected.HasValue && selected.Value.Equals(x)
                })
                .ToList();
        }
    }
}
