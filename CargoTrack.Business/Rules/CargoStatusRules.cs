using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Business.Rules
{
    public static class CargoStatusRules
    {
        private static readonly Dictionary<CargoStatus, CargoStatus[]> _allowedTransitions = new()
        {
            [CargoStatus.Created] = new[] { CargoStatus.AtOriginBranch },
            [CargoStatus.AtOriginBranch] = new[] { CargoStatus.InTransferCenter, CargoStatus.AtDestinationBranch },
            [CargoStatus.InTransferCenter] = new[] { CargoStatus.InTransferCenter, CargoStatus.AtDestinationBranch },
            [CargoStatus.AtDestinationBranch] = new[] { CargoStatus.OutForDelivery },
            [CargoStatus.OutForDelivery] = new[] { CargoStatus.Delivered, CargoStatus.DeliveryFailed },
            [CargoStatus.DeliveryFailed] = new[] { CargoStatus.OutForDelivery, CargoStatus.ReturnInProgress },
            [CargoStatus.ReturnInProgress] = new[] { CargoStatus.ReturnedToSender },
            [CargoStatus.Delivered] = Array.Empty<CargoStatus>(),
            [CargoStatus.ReturnedToSender] = Array.Empty<CargoStatus>()
        };

        public static bool CanTransition(CargoStatus from, CargoStatus to)
        {
            return _allowedTransitions.TryGetValue(from, out var targets) && targets.Contains(to);
        }

        public static CargoStatus[] GetNextStatuses(CargoStatus current)
        {
            return _allowedTransitions.TryGetValue(current, out var targets) ? targets : Array.Empty<CargoStatus>();
        }
    }
}