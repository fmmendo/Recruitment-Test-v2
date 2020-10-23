using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JG.FinTechTest.Services
{
    public interface IDonationService
    {
        decimal CalculateGiftAidAmount(decimal amount);
    }

    public class DonationService : IDonationService
    {
        private AppSettings _settings;

        public DonationService(IOptions<AppSettings> settings)
        {
            _settings = settings.Value;
        }

        public decimal CalculateGiftAidAmount(decimal amount)
        {
            return amount * (_settings.TaxRate / (100 - _settings.TaxRate));
        }
    }
}
