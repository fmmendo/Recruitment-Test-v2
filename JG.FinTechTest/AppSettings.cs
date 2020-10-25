using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JG.FinTechTest
{
    public class AppSettings
    {
        public decimal TaxRate { get; set; }
        public decimal MinimumDonation { get; set; }
        public decimal MaximumDonation { get; set; }
        public string DbLocation { get; set; }
        public string DonationsTable { get; set; }
    }
}
