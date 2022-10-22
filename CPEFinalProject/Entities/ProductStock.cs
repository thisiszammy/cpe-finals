using CPEFinalProject.Entities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.Entities
{
    public class ProductStock
    {
        int productId;
        public int ProductId
        {
            get { return productId; }
            set { productId = value; }
        }

        int quantity;
        [ConsoleTableColumn("Quantity", 2)]
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        int batch;
        [ConsoleTableColumn("BN", 0)]
        public int Batch
        {
            get { return batch; }
            set { batch = value; }
        }

        DateTime mfgDate;
        [ConsoleTableColumn("Manufacturing Date", 3)]
        public DateTime MfgDate
        {
            get { return mfgDate; }
            set { mfgDate = value; }
        }

        DateTime expDate;
        [ConsoleTableColumn("Expiry Date", 4)]
        public DateTime ExpDate
        {
            get { return expDate; }
            set { expDate = value; }
        }


        private DateTime receivedOn;
        [ConsoleTableColumn("Date Received", 5)]
        public DateTime ReceivedOn { get => receivedOn; set => receivedOn = value; }

        private int? receivedBy;
        public int? ReceivedBy { get => receivedBy; set => receivedBy = value; }

        public ProductStock()
        {

        }

        public ProductStock(int quantity, DateTime mfgDate, DateTime expDate, int batch, int productId, int? receivedBy)
        {
            this.quantity = quantity;
            this.mfgDate = mfgDate;
            this.expDate = expDate;
            this.batch = batch;
            this.productId = productId;
            this.receivedOn = DateTime.Now;
            this.receivedBy = receivedBy;
        }
    }
}
