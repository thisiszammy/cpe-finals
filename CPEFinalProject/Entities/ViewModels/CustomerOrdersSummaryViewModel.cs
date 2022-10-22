using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.Entities.ViewModels
{
    public class CustomerOrdersSummaryViewModel
    {
        public string CustomerName { get; set; }
        public string Username { get; set; }
        public int TotalTransactionCount { get; set; }
        public decimal TotalAmountGarnered { get; set; }
    }
}
