using CPEFinalProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.Entities
{
    public class Customer : ApplicationUser
    {
        private string address;

        public Customer()
        {

        }

        public Customer(string firstName, string lastName, string username, string password , string address)
            : base(firstName, lastName, username, password, ApplicationUserTypeEnum.CUSTOMER)
        {
            this.Address = address;
        }

        public string Address { get => address; set => address = value; }

        public override TEntity Clone<TEntity>()
        {
            var customer = new Customer()
            {
                Uid = this.Uid,
                CreatedBy = this.CreatedBy,
                CreatedOn = this.CreatedOn,
                DeletedBy = this.DeletedBy,
                DeletedOn = this.DeletedOn,
                FirstName = this.FirstName,
                IsDeleted = this.IsDeleted,
                LastModifiedBy = this.LastModifiedBy,
                LastModifiedOn = this.LastModifiedOn,
                LastName = this.LastName,
                Password = this.Password,
                Username = this.Username,
                Address = this.Address
            };

            return (TEntity)(object)customer;
        }

    }
}
