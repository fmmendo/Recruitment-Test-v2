using JG.FinTechTest.Controllers;
using JG.FinTechTest.Model;
using JG.FinTechTest.Services;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

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
            var okResult = result as OkObjectResult;

            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(expected, ((GiftAidResponse)okResult.Value).GiftAidAmount);

            service.Received().CalculateGiftAidAmount(Arg.Any<decimal>());
        }

        [Test]
        public void GET_CalculateGiftAid_ReturnsBadRequestIfAmountAboveMaximum()
        {
            service.CalculateGiftAidAmount(Arg.Any<decimal>())
                   .Returns(Result.Fail<decimal>(default, "error", ErrorType.DonationAmountAboveMaximum));


            var result = controller.CalculateGiftAid(123000);
            var badResult = result as BadRequestObjectResult;

            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual(400, badResult.StatusCode);

            service.Received().CalculateGiftAidAmount(Arg.Any<decimal>());
        }

        [Test]
        public void GET_CalculateGiftAid_ReturnsBadRequestIfAmountBelowMinimum()
        {
            service.CalculateGiftAidAmount(Arg.Any<decimal>())
                   .Returns(Result.Fail<decimal>(default, "error", ErrorType.DonationAmountBelowMinimum));


            var result = controller.CalculateGiftAid(1);
            var badResult = result as BadRequestObjectResult;

            Assert.IsTrue(result is BadRequestObjectResult);
            Assert.AreEqual(400, badResult.StatusCode);

            service.Received().CalculateGiftAidAmount(Arg.Any<decimal>());
        }
    }
}
