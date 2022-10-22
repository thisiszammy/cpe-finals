using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.Entities.ViewModels
{
    public class SalesSummaryViewModel
    {
        public string ProductIdentifier { get; set; }
        public string ProductName { get; set; }
        public decimal GrossAmountEarned { get; set; }
        public int TotalQuantitySold { get; set; }
    }
}
