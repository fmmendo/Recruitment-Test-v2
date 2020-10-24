using JG.FinTechTest.Services;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace JG.FinTechTest.Tests.Services
{
    public class DonationServiceTests
    {
        DonationService service;

        [SetUp]
        public void Setup()
        {
            var settings = Options.Create(new AppSettings
            {
                TaxRate = 20,
                MinimumDonation = 2,
                MaximumDonation = 100000
            });

            service = new DonationService(settings);
        }

        [Test]
        public void CalculateGiftAidAmount_CorrectlyCalculatesGiftAid()
        {
            decimal amount = 10m;
            decimal expected = 2.5m;

            var result = service.CalculateGiftAidAmount(amount);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expected, result.Value);
        }

        [Test]
        public void CalculateGiftAidAmount_ReturnsErrorIfAmountExceedsMaximum()
        {
            decimal amount = 123000m;

            var result = service.CalculateGiftAidAmount(amount);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ErrorType.DonationAmountAboveMaximum, result.ErrorType);
        }

        [Test]
        public void CalculateGiftAidAmount_ReturnsErrorIfAmountBelowMinimum()
        {
            decimal amount = 1m;

            var result = service.CalculateGiftAidAmount(amount);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ErrorType.DonationAmountBelowMinimum, result.ErrorType);
        }
    }
}