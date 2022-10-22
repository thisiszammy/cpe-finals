using CPEFinalProject.Entities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.Entities
{
    public class UnlistedProductStock : ProductStock
    {
        string remarks;

        [ConsoleTableColumn("Remarks", 1)]
        public string Remarks
        {
            get { return remarks; }
            set { remarks = value; }
        }


        DateTime unlistedOn;

        [ConsoleTableColumn("Date Received", 6)]
        public DateTime UnlistedOn { get => unlistedOn; set => unlistedOn = value; }

        int? removedBy;
        public int? RemovedBy { get => removedBy; set => removedBy = value; }

        public UnlistedProductStock(ProductStock productStock, string remarks, int removedBy) :
            base(productStock.Quantity, productStock.MfgDate, productStock.ExpDate, productStock.Batch, productStock.ProductId, productStock.ReceivedBy)
        {
            this.remarks = remarks;
            this.removedBy = removedBy;
            this.unlistedOn = DateTime.Now;
        }

        public UnlistedProductStock(int quantity, DateTime mfgDate, DateTime expDate, int batch, int productId, string remarks, int? receivedBy, int removedBy)
            : base(quantity, mfgDate, expDate, batch, productId, receivedBy)
        {
            this.remarks = remarks;
            this.UnlistedOn = DateTime.Now;
            this.removedBy = removedBy;
        }

    }
}
