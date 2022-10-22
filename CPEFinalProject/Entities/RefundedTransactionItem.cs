using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.Entities
{
    internal class RefundedTransactionItem : TransactionItem
    {
        string remarks;
        DateTime refundedOn;

        public RefundedTransactionItem(int quantity, DateTime mfgDate, DateTime expDate, int batch, int productId, decimal price, string remarks)
            : base(quantity, mfgDate, expDate, batch, productId, price)
        {
            this.Remarks = remarks;
            this.RefundedOn = DateTime.Now;
        }

        public string Remarks { get => remarks; set => remarks = value; }
        public DateTime RefundedOn { get => refundedOn; set => refundedOn = value; }
    }
}
