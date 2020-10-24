using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JG.FinTechTest.Services
{
    public interface IDonationService
    {
        Result<decimal> CalculateGiftAidAmount(decimal amount);
    }

    public class DonationService : IDonationService
    {
        private AppSettings _settings;

        public DonationService(IOptions<AppSettings> settings)
        {
            _settings = settings.Value;
        }

        public Result<decimal> CalculateGiftAidAmount(decimal amount)
        {
            if (amount <= _settings.MinimumDonation)
                return Result.Fail<decimal>(default, $"Donation amount '£{amount}' is below minimum (£{_settings.MinimumDonation})", ErrorType.DonationAmountBelowMinimum);
            if (amount >= _settings.MaximumDonation)
                return Result.Fail<decimal>(default, $"Donation amount '£{amount}' is above maximum (£{_settings.MaximumDonation})", ErrorType.DonationAmountAboveMaximum);

            var giftAid = amount * (_settings.TaxRate / (100 - _settings.TaxRate));

            return Result.Ok(giftAid);
        }
    }
}
