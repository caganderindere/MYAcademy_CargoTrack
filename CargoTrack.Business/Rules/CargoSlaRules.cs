using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using System.Linq.Expressions;

namespace CargoTrack.Business.Rules
{
    public static class CargoSlaRules
    {
        public static readonly CargoStatus[] FinishedStatuses =
        {
            CargoStatus.Delivered,
            CargoStatus.ReturnedToSender
        };

        public static readonly CargoStatus[] ClosedStatuses =
        {
            CargoStatus.Delivered,
            CargoStatus.ReturnInProgress,
            CargoStatus.ReturnedToSender
        };

        public static readonly CargoStatus[] ProblematicStatuses =
        {
            CargoStatus.DeliveryFailed,
            CargoStatus.ReturnInProgress
        };

        public static bool IsFinished(CargoStatus status)
        {
            return FinishedStatuses.Contains(status);
        }

        public static bool IsDelayed(CargoStatus status, DateTime estimatedDeliveryDate, DateTime? deliveredDate, DateTime now)
        {
            if (deliveredDate is not null)
            {
                return deliveredDate > estimatedDeliveryDate;
            }

            return !ClosedStatuses.Contains(status) && estimatedDeliveryDate < now;
        }

        public static Expression<Func<Cargo, bool>> DelayedExpression(DateTime now)
        {
            return x => !ClosedStatuses.Contains(x.CargoStatus) && x.EstimatedDeliveryDate < now;
        }

        public static Expression<Func<Cargo, bool>> ActiveExpression()
        {
            return x => !FinishedStatuses.Contains(x.CargoStatus);
        }
    }
}
