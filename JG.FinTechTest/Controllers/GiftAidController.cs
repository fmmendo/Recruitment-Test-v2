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
    [Produces("application/json")] 
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

        /// <summary>
        /// Creates GiftAid declaration.
        /// </summary>
        /// <param name="request">A DeclarationRequest with Name, PostCode and Donation Amount</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/giftaid/declaration
        ///     {
        ///         "name": "Bob",
        ///         "postcode": "SE1 7ND",
        ///         "amount": 10.00
        ///     }
        ///
        /// </remarks>
        /// <returns>A DeclarationResponse with and Id and GiftAid amount.</returns>
        /// <response code="201">Returns DeclarationResponse.</response>
        /// <response code="400">If the 'amount' is out of bounds or failed to create declaration.</response>   
        [HttpPost]
        [Route("declaration")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DeclarationResponse>> MakeGiftAidDeclaration(DeclarationRequest request)
        {
            var giftAid = _service.CalculateGiftAidAmount(request.Amount);

            if (giftAid.IsFailure)
                return BadRequest(giftAid.Error);

            var result = await _service.StoreGiftAidDeclaration(request.Name, request.PostCode, request.Amount);

            if (result.IsFailure)
                return BadRequest(result.Error);

            var response = new DeclarationResponse
            {
                GiftAidAmount = giftAid.Value,
                Id = result.Value
            };

            return Created(result.Value.ToString(), response);
        }
    }
}
