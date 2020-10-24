using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JG.FinTechTest.Model;
using JG.FinTechTest.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JG.FinTechTest.Controllers
{
    [Route("api/giftaid")]
    [ApiController]
    public class GiftAidController : ControllerBase
    {
        private IDonationService _service;
        private AppSettings _settings;

        public GiftAidController(IDonationService service, IOptions<AppSettings> settings)
        {
            _service = service;
            _settings = settings.Value;
        }

        //[HttpGet]
        //public IActionResult Test()
        //{
        //    return Ok("Hello World");
        //}

        [HttpGet]
        //[Route("{amount}")]
        public IActionResult CalculateGiftAid([FromQuery][Required()] decimal amount)
        {
            var giftAid = _service.CalculateGiftAidAmount(amount);

            var response = new GiftAidResponse()
            {
                DonationAmount = amount,
                GiftAidAmount = giftAid
            };

            return Ok(response);
        }
    }
}
