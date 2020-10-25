using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JG.FinTechTest.Model
{
    public class DeclarationRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string PostCode { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }
}
