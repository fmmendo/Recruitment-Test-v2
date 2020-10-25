using JG.FinTechTest.Controllers;
using JG.FinTechTest.Model;
using JG.FinTechTest.Services;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JG.FinTechTest.Tests.Controllers
{
    class GiftAidControllerTests
    {
        IDonationService service = Substitute.For<IDonationService>();
        GiftAidController controller;

        [SetUp]
        public void Setup()
        {
            controller = new GiftAidController(service);
        }

        [Test]
        public void GET_CalculateGiftAid_ReturnsCalculatedAmount()
        {
            var expected = 2.5m;

            service.CalculateGiftAidAmount(Arg.Any<decimal>()).Returns(Result.Ok<decimal>(2.5m));

            var result = controller.CalculateGiftAid(10);
            Assert.IsTrue(result is OkObjectResult);

            var okResult = result as OkObjectResult;
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(expected, ((GiftAidResponse)okResult.Value).GiftAidAmount);

            service.Received().CalculateGiftAidAmount(Arg.Any<decimal>());
        }

        [Test]
        [TestCase(123000, ErrorType.DonationAmountAboveMaximum)]
        [TestCase(1, ErrorType.DonationAmountBelowMinimum)]
        public void GET_CalculateGiftAid_ReturnsBadRequestIfAmountAboveMaximum(decimal amount, ErrorType errorType)
        {
            service.CalculateGiftAidAmount(Arg.Any<decimal>())
                   .Returns(Result.Fail<decimal>(default, "error", errorType));

            var result = controller.CalculateGiftAid(amount);
            Assert.IsTrue(result is BadRequestObjectResult);

            var badResult = result as BadRequestObjectResult;
            Assert.AreEqual(400, badResult.StatusCode);

            service.Received().CalculateGiftAidAmount(Arg.Any<decimal>());
        }

        [Test]
        [TestCase(100, "Bob", "SW1 1WS", 10, 1)]
        public async Task POST_MakeDeclaration_ReturnsDeclarationResponseWithValidRequest(decimal amount, string name, string postcode, decimal giftaid, int id)
        {
            service.CalculateGiftAidAmount(Arg.Any<decimal>())
                   .Returns(Result.Ok<decimal>(giftaid));
            service.StoreGiftAidDeclaration(Arg.Is(name), Arg.Is(postcode), Arg.Is(amount))
                   .Returns(Task.FromResult(Result.Ok<int>(id)));
            var request = new DeclarationRequest
            {
                Amount = amount,
                Name = name,
                PostCode = postcode
            };

            var result = await controller.MakeGiftAidDeclaration(request);

            Assert.IsTrue(result.Result is CreatedResult);
            var created = result.Result as CreatedResult;
            Assert.AreEqual(id, ((DeclarationResponse)created.Value).Id);
            Assert.AreEqual(giftaid, ((DeclarationResponse)created.Value).GiftAidAmount);
            _ = await service.Received().StoreGiftAidDeclaration(Arg.Is(name), Arg.Is(postcode), Arg.Is(amount));
        }

        [Test]
        [TestCase(1, "Bob", "SW1 1WS", ErrorType.DonationAmountBelowMinimum, "some error")]
        [TestCase(123000, "Bob", "SW1 1WS", ErrorType.DonationAmountAboveMaximum, "some error")]
        public async Task POST_MakeDeclaration_ReturnsBadRequestWithInvalidRequest(decimal amount, string name, string postcode, ErrorType errorType, string error)
        {
            service.CalculateGiftAidAmount(Arg.Any<decimal>())
                   .Returns(Result.Fail<decimal>(default, error, errorType));
            var request = new DeclarationRequest
            {
                Amount = amount,
                Name = name,
                PostCode = postcode
            };

            var result = await controller.MakeGiftAidDeclaration(request);

            Assert.IsTrue(result.Result is BadRequestObjectResult);
            var badrequest = result.Result as BadRequestObjectResult;
            Assert.AreEqual(error, ((string)badrequest.Value));
        }

        [Test]
        [TestCase(200, "Bob", "SW1 1WS", "some error", ErrorType.FailedToInsertToDatabase)]
        public async Task POST_MakeDeclaration_ReturnsBadRequestWhenUnableToWriteToDatabase(decimal amount, string name, string postcode, string error, ErrorType errorType)
        {
            service.CalculateGiftAidAmount(Arg.Any<decimal>())
                   .Returns(Result.Ok<decimal>(10));
            service.StoreGiftAidDeclaration(Arg.Is(name), Arg.Is(postcode), Arg.Is(amount))
                   .Returns(Result.Fail<int>(0, error, ErrorType.FailedToInsertToDatabase));
            var request = new DeclarationRequest
            {
                Amount = amount,
                Name = name,
                PostCode = postcode
            };

            var result = await controller.MakeGiftAidDeclaration(request);

            Assert.IsTrue(result.Result is BadRequestObjectResult);
            var badrequest = result.Result as BadRequestObjectResult;
            Assert.AreEqual(error, ((string)badrequest.Value));
            _ = await service.Received().StoreGiftAidDeclaration(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<decimal>());
        }
    }
}
