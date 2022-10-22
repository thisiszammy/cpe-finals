using CPEFinalProject.Entities.Attributes;
using CPEFinalProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace CPEFinalProject.Entities
{
    public class ApplicationUser : BaseEntity
    {
        string firstName;
        string lastName;
        string username;
        string password;
        string salt;
        ApplicationUserTypeEnum userType;

        [UpdateEntity]
        public string Username
        {
            get { return username; }
            set { username = value ?? string.Empty; }
        }

        [UpdateEntity]
        public string Password
        {
            get { return password; }
            set { password = value ?? string.Empty; }
        }


        [UpdateEntity]
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        [UpdateEntity]
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public string Salt
        {
            get { return salt; }
            set { salt = value; }
        }

        public string CompleteName
        {
            get => $"{FirstName} {LastName}";
        }
        public ApplicationUserTypeEnum UserType { get => userType; set => userType = value; }

        public ApplicationUser()
        {

        }
        public ApplicationUser(string firstName, string lastName, string? username, string? password, ApplicationUserTypeEnum userType)
        {
            FirstName = firstName;
            LastName = lastName;
            CreatedOn = DateTime.Now;
            CreatedBy = (Program.currentUser == null) ? -1 : Program.currentUser.Uid;
            IsDeleted = false;
            this.username = username ?? string.Empty;
            this.password = password ?? string.Empty;
            this.UserType = userType;

            Salt = "_" + DateTime.Now.ToString("MMddyyyyHHmmss");

            SHA256 hasher = SHA256.Create();
            var passwordBytes = hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + Salt));
            this.password = Convert.ToBase64String(passwordBytes);
        }


        public override TEntity Clone<TEntity>()
        {
            var administrativeUser = new AdministrativeUser()
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
                UserType = this.UserType
            };

            return (TEntity)(object)administrativeUser;
        }

        public bool AuthenticateUser(string password)
        {
            SHA256 hasher = SHA256.Create();
            var passwordBytes = hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + Salt));

            return this.password == Convert.ToBase64String(passwordBytes);
        }

        public void UpdatePassword(string password)
        {
            SHA256 hasher = SHA256.Create();
            var passwordBytes = hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + Salt));
            this.password = Convert.ToBase64String(passwordBytes);
        }
    }
}
