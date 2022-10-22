using CPEFinalProject.Entities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.Entities
{
    public class ProductProfile : BaseEntity
    {
        private List<ProductStock> productStocks;
        private List<UnlistedProductStock> unlistedProductStocks;

        string productName;

        [UpdateEntity]
        public string ProductName
        {
            get { return productName; }
            set { productName = value; }
        }

        decimal price;

        [ConsoleTableColumn("Price", 2)]
        [UpdateEntity]
        public decimal Price
        {
            get { return price; }
            set { price = value; }
        }

        [ConsoleTableColumn("Available Stock", 3)]
        public int QtyInStock
        {
            get { return productStocks.Sum(x => x.Quantity); }
        }

        [ConsoleTableColumn("Removed Stock", 4)]
        public int QtyOutStock
        {
            get { return unlistedProductStocks.Sum(x => x.Quantity); }
        }

        string sku;

        [UpdateEntity]
        public string SKU
        {
            get { return sku; }
            set { sku = value; }
        }

        [ConsoleTableColumn("Product", 1)]
        public string ProductIdentifier
        {
            get { return $"({sku}) {productName}"; }
        }

        [UpdateEntity]
        public List<ProductStock> ProductStocks { get => productStocks; set => productStocks = value; }
        [UpdateEntity]
        public List<UnlistedProductStock> UnlistedProductStock { get => unlistedProductStocks; set => unlistedProductStocks = value; }

        public ProductProfile()
        {

        }

        public ProductProfile(string productName, decimal price, string sku)
        {
            this.productName = productName;
            this.price = price;
            this.sku = sku;

            productStocks = new List<ProductStock>();
            unlistedProductStocks = new List<UnlistedProductStock>();
        }

        public void AddStock(int quantity, DateTime mfgDate, DateTime expDate, int receivedBy)
        {
            int batch = (ProductStocks.Count == 0) ? 1 : ProductStocks.Max(x => x.Batch) + 1;
            ProductStocks.Add(new ProductStock(quantity, mfgDate, expDate, batch, Uid, receivedBy));
        }

        public void ReturnStock(int quantity, int batch)
        {
            var stock = ProductStocks.Where(x => x.Batch == batch).FirstOrDefault();
            stock.Quantity += quantity;
        }

        public void RemoveStock(int quantity, int batch)
        {
            var productStock = ProductStocks.Where(x => x.Batch == batch).FirstOrDefault();
            productStock.Quantity -= quantity;
        }

        public void UnlistStock(int quantity, DateTime mfgDate, DateTime expDate, int batch, string remarks, int removedBy, int? receivedBy)
        {
            RemoveStock(quantity, batch);
            unlistedProductStocks.Add(new UnlistedProductStock(quantity, mfgDate, expDate, batch, Uid, remarks, receivedBy, removedBy));
        }



        public override TEntity Clone<TEntity>()
        {
            var profile = new ProductProfile();
            profile.Price = this.price;
            profile.sku = this.sku;
            profile.ProductName = this.ProductName;
            profile.Uid = this.Uid;
            profile.productStocks = new List<ProductStock>(this.ProductStocks);
            profile.unlistedProductStocks = new List<UnlistedProductStock>(this.UnlistedProductStock);
            profile.CreatedBy = this.CreatedBy;
            profile.CreatedOn = this.CreatedOn;
            profile.LastModifiedBy = this.LastModifiedBy;
            profile.LastModifiedOn = this.LastModifiedOn;

            return (TEntity)(object)profile;
        }
    }
}
