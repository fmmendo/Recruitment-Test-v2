using JG.FinTechTest.Model;
using JG.FinTechTest.Repository;
using JG.FinTechTest.Services;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JG.FinTechTest.Tests.Services
{
    public class DonationServiceTests
    {
        DonationService service;
        IRepository<GiftAidDeclaration> repository = Substitute.For<IRepository<GiftAidDeclaration>>();

        [SetUp]
        public void Setup()
        {
            var settings = Options.Create(new AppSettings
            {
                TaxRate = 20,
                MinimumDonation = 2,
                MaximumDonation = 100000
            });

            service = new DonationService(settings, repository);
        }

        [Test]
        [TestCase(10, 2.5, true, ErrorType.None)]
        [TestCase(123000, 0, false, ErrorType.DonationAmountAboveMaximum)]
        [TestCase(1, 0, false, ErrorType.DonationAmountBelowMinimum)]
        public void CalculateGiftAidAmount_CorrectlyCalculatesGiftAid(decimal amount, decimal expected, bool isSuccess, ErrorType errorType)
        {
            var result = service.CalculateGiftAidAmount(amount);

            Assert.AreEqual(isSuccess, result.IsSuccess);
            if (isSuccess)
                Assert.AreEqual(expected, result.Value);
            else
                Assert.AreEqual(errorType, result.ErrorType);
        }

        [Test]
        [TestCase("bob", "e15 4qa", 12, 1)]
        public async Task Store_WritesToDatabaseAndReturnsId(string name, string postcode, decimal amount, int id)
        {
            repository.InsertRecord(Arg.Any<GiftAidDeclaration>())
                      .Returns(Result.Ok(id));

            var result = await service.StoreGiftAidDeclaration(name, postcode, amount);

            Assert.IsTrue(result.IsSuccess);

            Assert.AreEqual(id, result.Value);
            repository.Received().InsertRecord(Arg.Any<GiftAidDeclaration>());
        }

        [Test]
        [TestCase("bob", "e15 4qa", 12, 1)]
        public async Task Store_ReturnsErrorIfFailedToWriteToDatabase(string name, string postcode, decimal amount, int id)
        {
            repository.InsertRecord(Arg.Any<GiftAidDeclaration>())
                      .Returns(Result.Fail<int>(default(int),"error", ErrorType.FailedToInsertToDatabase));

            var result = await service.StoreGiftAidDeclaration(name, postcode, amount);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ErrorType.FailedToInsertToDatabase, result.ErrorType);
            repository.Received().InsertRecord(Arg.Any<GiftAidDeclaration>());
        }

        [Test]
        [TestCase("", "sw8", 12,   ErrorType.InvalidParameter)]
        [TestCase("bob", "", 12,   ErrorType.InvalidParameter)]
        [TestCase("bob", "abcd", 12,   ErrorType.InvalidPostCode)]
        [TestCase("bob", "ab2aaa 2cd", 12,   ErrorType.InvalidPostCode)]
        public async Task Store_FailsWithInvalidParameters(string name, string postcode, decimal amount, ErrorType errorType)
        {
            var result = await service.StoreGiftAidDeclaration(name, postcode, amount);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(errorType, result.ErrorType);
            repository.DidNotReceive().InsertRecord(Arg.Any<GiftAidDeclaration>());
        }
    }
}