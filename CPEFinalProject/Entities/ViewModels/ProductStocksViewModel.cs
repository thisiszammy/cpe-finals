using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.Entities.ViewModels
{
    public class ProductStocksViewModel
    {
        public int ProductId { get; set; }
        public int Batch { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public DateTime MfgDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime ReceivedOn { get; set; }
        public DateTime? RemovedOn { get; set; }
    }
}
