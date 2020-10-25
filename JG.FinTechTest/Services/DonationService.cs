using JG.FinTechTest.Model;
using JG.FinTechTest.Repository;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JG.FinTechTest.Services
{
    public interface IDonationService
    {
        Result<decimal> CalculateGiftAidAmount(decimal amount);
        Task<Result<int>> StoreGiftAidDeclaration(string name, string postcode, decimal amount);
    }

    public class DonationService : IDonationService
    {
        private AppSettings _settings;
        private IRepository<GiftAidDeclaration> _repository;

        public DonationService(IOptions<AppSettings> settings, IRepository<GiftAidDeclaration> repository)
        {
            _settings = settings.Value;
            _repository = repository;
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

        public async Task<Result<int>> StoreGiftAidDeclaration(string name, string postcode, decimal amount)
        {
            if (string.IsNullOrEmpty(name))
                return Result.Fail<int>(default, "Name can not be an empty string", ErrorType.InvalidParameter);
            if (string.IsNullOrEmpty(postcode))
                return Result.Fail<int>(default, "PostCode can not be an empty string", ErrorType.InvalidParameter);

            var validPostcode = ValidatePostCode(postcode);
            if (validPostcode.IsFailure)
                return Result.Fail(default(int), validPostcode.Error, validPostcode.ErrorType);

            var declaration = new GiftAidDeclaration { Name = name, PostCode = postcode, Amount = amount };

            Result<int> result = null;
            await Task.Run(() => result = _repository.InsertRecord(declaration));

            if (result == null || result.IsFailure)
                return Result.Fail<int>(0, "Unable to create GiftAid Declaration", ErrorType.FailedToInsertToDatabase);

            return Result.Ok(result.Value);
        }

        private Result ValidatePostCode(string postcode)
        {
            string regex = @"([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9][A-Za-z]?))))\s?[0-9][A-Za-z]{2})";

            if (!Regex.Match(postcode, regex).Success)
                return Result.Fail("Invalid UK Post Code", ErrorType.InvalidPostCode);

            return Result.Ok();
        }
    }
}
