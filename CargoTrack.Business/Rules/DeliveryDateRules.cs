using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Business.Rules
{
    public static class DeliveryDateRules
    {
        public static DateTime CalculateEstimatedDeliveryDate(DateTime shipmentDate,
                                                              DeliveryType deliveryType,
                                                              AdditionalService additionalServices)
        {
            if (deliveryType == DeliveryType.SameDay)
            {
                return shipmentDate.Date.AddHours(21);
            }

            var businessDays = deliveryType == DeliveryType.Express ? 1 : 3;
            var includeWeekends = additionalServices.HasFlag(AdditionalService.WeekendDelivery);

            var date = shipmentDate.Date;

            while (businessDays > 0)
            {
                date = date.AddDays(1);

                var isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;

                if (isWeekend && !includeWeekends)
                {
                    continue;
                }

                businessDays--;
            }

            return date.AddHours(18);
        }
    }
}