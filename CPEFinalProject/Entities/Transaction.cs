using CPEFinalProject.DAL;
using CPEFinalProject.Entities.Attributes;
using CPEFinalProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.Entities
{
    internal class Transaction : BaseEntity
    {
        int? customerId;
        string transactionId;
        decimal additionalFees;
        string remarks;
        TransactionStatusEnum transactionStatus;
        List<TransactionItem> transactionItems;
        List<TransactionItem> bufferItems;
        List<RefundedTransactionItem> refundedItems;

        public Transaction()
        {
            this.TransactionItems = new List<TransactionItem>();
            this.RefundedItems = new List<RefundedTransactionItem>();
            TransactionStatus = TransactionStatusEnum.PENDING;
            IsDeleted = false;
            BufferItems = new List<TransactionItem>();

        }

        public Transaction(int? customerId,
            string transactionId,
            decimal additionalFees,
            TransactionStatusEnum transactionStatus,
            string remarks)
        {
            this.CustomerId = customerId;
            this.TransactionId = transactionId;
            this.AdditionalFees = additionalFees;
            this.TransactionStatus = transactionStatus;
            this.Remarks = remarks;
            BufferItems = new List<TransactionItem>();
            this.TransactionItems = new List<TransactionItem>();
            this.RefundedItems = new List<RefundedTransactionItem>();
            this.IsDeleted = false;
        }
        [UpdateEntity]
        public int? CustomerId { get => customerId; set => customerId = value; }
        public string TransactionId { get => transactionId; set => transactionId = value; }
        [UpdateEntity]
        public decimal AdditionalFees { get => additionalFees; set => additionalFees = value; }
        [UpdateEntity]
        public TransactionStatusEnum TransactionStatus { get => transactionStatus; set => transactionStatus = value; }
        [UpdateEntity]
        public List<TransactionItem> TransactionItems { get => transactionItems; set => transactionItems = value; }
        [UpdateEntity]
        public List<RefundedTransactionItem> RefundedItems { get => refundedItems; set => refundedItems = value; }
        public decimal TotalAmountDue
        {
            get
            {
                return transactionItems.Sum(x => x.SubTotalPrice) + additionalFees;
            }
        }
        public void AddTransactionItem(int productId, int batch, int quantity, decimal price, DateTime mfgDate, DateTime expDate)
        {
            var item = transactionItems.Where(x => x.ProductId == productId && x.Batch == batch).FirstOrDefault();
            if (item == null) transactionItems.Add(new TransactionItem(quantity, mfgDate, expDate, batch, productId, price));
            else item.Quantity += quantity;

            var bItem = BufferItems.Where(x => x.ProductId == productId && x.Batch == batch).FirstOrDefault();
            if (bItem == null) BufferItems.Add(new TransactionItem(quantity, mfgDate, expDate, batch, productId, price));
            else bItem.Quantity += quantity;

            var product = DBContext<ProductProfile>.GetById(productId);
            var stock = product.ProductStocks.Where(x => x.Batch == batch).FirstOrDefault();
            stock.Quantity -= quantity;

            DBContext<ProductProfile>.Update(product);
        }

        public void RollBackTransactionItems()
        {
            foreach(var item in BufferItems)
            {
                var product = DBContext<ProductProfile>.GetById(item.ProductId);
                var stock = product.ProductStocks.Where(x => x.Batch == item.Batch).FirstOrDefault();
                stock.Quantity += item.Quantity;
                DBContext<ProductProfile>.Update(product);
            }
            BufferItems = new List<TransactionItem>();
        }

        public void RemoveTransactionItem(int productId, int batch, int quantity)
        {
            var item = transactionItems.Where(x => x.ProductId == productId && x.Batch == batch).FirstOrDefault();
            item.Quantity -= quantity;
            if (item.Quantity == 0) transactionItems.Remove(item);

            var product = DBContext<ProductProfile>.GetById(productId);
            var stock = product.ProductStocks.Where(x => x.Batch == batch).FirstOrDefault();
            stock.Quantity += quantity;
            DBContext<ProductProfile>.Update(product);
        }

        public void RefundTransactionItem(int productId, int batch, int quantity, string remarks)
        {
            var item = transactionItems.Where(x => x.ProductId == productId && x.Batch == batch).FirstOrDefault();
            item.Quantity -= quantity;

            var rItem = refundedItems.Where(x => x.ProductId == productId && x.Batch == batch).FirstOrDefault();
            if (rItem != null)
            {
                rItem.Quantity += quantity;
                rItem.Remarks = remarks;
                rItem.RefundedOn = DateTime.Now;
            }
            else
            {
                refundedItems.Add(new RefundedTransactionItem(quantity, item.MfgDate, item.ExpDate, item.Batch, Uid, item.Price, remarks));
            }
        }

        [UpdateEntity]
        public string Remarks { get => remarks; set => remarks = value; }
        public List<TransactionItem> BufferItems { get => bufferItems; set => bufferItems = value; }

        public override TEntity Clone<TEntity>()
        {
            var transaction = new Transaction()
            {
                Uid = this.Uid,
                AdditionalFees = this.additionalFees,
                CreatedBy = this.CreatedBy,
                CreatedOn = this.CreatedOn,
                CustomerId = this.CustomerId,
                DeletedBy = this.DeletedBy,
                DeletedOn = this.DeletedOn,
                IsDeleted = this.IsDeleted,
                LastModifiedBy = this.LastModifiedBy,
                LastModifiedOn = this.LastModifiedOn,
                RefundedItems = new List<RefundedTransactionItem>(this.RefundedItems),
                TransactionId = this.TransactionId,
                TransactionItems = new List<TransactionItem>(this.TransactionItems),
                BufferItems = new List<TransactionItem>(this.BufferItems),
                TransactionStatus = this.TransactionStatus,
                Remarks = this.Remarks
            };

            return (TEntity)(object)transaction;
        }
    }
}
