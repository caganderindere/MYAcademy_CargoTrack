using CargoTrack.Entity.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CargoTrack.WebUI.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var type = value.GetType();

            if (type.GetCustomAttribute<FlagsAttribute>() is not null && Convert.ToInt64(value) != 0)
            {
                var names = Enum.GetValues(type)
                    .Cast<Enum>()
                    .Where(x => Convert.ToInt64(x) != 0 && value.HasFlag(x))
                    .Select(GetSingleName);

                return string.Join(", ", names);
            }

            return GetSingleName(value);
        }

        public static string BadgeClass(this CargoStatus status)
        {
            return status switch
            {
                CargoStatus.Created => "st-created",
                CargoStatus.AtOriginBranch => "st-origin",
                CargoStatus.InTransferCenter => "st-transfer",
                CargoStatus.AtDestinationBranch => "st-destination",
                CargoStatus.OutForDelivery => "st-out",
                CargoStatus.Delivered => "st-delivered",
                CargoStatus.DeliveryFailed => "st-failed",
                CargoStatus.ReturnInProgress => "st-return",
                _ => "st-returned"
            };
        }

        public static string Icon(this CargoStatus status)
        {
            return status switch
            {
                CargoStatus.Created => "bi-box-seam",
                CargoStatus.AtOriginBranch => "bi-shop",
                CargoStatus.InTransferCenter => "bi-diagram-3",
                CargoStatus.AtDestinationBranch => "bi-geo-alt",
                CargoStatus.OutForDelivery => "bi-truck",
                CargoStatus.Delivered => "bi-check2-circle",
                CargoStatus.DeliveryFailed => "bi-exclamation-octagon",
                CargoStatus.ReturnInProgress => "bi-arrow-counterclockwise",
                _ => "bi-arrow-return-left"
            };
        }

        public static string BadgeClass(this AuditActionType actionType)
        {
            return actionType switch
            {
                AuditActionType.Create => "tag-success",
                AuditActionType.Update => "tag-info",
                AuditActionType.Delete => "tag-delay",
                _ => "tag-warning"
            };
        }

        private static string GetSingleName(Enum value)
        {
            var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            return member?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? value.ToString();
        }
    }
}
