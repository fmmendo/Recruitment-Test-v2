using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JG.FinTechTest.Model;
using JG.FinTechTest.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JG.FinTechTest.Controllers
{
    [Route("api/giftaid")]
    [ApiController]
    public class GiftAidController : ControllerBase
    {
        private IDonationService _service;

        public GiftAidController(IDonationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Calculates GiftAid amount.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     Get api/giftaid?amount=10.00
        ///
        /// </remarks>
        /// <param name="amount"></param>
        /// <returns>A GiftAidResponse with Donation amount and GiftAid amount.</returns>
        /// <response code="200">Returns GiftAid calculation.</response>
        /// <response code="400">If the 'amount' is out of bounds.</response>   
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CalculateGiftAid([FromQuery][Required()] decimal amount)
        {
            var giftAid = _service.CalculateGiftAidAmount(amount);

            if (giftAid.IsFailure)
                return BadRequest(giftAid.Error);

            var response = new GiftAidResponse()
            {
                DonationAmount = amount,
                GiftAidAmount = giftAid.Value
            };

            return Ok(response);
        }
    }
}
