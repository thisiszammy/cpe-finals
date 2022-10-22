using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.Entities.ViewModels
{
    public class TransactionViewModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string Customer { get; set; }
        public decimal TotalAmountDue { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
