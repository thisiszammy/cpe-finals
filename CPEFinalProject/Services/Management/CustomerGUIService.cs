using CPEFinalProject.DAL;
using CPEFinalProject.Entities;
using CPEFinalProject.Entities.Enums;
using CPEFinalProject.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.Services.Management
{
    public class CustomerGUIService : ICustomerGUIService
    {
        readonly INotificationService notificationService;
        int? selectedTransaction = null;
        public CustomerGUIService(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        public void EditProfile()
        {
            Console.Clear();

            var selectedAdminEntity = DBContext<ApplicationUser>.GetById(Program.currentUser.Uid);

            ApplicationUser? createdBy = DBContext<ApplicationUser>.GetById(selectedAdminEntity.CreatedBy);
            ApplicationUser? lastModifiedBy = (selectedAdminEntity.LastModifiedBy == null) ? null : DBContext<ApplicationUser>.GetById(selectedAdminEntity.LastModifiedBy.Value);

            int pageStartX = Console.WindowWidth / 2 - 40;
            int pageEndX = Console.WindowWidth / 2 + 40;

            ZConsole.DrawBox(pageStartX, pageEndX, 2, 25);
            ZConsole.Write("ZINVSYS Customer Interface", pageStartX, 1, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.Write("[EDIT PROFILE]", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.DrawClosedPipe(4, pageStartX, pageEndX);

            ZConsole.Write("UID: " + selectedAdminEntity.Uid, pageStartX + 2, 5, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("First Name: " + selectedAdminEntity.FirstName, pageStartX + 2, 6, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Last Name: " + selectedAdminEntity.LastName, pageStartX + 2, 7, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Username: " + selectedAdminEntity.Username, pageStartX + 2, 8, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Created On: " + selectedAdminEntity.CreatedOn.ToString("MM/dd/yyyy hh:mm tt"), pageStartX + 2, 9, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Created By: " + ((createdBy == null) ? "[unable to retrieve user]" : createdBy.Username), pageStartX + 2, 10, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Last Modified On: " + ((selectedAdminEntity.LastModifiedOn == null) ? "" : selectedAdminEntity.LastModifiedOn.ToString()), pageStartX + 2, 11, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Last Modified By: " + ((lastModifiedBy == null) ? "[unable to retrieve user]" : lastModifiedBy.Username), pageStartX + 2, 12, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);

            ZConsole.DrawClosedPipe(13, pageStartX, pageEndX);
            ZConsole.Write("Enter New Details", pageStartX, 14, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.DrawClosedPipe(15, pageStartX, pageEndX);


            string firstName, lastName, username, password = string.Empty, confirmPassword = string.Empty;

            ZConsole.Write("Enter First Name: ", pageStartX + 2, 16, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            firstName = Console.ReadLine();

            if (string.IsNullOrEmpty(firstName)) throw new Exception("Please Enter First Name!");

            ZConsole.Write("Enter Last Name: ", pageStartX + 2, 17, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            lastName = Console.ReadLine();

            if (string.IsNullOrEmpty(lastName)) throw new Exception("Please Enter Last Name!");

            ZConsole.Write("Enter Username: ", pageStartX + 2, 18, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            username = Console.ReadLine();

            if (string.IsNullOrEmpty(username)) throw new Exception("Please Enter Username!");

            ZConsole.Write("Enter Password: ", pageStartX + 2, 19, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);

            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && password.Length > 0)
                {
                    Console.Write("\b \b");
                    password = password[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    password += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);

            if (string.IsNullOrEmpty(password)) throw new Exception("Please Enter Password!");

            ZConsole.Write("Re-Enter Password: ", pageStartX + 2, 20, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && confirmPassword.Length > 0)
                {
                    Console.Write("\b \b");
                    confirmPassword = confirmPassword[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    confirmPassword += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);

            if (string.IsNullOrEmpty(confirmPassword)) throw new Exception("Please Re-Enter Password!");

            if (password != confirmPassword) throw new Exception("Passwords Not Matching!");

            var existingUser = DBContext<ApplicationUser>.GetByCustomQuery(x => x.Username == username && x.Uid != Program.currentUser.Uid).FirstOrDefault();
            if (existingUser != null) throw new Exception("Username Is Already Used!");

            selectedAdminEntity.FirstName = firstName;
            selectedAdminEntity.LastName = lastName;
            selectedAdminEntity.Username = username;
            selectedAdminEntity.LastModifiedBy = Program.currentUser.Uid;
            selectedAdminEntity.Password = password;


            DBContext<ApplicationUser>.Update(selectedAdminEntity);

            notificationService.DrawMessageBox("Successfully Updated Admin!");

        }

        public void ListOrderHistory()
        {
            int pageStartX = Console.WindowWidth / 2 - 80;
            int pageEndX = Console.WindowWidth / 2 + 80;

            string additionalTitle = string.Empty;

            ConsoleKeyInfo? command = null;

            int page = 0, rowPerPage = 18, scrollerIndex = 0, pageCount = 0;
            int maxScrollerIndex = 0, lastPageMaxScrollerIndex = 0;
            string? searchQuery = null;

            int columnWidth = pageEndX - pageStartX + 4;

            scrollerIndex = 0;

            List<Entities.Transaction> allrecords = DBContext<Entities.Transaction>.GetByCustomQuery(x=>x.CustomerId == Program.currentUser.Uid);
            List<Entities.Transaction> records = new List<Entities.Transaction>(allrecords);

            int mode = 1;
            do
            {

                if (command != null)
                {
                    switch (command.Value.Key)
                    {
                        case ConsoleKey.Escape:
                            return;
                        case ConsoleKey.A:
                        case ConsoleKey.LeftArrow:
                            scrollerIndex = 0;
                            page--;
                            if (page < 0) page = 0;
                            break;
                        case ConsoleKey.D:
                        case ConsoleKey.RightArrow:
                            scrollerIndex = 0;
                            page++;
                            if (page == pageCount) page = 0;
                            break;
                        case ConsoleKey.W:
                        case ConsoleKey.UpArrow:
                            scrollerIndex--;
                            if (scrollerIndex < 0)
                            {
                                scrollerIndex = rowPerPage - 1;
                                page--;
                                if (page < 0)
                                {
                                    page = pageCount - 1;
                                    scrollerIndex = lastPageMaxScrollerIndex;
                                }
                            }
                            break;
                        case ConsoleKey.S:
                        case ConsoleKey.DownArrow:
                            scrollerIndex++;
                            if (scrollerIndex == maxScrollerIndex)
                            {
                                scrollerIndex = 0;
                                page++;
                                if (page == pageCount) page = 0;
                            }
                            break;
                        case ConsoleKey.F1:
                            scrollerIndex = 0;
                            page = 0;
                            searchQuery = PromptSearchQuery();
                            break;
                        case ConsoleKey.F2:
                            mode++;
                            if (mode > 2) mode = 0;
                            break;
                        case ConsoleKey.Enter:
                            selectedTransaction = records[scrollerIndex].Uid;
                            ViewOrderDetails();
                            break;
                    }
                }

                Console.Clear();


                if (string.IsNullOrEmpty(searchQuery))
                {
                    records = allrecords.Where(x => x.TransactionStatus == (TransactionStatusEnum)mode).ToList();
                }
                else
                {
                    records = allrecords.Where(x => x.TransactionStatus == (TransactionStatusEnum)mode).Where(x => x.TransactionId.ToLower().Contains(searchQuery.ToLower()) ||
                    Program.currentUser.Username.ToLower().Contains(searchQuery.ToLower())).ToList();

                    ZConsole.Write($"Searching: \"{searchQuery}\"", pageStartX + 2, 3, (pageEndX / 2) - 5, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
                }

                records = records.OrderByDescending(x => x.Uid).ToList();

                decimal div = (decimal)records.Count() / rowPerPage;
                pageCount = int.Parse(Math.Ceiling(div).ToString());
                pageCount = (pageCount == 0) ? 1 : pageCount;

                lastPageMaxScrollerIndex = (records.Count() % rowPerPage) - 1;
                records = records.Skip(page * rowPerPage).Take(rowPerPage).ToList();

                maxScrollerIndex = records.Count;

                additionalTitle = $"Showing \"{((TransactionStatusEnum)mode).ToString().ToUpper()}\" Transactions";

                ZConsole.DrawBox(pageStartX, pageEndX, 2, 27);
                ZConsole.Write("ZINVSYS Customer Interface", pageStartX, 1, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write($"[TRANSACTIONS HISTORY] {additionalTitle}", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write($"(Page {page + 1}/{pageCount}) ", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.RIGHT);

                int recStartLocY = 7;

                ZConsole.DrawClosedPipe(4, pageStartX, pageEndX);

                int remainingSpaces = pageEndX - (pageStartX + 11);
                int colSize = remainingSpaces / 5;

                ZConsole.Write("#", pageStartX + 1, 5, pageStartX + 4, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("ID", pageStartX + 6, 5, pageStartX + 11, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("Transaction ID", pageStartX + 11, 5, (pageStartX + 11) + (colSize * 1), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("Customer", (pageStartX + 11) + (colSize * 1), 5, (pageStartX + 11) + (colSize * 2), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("Total Amount", (pageStartX + 11) + (colSize * 2), 5, (pageStartX + 11) + (colSize * 3), null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                ZConsole.Write("Status", (pageStartX + 11) + (colSize * 3), 5, (pageStartX + 11) + (colSize * 4), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("Created On", (pageStartX + 11) + (colSize * 4), 5, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);

                ZConsole.DrawClosedPipe(6, pageStartX, pageEndX);
                ZConsole.DrawClosedPipe(25, pageStartX, pageEndX);

                ZConsole.Write("ESC - Back | W - Up | S - Down | A - Prv Pg. | D - Nxt Pg. | F1 - Search | F2 - Switch Mode | Enter - Select", pageStartX + 1, 26, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);



                for (int i = 0; i < maxScrollerIndex; i++)
                {

                    if (scrollerIndex == i)
                    {
                        ZConsole.Write(">", pageStartX - 1, recStartLocY + i, null, null);
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    Console.SetCursorPosition(pageStartX + 1, recStartLocY + i);
                    for (int j = pageStartX + 1; j < pageEndX - 1; j++) Console.Write(" ");

                    var _entity = records[i];

                    var customer = DBContext<ApplicationUser>.GetById(_entity.CustomerId.Value);

                    ZConsole.Write((i + 1).ToString(), pageStartX + 1, recStartLocY + i, pageStartX + 4, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    ZConsole.Write(_entity.Uid.ToString(), pageStartX + 6, recStartLocY + i, pageStartX + 11, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    ZConsole.Write(_entity.TransactionId, pageStartX + 11, recStartLocY + i, (pageStartX + 11) + (colSize * 1), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    ZConsole.Write(customer.Username, (pageStartX + 11) + (colSize * 1), recStartLocY + i, (pageStartX + 11) + (colSize * 2), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    ZConsole.Write("PHP " + _entity.TotalAmountDue.ToString("N2"), (pageStartX + 11) + (colSize * 2), recStartLocY + i, (pageStartX + 11) + (colSize * 3), null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                    ZConsole.Write(_entity.TransactionStatus.ToString(), (pageStartX + 11) + (colSize * 3), recStartLocY + i, (pageStartX + 11) + (colSize * 4), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    ZConsole.Write(_entity.CreatedOn.ToString("MM/dd/yyyy hh:mm tt"), (pageStartX + 11) + (colSize * 4), recStartLocY + i, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);

                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                command = Console.ReadKey();
            }
            while (command.Value.Key != ConsoleKey.Escape);
        }

        private string PromptSearchQuery()
        {
            int pageStartX = Console.WindowWidth / 2 - 50;
            int pageEndX = Console.WindowWidth / 2 + 50;

            ZConsole.DrawBox(pageStartX, pageEndX, 28, 30);
            ZConsole.Write("Search Keyword: ", pageStartX + 2, 29, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);

            string query = Console.ReadLine();

            return query;
        }

        public void ViewOrderDetails()
        {
            if (selectedTransaction == null) throw new Exception("Please Select Record To View!");
            Entities.Transaction transaction = DBContext<Entities.Transaction>.GetById(selectedTransaction.Value);

            int pageStartX = Console.WindowWidth / 2 - 80;
            int pageEndX = Console.WindowWidth / 2 + 80;

            int page = 0, rowPerPage = 16, scrollerIndex = 0, pageCount = 0;
            int maxScrollerIndex = 0, lastPageMaxScrollerIndex = 0;
            string? searchQuery = null;

            List<TransactionItem> records = new List<TransactionItem>();
            ConsoleKeyInfo command = new ConsoleKeyInfo('A', ConsoleKey.A, false, false, false);
            while (true)
            {

                Customer customer = null;

                if (transaction.CustomerId != null)
                {
                    customer = (Customer)DBContext<ApplicationUser>.GetById(transaction.CustomerId.Value);
                }

                switch (command.Key)
                {
                    case ConsoleKey.Escape:
                        return;
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        scrollerIndex--;
                        if (scrollerIndex < 0)
                        {
                            scrollerIndex = rowPerPage - 1;
                            page--;
                            if (page < 0)
                            {
                                page = pageCount - 1;
                                scrollerIndex = lastPageMaxScrollerIndex;
                            }
                        }
                        break;
                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        scrollerIndex++;
                        if (scrollerIndex == maxScrollerIndex)
                        {
                            scrollerIndex = 0;
                            page++;
                            if (page == pageCount) page = 0;
                        }
                        break;
                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        scrollerIndex = 0;
                        page--;
                        if (page < 0) page = 0;
                        break;
                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        scrollerIndex = 0;
                        page++;
                        if (page == pageCount) page = 0;
                        break;
                }

                // transaction details 
                Console.Clear();

                ZConsole.DrawBox(pageStartX, pageEndX, 2, 30);
                ZConsole.Write("ZINVSYS Customer Interface", pageStartX, 1, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("[TRANSACTION DETAILS]", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.DrawClosedPipe(4, pageStartX, pageEndX);
                ZConsole.Write("Transaction Details", pageStartX + 2, 6, pageStartX + 40, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.DrawBox(pageStartX + 2, pageStartX + 40, 5, 29);
                ZConsole.DrawClosedPipe(7, pageStartX + 2, pageStartX + 40);

                ZConsole.Write("Transaction ID: " + transaction.TransactionId, pageStartX + 3, 8, pageStartX + 40, null);
                ZConsole.Write("Customer: " + ((customer == null) ? string.Empty : customer.Username), pageStartX + 3, 9, pageStartX + 40, null);
                ZConsole.Write("Additional Fees: PHP " + transaction.AdditionalFees.ToString("N2"), pageStartX + 3, 10, pageStartX + 40, null);

                ZConsole.Write("Status: " + transaction.TransactionStatus.ToString(), pageStartX + 3, 12, pageStartX + 40, null);
                ZConsole.Write("Total Amount: PHP " + transaction.TotalAmountDue.ToString("N2"), pageStartX + 3, 14, pageStartX + 40, null);
                ZConsole.Write("Remarks: ", pageStartX + 3, 16, pageStartX + 40, null);
                ZConsole.Write(transaction.Remarks ?? "", pageStartX + 5, 17, pageStartX + 40, 18, flag: ZConsole.ConsoleFormatFlags.TOP_LEFT);

                ZConsole.Write("(ESC) - [Back to Menu]", pageStartX + 3, 20, pageStartX + 40, null);

                // cart

                if (string.IsNullOrEmpty(searchQuery))
                {
                    records = transaction.TransactionItems;
                }
                else
                {
                    records = transaction.TransactionItems.Where(x => DBContext<ProductProfile>.GetById(x.ProductId).ProductName.Contains(searchQuery)).ToList();
                    ZConsole.Write($"Searching: \"{searchQuery}\"", pageStartX + 42, 6, pageEndX - 1, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
                }

                decimal div = (decimal)records.Count() / rowPerPage;
                pageCount = int.Parse(Math.Ceiling(div).ToString());
                pageCount = (pageCount == 0) ? 1 : pageCount;

                lastPageMaxScrollerIndex = (records.Count() % rowPerPage) - 1;
                records = records.Skip(page * rowPerPage).Take(rowPerPage).ToList();

                maxScrollerIndex = records.Count;


                ZConsole.DrawBox(pageStartX + 42, pageEndX - 1, 5, 29);
                ZConsole.Write("Transaction Cart", pageStartX + 42, 6, pageEndX - 1, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write($"(Page {page + 1}/{pageCount}) ", pageStartX, 6, pageEndX - 1, null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                ZConsole.DrawClosedPipe(7, pageStartX + 42, pageEndX - 1);

                int recStartLocY = 10;

                int remainingSpaces = pageEndX - (pageStartX + 52);
                int colSize = remainingSpaces / 6;

                ZConsole.Write("#", pageStartX + 42, 8, pageStartX + 46, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("BN", pageStartX + 48, 8, pageStartX + 53, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("Product Identifier", pageStartX + 53, 8, (pageStartX + 53) + (colSize * 1), null, flag: ZConsole.ConsoleFormatFlags.LEFT);
                ZConsole.Write("Manufacturing Date", (pageStartX + 53) + (colSize * 1), 8, (pageStartX + 53) + (colSize * 2), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("Expiry Date", (pageStartX + 53) + (colSize * 2), 8, (pageStartX + 53) + (colSize * 3), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("Quantity", (pageStartX + 53) + (colSize * 3), 8, (pageStartX + 53) + (colSize * 4), null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                ZConsole.Write("Price", (pageStartX + 53) + (colSize * 4), 8, (pageStartX + 53) + (colSize * 5), null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                ZConsole.Write("Total", (pageStartX + 53) + (colSize * 5), 8, pageEndX - 2, null, flag: ZConsole.ConsoleFormatFlags.RIGHT);


                ZConsole.DrawClosedPipe(9, pageStartX + 42, pageEndX - 1);
                ZConsole.DrawClosedPipe(27, pageStartX + 42, pageEndX - 1);
                ZConsole.Write("W - Up | S - Down | A - Prv Pg. | D - Nxt Pg. | [DEL] - Remove Cart Item", pageStartX + 44, 28, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);


                for (int i = 0; i < maxScrollerIndex; i++)
                {

                    if (scrollerIndex == i)
                    {
                        ZConsole.Write(">", pageStartX + 41, recStartLocY + i, null, null);
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    Console.SetCursorPosition(pageStartX + 43, recStartLocY + i);
                    for (int j = pageStartX + 43; j < pageEndX - 2; j++) Console.Write(" ");

                    var _entity = records[i];

                    var product = DBContext<ProductProfile>.GetById(_entity.ProductId);

                    ZConsole.Write((i + 1).ToString(), pageStartX + 42, recStartLocY + i, pageStartX + 46, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    ZConsole.Write(_entity.Batch.ToString(), pageStartX + 48, recStartLocY + i, pageStartX + 53, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    ZConsole.Write(product.ProductIdentifier, pageStartX + 53, recStartLocY + i, (pageStartX + 53) + (colSize * 1), null, flag: ZConsole.ConsoleFormatFlags.LEFT);
                    ZConsole.Write(_entity.MfgDate.ToString("MM/dd/yyyy"), (pageStartX + 53) + (colSize * 1), recStartLocY + i, (pageStartX + 53) + (colSize * 2), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    ZConsole.Write(_entity.ExpDate.ToString("MM/dd/yyyy"), (pageStartX + 53) + (colSize * 2), recStartLocY + i, (pageStartX + 53) + (colSize * 3), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    ZConsole.Write(_entity.Quantity.ToString(), (pageStartX + 53) + (colSize * 3), recStartLocY + i, (pageStartX + 53) + (colSize * 4), null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                    ZConsole.Write("PHP " + _entity.Price.ToString("N2"), (pageStartX + 53) + (colSize * 4), recStartLocY + i, (pageStartX + 53) + (colSize * 5), null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                    ZConsole.Write("PHP " + (_entity.Quantity * _entity.Price).ToString("N2"), (pageStartX + 53) + (colSize * 5), recStartLocY + i, pageEndX - 2, null, flag: ZConsole.ConsoleFormatFlags.RIGHT);

                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                command = Console.ReadKey();
            }
            selectedTransaction = null;
        }

        
    }
}
