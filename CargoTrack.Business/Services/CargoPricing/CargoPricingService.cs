using CargoTrack.Business.Exceptions;
using CargoTrack.DataAccess.Repositories.Branches;
using CargoTrack.DataAccess.Repositories.CargoPrices;
using CargoTrack.DTO.DTOs.Cargos;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Business.Services.CargoPricing
{
    public class CargoPricingService(ICargoPriceRepository _cargoPriceRepository,
                                     IBranchRepository _branchRepository) : ICargoPricingService
    {
        public async Task<(decimal Desi, decimal Price)> CalculateAsync(CreateCargoDto createCargoDto)
        {
            var desi = CalculateDesi(createCargoDto.Width, createCargoDto.Length, createCargoDto.Height);
            var chargeableDesi = Math.Max(desi, createCargoDto.Weight);

            var distanceZone = await GetDistanceZoneAsync(createCargoDto.OriginBranchId, createCargoDto.DestinationBranchId);

            if (createCargoDto.DeliveryType == DeliveryType.SameDay && distanceZone != DistanceZone.SameCity)
            {
                throw new BusinessException("Aynı gün teslimat sadece şehir içi gönderilerde kullanılabilir.");
            }

            var tariff = await _cargoPriceRepository.GetActivePriceAsync(distanceZone, chargeableDesi);

            if (tariff is null)
            {
                throw new BusinessException("Bu gönderi ölçüleri için tanımlı bir fiyat tarifesi bulunmamaktadır.");
            }

            var price = tariff.Price * GetDeliveryTypeMultiplier(createCargoDto.DeliveryType);
            price += GetAdditionalServicesFee(createCargoDto.AdditionalServices);

            return (desi, Math.Round(price, 2));
        }

        private static decimal CalculateDesi(decimal width, decimal length, decimal height)
        {
            return Math.Round(width * length * height / 3000, 2);
        }

        private async Task<DistanceZone> GetDistanceZoneAsync(Guid originBranchId, Guid destinationBranchId)
        {
            var originBranch = await _branchRepository.GetByIdAsync(originBranchId);
            var destinationBranch = await _branchRepository.GetByIdAsync(destinationBranchId);

            if (originBranch is null || destinationBranch is null)
            {
                throw new BusinessException("Çıkış veya varış şubesi bulunamadı.");
            }

            if (originBranch.CityId == destinationBranch.CityId)
            {
                return DistanceZone.SameCity;
            }

            if (originBranch.City.Region == destinationBranch.City.Region)
            {
                return DistanceZone.SameRegion;
            }

            return DistanceZone.InterRegion;
        }

        private static decimal GetDeliveryTypeMultiplier(DeliveryType deliveryType)
        {
            return deliveryType switch
            {
                DeliveryType.Express => 1.5m,
                DeliveryType.SameDay => 2.0m,
                _ => 1.0m
            };
        }

        private static decimal GetAdditionalServicesFee(AdditionalService additionalServices)
        {
            decimal fee = 0;

            if (additionalServices.HasFlag(AdditionalService.Insurance)) fee += 25;
            if (additionalServices.HasFlag(AdditionalService.CashOnDelivery)) fee += 15;
            if (additionalServices.HasFlag(AdditionalService.SmsNotification)) fee += 5;
            if (additionalServices.HasFlag(AdditionalService.WeekendDelivery)) fee += 30;

            return fee;
        }
    }
}