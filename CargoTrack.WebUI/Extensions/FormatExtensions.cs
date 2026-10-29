namespace CargoTrack.WebUI.Extensions
{
    public static class FormatExtensions
    {
        public static string ToMoney(this decimal value)
        {
            return value.ToString("N2") + " ₺";
        }

        public static string ToShortDateTime(this DateTime value)
        {
            return value.ToString("dd MMM yyyy, HH:mm");
        }

        public static string ToShortDateTime(this DateTime? value)
        {
            return value?.ToString("dd MMM yyyy, HH:mm") ?? "-";
        }

        public static string Initials(this string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "?";
            }

            var parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return string.Concat(parts.Take(2).Select(x => char.ToUpperInvariant(x[0])));
        }
    }
}
