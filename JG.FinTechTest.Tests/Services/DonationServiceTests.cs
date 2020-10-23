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
        IOptions<AppSettings> settings;

        [SetUp]
        public void Setup()
        {
            var settings = Options.Create(new AppSettings { TaxRate = 20 });

            service = new DonationService(settings);
        }

        [Test]
        public void CalculateGiftAidAmount_CorrectlyCalculatesGiftAid()
        {
            decimal amount = 10m;
            decimal expected = 2.5m;

            var result = service.CalculateGiftAidAmount(amount);

            Assert.AreEqual(expected, result);
        }
    }
}