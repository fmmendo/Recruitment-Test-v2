using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JG.FinTechTest.Model
{
    public class GiftAidDeclaration
    {
        public int Id { get; set; }
        //public Guid DonationReference { get; set; }
        public string Name { get; set; }
        public string PostCode { get; set; }
        public decimal Amount { get; set; }
    }
}
