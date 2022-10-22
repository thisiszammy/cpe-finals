using CPEFinalProject.Entities;
using CPEFinalProject.Entities.Enums;
using CPEFinalProject.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.DAL
{
    public static class Database
    {
        public static List<object> Tables { get; private set; }

        public static void Init(Type[] types)
        {
            Tables = new List<object>();
            foreach (var type in types)
            {
                Tables.Add(GenerateDynamicList(type));
            }
            MockData();
        }

        public static List<T> GetTable<T>()
        {
            var temp = Tables.Where(x => typeof(List<T>).IsAssignableFrom(x.GetType())).First();
            return (List<T>)temp;
        }

        private static object GenerateDynamicList(Type type)
        {
            Type temp = typeof(List<>).MakeGenericType(new Type[] { type });
            return Activator.CreateInstance(temp);
        }

        private static void MockData()
        {
            // Mock Admins
            DBContext<ApplicationUser>.Add(new AdministrativeUser("Michael Jay", "Zamoras", $"mjzamoras", "123"));
            DBContext<ApplicationUser>.Add(new AdministrativeUser("Alexander", "Ghemlit", $"alexGhem", "123"));
            DBContext<ApplicationUser>.Add(new AdministrativeUser("Lucille", "Vinoe", $"luvinoe", "123"));
            DBContext<ApplicationUser>.Add(new AdministrativeUser("Matthias", "Tallow", $"matt", "123"));

            Customer[] customer = new Customer[]
            {
                new Customer($"Arthas", $"Menethil", $"lichKing", "123", "Lordaeron"),
                new Customer($"Sylvanas", $"Windrunner", $"sylvie", "123", "Silvermoon"),
                new Customer($"Thrall", $"Wolfblud", $"warchief", "123", "Orgrimmar")
            };

            // Mock Customers
            DBContext<ApplicationUser>.Add(customer[0]);
            DBContext<ApplicationUser>.Add(customer[1]);
            DBContext<ApplicationUser>.Add(customer[2]);

            // Mock Products
            ProductProfile[] product = new ProductProfile[]
            {
                new ProductProfile($"Black King's Bar", 20.00m, RandomStringGeneratorService.GenerateRandomString(5)),
                new ProductProfile($"Orb of Perseverance", 40.00m, RandomStringGeneratorService.GenerateRandomString(5)),
                new ProductProfile($"Eye of Sargeras", 530.00m, RandomStringGeneratorService.GenerateRandomString(5)),
                new ProductProfile($"Philosopher's Stone", 80.00m, RandomStringGeneratorService.GenerateRandomString(5)),
                new ProductProfile($"Skull of Gul'dan", 110.00m, RandomStringGeneratorService.GenerateRandomString(5))
            };

            DBContext<ProductProfile>.Add(product[0]);
            DBContext<ProductProfile>.Add(product[1]);
            DBContext<ProductProfile>.Add(product[2]);
            DBContext<ProductProfile>.Add(product[3]);
            DBContext<ProductProfile>.Add(product[4]);


            // Mock Stocks
            product[0].AddStock(100, DateTime.Now, DateTime.Now.AddYears(1), 1);
            product[1].AddStock(80, DateTime.Now, DateTime.Now.AddYears(1), 1);
            product[2].AddStock(60, DateTime.Now, DateTime.Now.AddYears(1), 1);
            product[3].AddStock(120, DateTime.Now, DateTime.Now.AddYears(1), 1);
            product[4].AddStock(130, DateTime.Now, DateTime.Now.AddYears(1), 1);


            Transaction transaction = new Transaction(customer[0].Uid, RandomStringGeneratorService.GenerateRandomString(8), 100, TransactionStatusEnum.PAID, "P100 FOR DELIVER");
            transaction.AddTransactionItem(product[0].Uid, 1, 4, product[0].Price, product[0].ProductStocks.FirstOrDefault().MfgDate, product[0].ProductStocks.FirstOrDefault().ExpDate);
            transaction.AddTransactionItem(product[1].Uid, 1, 10, product[0].Price, product[0].ProductStocks.FirstOrDefault().MfgDate, product[0].ProductStocks.FirstOrDefault().ExpDate);
            transaction.AddTransactionItem(product[3].Uid, 1, 2, product[0].Price, product[0].ProductStocks.FirstOrDefault().MfgDate, product[0].ProductStocks.FirstOrDefault().ExpDate);

            DBContext<Transaction>.Add(transaction);

        }
    }
}
