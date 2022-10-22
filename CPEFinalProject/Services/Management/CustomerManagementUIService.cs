using CPEFinalProject.DAL;
using CPEFinalProject.Entities;
using CPEFinalProject.Entities.Enums;
using CPEFinalProject.Entities.ViewModels;
using CPEFinalProject.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.Services.Management
{
    public class CustomerManagementUIService : IManagementGUIService
    {
        readonly INotificationService notificationService;
        readonly Type contextType;
        int? selectedCustomer = null;

        public CustomerManagementUIService()
        {
            notificationService = Program.notificationService;
            contextType = typeof(Customer);
        }

        public void DrawScrollerSelection(string title, ScrollerOption[] options)
        {
            var scroller = new MenuScrollerService(title, options);
            var scrollerSelection = scroller.Run();
            Program.NextPage(scrollerSelection.DestinationStateFlagEnum, scrollerSelection.DestinationType, scrollerSelection.DestinationStateFlagEnum == ApplicationStateFlagsEnum.BACK);
        }


        public void CreateEntity()
        {
            Console.Clear();

            int pageStartX = Console.WindowWidth / 2 - 30;
            int pageEndX = Console.WindowWidth / 2 + 30;

            ZConsole.DrawBox(pageStartX, pageEndX, 2, 20);
            ZConsole.Write("ZINVSYS Customer Management Menu", pageStartX, 1, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.Write("[CREATE NEW CUSTOMER]", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);

            string firstName, lastName, username, password = string.Empty, confirmPassword = string.Empty, address = string.Empty;

            ZConsole.Write("Enter First Name: ", pageStartX + 2, 5, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            firstName = Console.ReadLine();

            if (string.IsNullOrEmpty(firstName)) throw new Exception("Please Enter First Name!");

            ZConsole.Write("Enter Last Name: ", pageStartX + 2, 6, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            lastName = Console.ReadLine();

            if (string.IsNullOrEmpty(lastName)) throw new Exception("Please Enter Last Name!");

            ZConsole.Write("Enter Username: ", pageStartX + 2, 7, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            username = Console.ReadLine();

            if (string.IsNullOrEmpty(username)) throw new Exception("Please Enter Username!");

            ZConsole.Write("Enter Password: ", pageStartX + 2, 8, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);

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

            ZConsole.Write("Re-Enter Password: ", pageStartX + 2, 9, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
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

            ZConsole.Write("Please Enter Address: ", pageStartX + 2, 10, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            address = Console.ReadLine();

            if (string.IsNullOrEmpty(address)) throw new Exception("Please Enter Address!");

            var existingUser = DBContext<ApplicationUser>.GetByCustomQuery(x => x.Username == username).FirstOrDefault();
            if (existingUser != null) throw new Exception("Username Is Already Used!");

            var applicationUser = new Customer(firstName, lastName, username, password, address);
            applicationUser.CreatedBy = Program.currentUser.Uid;

            DBContext<ApplicationUser>.Add(applicationUser);

            notificationService.DrawMessageBox("Successfully Created Customer!");
        }
        public void DeleteEntity()
        {
            ListEntity();

            if (selectedCustomer == null) return;

            Console.Clear();

            if (selectedCustomer == null) throw new Exception("No Entity Selected!");
            var selectedCustomerEntity = (Customer)DBContext<ApplicationUser>.GetById(selectedCustomer.Value);

            ApplicationUser? createdBy = DBContext<ApplicationUser>.GetById(selectedCustomerEntity.CreatedBy);
            ApplicationUser? lastModifiedBy = (selectedCustomerEntity.LastModifiedBy == null) ? null : DBContext<ApplicationUser>.GetById(selectedCustomerEntity.LastModifiedBy.Value);

            int pageStartX = Console.WindowWidth / 2 - 40;
            int pageEndX = Console.WindowWidth / 2 + 40;

            ZConsole.DrawBox(pageStartX, pageEndX, 2, 25);
            ZConsole.Write("ZINVSYS Customer Management", pageStartX, 1, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.Write("[DELETE CUSTOMER]", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.DrawClosedPipe(4, pageStartX, pageEndX);

            ZConsole.Write("UID: " + selectedCustomerEntity.Uid, pageStartX + 2, 5, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("First Name: " + selectedCustomerEntity.FirstName, pageStartX + 2, 6, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Last Name: " + selectedCustomerEntity.LastName, pageStartX + 2, 7, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Username: " + selectedCustomerEntity.Username, pageStartX + 2, 8, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Address: " + selectedCustomerEntity.Address, pageStartX + 2, 9, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Created On: " + selectedCustomerEntity.CreatedOn.ToString("MM/dd/yyyy hh:mm tt"), pageStartX + 2, 10, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Created By: " + ((createdBy == null) ? "[unable to retrieve user]" : createdBy.Username), pageStartX + 2, 11, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Last Modified On: " + ((selectedCustomerEntity.LastModifiedOn == null) ? "" : selectedCustomerEntity.LastModifiedOn.ToString()), pageStartX + 2, 12, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Last Modified By: " + ((lastModifiedBy == null) ? "[unable to retrieve user]" : lastModifiedBy.Username), pageStartX + 2, 13, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);

            ZConsole.DrawClosedPipe(14, pageStartX, pageEndX);
            ZConsole.Write("Are you sure you want to delete this record?", pageStartX, 15, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.DrawClosedPipe(16, pageStartX, pageEndX);

            ZConsole.Write("Enter Selection (Y/N): ", pageStartX + 1, 17, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            var selection = Console.ReadKey();

            if (selection.Key == ConsoleKey.Y)
            {
                DBContext<ApplicationUser>.Delete(selectedCustomer.Value, Program.currentUser.Uid);
                notificationService.DrawMessageBox("Successfully Deleted Customer!");
            }

            selectedCustomer = null;

            DeleteEntity();
        }

        public void DrawMenu()
        {
            DrawScrollerSelection("Manage Customers", new ScrollerOption[]
            {
                new ScrollerOption("List Customer", contextType, ApplicationStateFlagsEnum.LIST),
                new ScrollerOption("Create New Customer", contextType, ApplicationStateFlagsEnum.CREATE),
                new ScrollerOption("Edit Customer", contextType, ApplicationStateFlagsEnum.EDIT),
                new ScrollerOption("Delete Customer", contextType, ApplicationStateFlagsEnum.DELETE),
                new ScrollerOption("Generate Transactions Report", contextType, ApplicationStateFlagsEnum.REPORTS),
                new ScrollerOption("Back", contextType, ApplicationStateFlagsEnum.BACK),
            });
        }

        public void ListEntity()
        {
            int pageStartX = Console.WindowWidth / 2 - 50;
            int pageEndX = Console.WindowWidth / 2 + 50;

            string additionalTitle = string.Empty;

            if (Program.navigationHistory.Peek() == ApplicationStateFlagsEnum.EDIT) additionalTitle = "(Select Record To Edit)";
            else if (Program.navigationHistory.Peek() == ApplicationStateFlagsEnum.DELETE) additionalTitle = "(Select Record To Delete)";

            ConsoleKeyInfo? command = null;

            int page = 0, rowPerPage = 18, scrollerIndex = 0, pageCount = 0;
            int maxScrollerIndex = 0, lastPageMaxScrollerIndex = 0;
            string? searchQuery = null;

            int columnWidth = pageEndX - pageStartX + 4;

            scrollerIndex = 0;

            List<ApplicationUser> allrecords = DBContext<ApplicationUser>.GetByCustomQuery(x=>x.UserType == ApplicationUserTypeEnum.CUSTOMER);
            List<ApplicationUser> records = new List<ApplicationUser>(allrecords);


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
                                scrollerIndex = rowPerPage-1;
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
                        case ConsoleKey.Enter:
                            selectedCustomer = records[scrollerIndex].Uid;
                            switch (Program.navigationHistory.Peek())
                            {
                                case ApplicationStateFlagsEnum.LIST:
                                    ViewEntityDetails();
                                    break;
                                default:
                                    return;
                            }
                            break;
                    }
                }


                Console.Clear();


                if (string.IsNullOrEmpty(searchQuery))
                {
                    records = allrecords.ToList();
                }
                else
                {
                    records = allrecords.Where(x => (x.CompleteName.ToLower().Contains(searchQuery.ToLower()) ||
                        x.Username.ToLower().Contains(searchQuery.ToLower()))).ToList();

                    ZConsole.Write($"Searching: \"{searchQuery}\"", pageStartX + 2, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
                }

                decimal div = (decimal)records.Count() / rowPerPage;
                pageCount = int.Parse(Math.Ceiling(div).ToString());
                pageCount = (pageCount == 0) ? 1 : pageCount;

                lastPageMaxScrollerIndex = (records.Count() % rowPerPage) - 1;
                records = records.Skip(page * rowPerPage).Take(rowPerPage).ToList();

                maxScrollerIndex = records.Count;


                ZConsole.DrawBox(pageStartX, pageEndX, 2, 27);
                ZConsole.Write("ZINVSYS Customer Management", pageStartX, 1, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write($"[CUSTOMERS TABLE] {additionalTitle}", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write($"(Page {page + 1}/{pageCount}) ", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.RIGHT);

                int recStartLocY = 7;

                ZConsole.DrawClosedPipe(4, pageStartX, pageEndX);

                int remainingSpaces = pageEndX - (pageStartX + 11);
                int colSize = remainingSpaces / 3;

                ZConsole.Write("#", pageStartX + 1, 5, pageStartX + 4, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("ID", pageStartX + 6, 5, pageStartX + 11, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("Username", pageStartX + 11, 5, (pageStartX + 11) + (colSize * 1), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("Complete Name", (pageStartX + 11) + (colSize * 1), 5, (pageStartX + 11) + (colSize * 2), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("Created On", (pageStartX + 11) + (colSize * 2), 5, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);

                ZConsole.DrawClosedPipe(6, pageStartX, pageEndX);
                ZConsole.DrawClosedPipe(25, pageStartX, pageEndX);

                ZConsole.Write("ESC - Back | W - Up | S - Down | A - Prv Pg. | D - Nxt Pg. | F1 - Search | Enter - Select", pageStartX + 1, 26, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);



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

                    ZConsole.Write((i + 1).ToString(), pageStartX + 1, recStartLocY + i, pageStartX + 4, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    ZConsole.Write(_entity.Uid.ToString(), pageStartX + 6, recStartLocY + i, pageStartX + 11, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    ZConsole.Write(_entity.Username, pageStartX + 11, recStartLocY + i, (pageStartX + 11) + (colSize * 1), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    ZConsole.Write(_entity.CompleteName, (pageStartX + 11) + (colSize * 1), recStartLocY + i, (pageStartX + 11) + (colSize * 2), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    ZConsole.Write(_entity.CreatedOn.ToString("MM/dd/yyyy hh:mm tt"), (pageStartX + 11) + (colSize * 2), recStartLocY + i, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);

                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                command = Console.ReadKey();
            }
            while (command.Value.Key != ConsoleKey.Escape);
        }

        public void SearchEntity()
        {
            throw new NotImplementedException();
        }

        public void UpdateEntity()
        {
            ListEntity();


            if (selectedCustomer == null) return;

            Console.Clear();

            if (selectedCustomer == null) throw new Exception("No Entity Selected!");
            var selectedCustomerEntity = (Customer)DBContext<ApplicationUser>.GetById(selectedCustomer.Value);

            ApplicationUser? createdBy = DBContext<ApplicationUser>.GetById(selectedCustomerEntity.CreatedBy);
            ApplicationUser? lastModifiedBy = (selectedCustomerEntity.LastModifiedBy == null) ? null : DBContext<ApplicationUser>.GetById(selectedCustomerEntity.LastModifiedBy.Value);

            int pageStartX = Console.WindowWidth / 2 - 40;
            int pageEndX = Console.WindowWidth / 2 + 40;

            ZConsole.DrawBox(pageStartX, pageEndX, 2, 25);
            ZConsole.Write("ZINVSYS Customer Management", pageStartX, 1, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.Write("[EDIT Customer DETAILS]", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.DrawClosedPipe(4, pageStartX, pageEndX);

            ZConsole.Write("UID: " + selectedCustomerEntity.Uid, pageStartX + 2, 5, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("First Name: " + selectedCustomerEntity.FirstName, pageStartX + 2, 6, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Last Name: " + selectedCustomerEntity.LastName, pageStartX + 2, 7, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Username: " + selectedCustomerEntity.Username, pageStartX + 2, 8, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Address: " + selectedCustomerEntity.Address, pageStartX + 2, 9, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);

            ZConsole.Write("Created On: " + selectedCustomerEntity.CreatedOn.ToString("MM/dd/yyyy hh:mm tt"), pageStartX + 2, 10, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Created By: " + ((createdBy == null) ? "[unable to retrieve user]" : createdBy.Username), pageStartX + 2, 11, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Last Modified On: " + ((selectedCustomerEntity.LastModifiedOn == null) ? "" : selectedCustomerEntity.LastModifiedOn.ToString()), pageStartX + 2, 12, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Last Modified By: " + ((lastModifiedBy == null) ? "[unable to retrieve user]" : lastModifiedBy.Username), pageStartX + 2, 13, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);

            ZConsole.DrawClosedPipe(14, pageStartX, pageEndX);
            ZConsole.Write("Enter New Details", pageStartX, 15, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.DrawClosedPipe(16, pageStartX, pageEndX);


            string firstName, lastName, username, password = string.Empty, confirmPassword = string.Empty, address;

            ZConsole.Write("Enter First Name: ", pageStartX + 2, 17, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            firstName = Console.ReadLine();

            if (string.IsNullOrEmpty(firstName)) throw new Exception("Please Enter First Name!");

            ZConsole.Write("Enter Last Name: ", pageStartX + 2, 18, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            lastName = Console.ReadLine();

            if (string.IsNullOrEmpty(lastName)) throw new Exception("Please Enter Last Name!");

            ZConsole.Write("Enter Username: ", pageStartX + 2, 19, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            username = Console.ReadLine();

            if (string.IsNullOrEmpty(username)) throw new Exception("Please Enter Username!");

            ZConsole.Write("Enter Password: ", pageStartX + 2, 20, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);

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

            ZConsole.Write("Re-Enter Password: ", pageStartX + 2, 21, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
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

            ZConsole.Write("Enter Address: ", pageStartX + 2, 22, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            address = Console.ReadLine();

            if (string.IsNullOrEmpty(address)) throw new Exception("Please Enter Address!");

            var existingUser = DBContext<ApplicationUser>.GetByCustomQuery(x => x.Username == username && x.Uid != selectedCustomer.Value).FirstOrDefault();
            if (existingUser != null) throw new Exception("Username Is Already Used!");

            selectedCustomerEntity.FirstName = firstName;
            selectedCustomerEntity.LastName = lastName;
            selectedCustomerEntity.Username = username;
            selectedCustomerEntity.LastModifiedBy = Program.currentUser.Uid;
            selectedCustomerEntity.Password = password;
            selectedCustomerEntity.Address = address;


            DBContext<ApplicationUser>.Update(selectedCustomerEntity);

            notificationService.DrawMessageBox("Successfully Updated Customer!");
            selectedCustomer = null;
            UpdateEntity();
        }

        public void ViewEntityDetails()
        {
            Console.Clear();

            if (selectedCustomer == null) throw new Exception("No Entity Selected!");
            var selectedAdminEntity = DBContext<ApplicationUser>.GetById(selectedCustomer.Value);

            ApplicationUser? createdBy = DBContext<ApplicationUser>.GetById(selectedAdminEntity.CreatedBy);
            ApplicationUser? lastModifiedBy = (selectedAdminEntity.LastModifiedBy == null) ? null : DBContext<ApplicationUser>.GetById(selectedAdminEntity.LastModifiedBy.Value);

            int pageStartX = Console.WindowWidth / 2 - 30;
            int pageEndX = Console.WindowWidth / 2 + 30;

            ZConsole.DrawBox(pageStartX, pageEndX, 2, 22);
            ZConsole.Write("ZINVSYS Customer Management", pageStartX, 1, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.Write("[VIEW SELECTED CUSTOMER DETAILS]", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);

            ZConsole.Write("UID: " + selectedAdminEntity.Uid, pageStartX + 2, 5, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("First Name: " + selectedAdminEntity.FirstName, pageStartX + 2, 6, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Last Name: " + selectedAdminEntity.LastName, pageStartX + 2, 7, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Username: " + selectedAdminEntity.Username, pageStartX + 2, 8, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Address: " + selectedAdminEntity.Username, pageStartX + 2, 9, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Created On: " + selectedAdminEntity.CreatedOn.ToString("MM/dd/yyyy hh:mm tt"), pageStartX + 2, 11, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Created By: " + ((createdBy == null) ? "[unable to retrieve user]" : createdBy.Username), pageStartX + 2, 12, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Last Modified On: " + ((selectedAdminEntity.LastModifiedOn == null) ? "" : selectedAdminEntity.LastModifiedOn.ToString()), pageStartX + 2, 14, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Last Modified By: " + ((lastModifiedBy == null) ? "[unable to retrieve user]" : lastModifiedBy.Username), pageStartX + 2, 15, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);


            ZConsole.Write("Press Any Key To Go Back...", pageStartX + 1, 21, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            Console.ReadKey();

            selectedCustomer = null;
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

        public T DialogGetEntityFromList<T>()
        {
            ListEntity();
            if (selectedCustomer == null) return default(T);
            var entity = DBContext<ApplicationUser>.GetById(selectedCustomer.Value);
            selectedCustomer = null;
            return (T)(object)entity;
        }

        public void GenerateReportSummary()
        {
            Console.Clear();

            var transactions = DBContext<Entities.Transaction>.GetByCustomQuery(x => x.TransactionStatus == TransactionStatusEnum.PAID).ToList();


            int pageStartX = Console.WindowWidth / 2 - 60;
            int pageEndX = Console.WindowWidth / 2 + 60;

            string additionalTitle = string.Empty;

            if (Program.navigationHistory.Peek() == ApplicationStateFlagsEnum.EDIT) additionalTitle = "(Select Record To Edit)";
            else if (Program.navigationHistory.Peek() == ApplicationStateFlagsEnum.DELETE) additionalTitle = "(Select Record To Delete)";

            ConsoleKeyInfo? command = null;

            int page = 0, rowPerPage = 18, scrollerIndex = 0, pageCount = 0;
            int maxScrollerIndex = 0, lastPageMaxScrollerIndex = 0;
            string? searchQuery = null;

            int columnWidth = pageEndX - pageStartX + 4;

            scrollerIndex = 0;

            List<CustomerOrdersSummaryViewModel> allrecords = transactions.GroupBy(x=>x.CustomerId).Select(x => new CustomerOrdersSummaryViewModel
            {
                Username = DBContext<ApplicationUser>.GetById(x.Key.Value).Username,
                CustomerName = DBContext<ApplicationUser>.GetById(x.Key.Value).CompleteName,
                TotalAmountGarnered = x.Sum(y => y.TotalAmountDue),
                TotalTransactionCount = x.Count()
            }).ToList();

            List<CustomerOrdersSummaryViewModel> records = new List<CustomerOrdersSummaryViewModel>(allrecords);

            int orderMode = 0;

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
                            orderMode++;
                            if (orderMode > 2) orderMode = 0;
                            break;
                    }
                }


                Console.Clear();


                if (string.IsNullOrEmpty(searchQuery))
                {
                    records = allrecords.ToList();
                }
                else
                {
                    records = allrecords.Where(x => x.CustomerName.Contains(searchQuery)).ToList();

                    ZConsole.Write($"Searching: \"{searchQuery}\"", pageStartX + 2, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
                }

                switch (orderMode)
                {
                    case 0:
                        records = records.OrderBy(x => x.CustomerName).ToList();
                        additionalTitle = "(ORDERED BY CUSTOMER NAME)";
                        break;
                    case 1:
                        records = records.OrderByDescending(x => x.TotalTransactionCount).ToList();
                        additionalTitle = "(ORDERED BY NO. OF ORDERS)";
                        break;
                    case 2:
                        records = records.OrderByDescending(x => x.TotalAmountGarnered).ToList();
                        additionalTitle = "(ORDERED BY TOTAL AMOUNT)";
                        break;
                }

                decimal div = (decimal)records.Count() / rowPerPage;
                pageCount = int.Parse(Math.Ceiling(div).ToString());
                pageCount = (pageCount == 0) ? 1 : pageCount;

                lastPageMaxScrollerIndex = (records.Count() % rowPerPage) - 1;
                records = records.Skip(page * rowPerPage).Take(rowPerPage).ToList();

                maxScrollerIndex = records.Count;


                ZConsole.DrawBox(pageStartX, pageEndX, 2, 27);
                ZConsole.Write("ZINVSYS Sales Report", pageStartX, 1, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write($"[SALES SUMMARY] {additionalTitle}", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write($"(Page {page + 1}/{pageCount}) ", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.RIGHT);

                int recStartLocY = 7;

                ZConsole.DrawClosedPipe(4, pageStartX, pageEndX);

                int remainingSpaces = pageEndX - (pageStartX + 6);
                int colSize = remainingSpaces / 3;

                ZConsole.Write("#", pageStartX + 1, 5, pageStartX + 4, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("Customer", pageStartX + 6, 5, (pageStartX + 6) + (colSize * 1), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("No of. Orders", (pageStartX + 6) + (colSize * 1), 5, (pageStartX + 11) + (colSize * 2), null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                ZConsole.Write("Total Amount Garnered", (pageStartX + 11) + (colSize * 2), 5, pageEndX - 2, null, flag: ZConsole.ConsoleFormatFlags.RIGHT);

                ZConsole.DrawClosedPipe(6, pageStartX, pageEndX);
                ZConsole.DrawClosedPipe(25, pageStartX, pageEndX);

                ZConsole.Write("ESC - Back | W - Up | S - Down | A - Prv Pg. | D - Nxt Pg. | F1 - Search | F2 - Switch Order Mode ", pageStartX + 1, 26, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);



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

                    ZConsole.Write((i + 1).ToString(), pageStartX + 1, recStartLocY + i, pageStartX + 4, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    ZConsole.Write($"({_entity.Username})" + _entity.CustomerName, pageStartX + 6, recStartLocY + i, (pageStartX + 6) + (colSize * 1), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    ZConsole.Write(_entity.TotalTransactionCount.ToString(), (pageStartX + 6) + (colSize * 1), recStartLocY + i, (pageStartX + 6) + (colSize * 2), null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                    ZConsole.Write("PHP " + _entity.TotalAmountGarnered.ToString("N2"), (pageStartX + 6) + (colSize * 2), recStartLocY + i, pageEndX - 2, null, flag: ZConsole.ConsoleFormatFlags.RIGHT);

                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                command = Console.ReadKey();
            }
            while (command.Value.Key != ConsoleKey.Escape);
        }
    }
}
