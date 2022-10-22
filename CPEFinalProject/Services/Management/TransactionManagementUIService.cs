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
using System.Transactions;

namespace CPEFinalProject.Services.Management
{
    public class TransactionManagementUIService : IManagementGUIService
    {
        readonly INotificationService notificationService;
        readonly IManagementGUIService customerManagementUIService;
        readonly IExceptionHandlerService exceptionHandlerService;
        readonly IManagementGUIService productStockManagementUIService;
        readonly Type contextType;
        int? selectedTransaction = null;

        public TransactionManagementUIService(INotificationService notificationService, IManagementGUIService customerManagementUIService, IExceptionHandlerService exceptionHandlerService, IManagementGUIService productStockManagementUIService)
        {
            this.notificationService = notificationService;
            contextType = typeof(Entities.Transaction);
            this.customerManagementUIService = customerManagementUIService;
            this.exceptionHandlerService = exceptionHandlerService;
            this.productStockManagementUIService = productStockManagementUIService;
        }

        public void CreateEntity()
        {
            Entities.Transaction transaction = new Entities.Transaction();

            transaction.TransactionId = RandomStringGeneratorService.GenerateRandomString(8);
            transaction.AdditionalFees = 0;
            transaction.TransactionStatus = TransactionStatusEnum.PENDING;

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

                if(transaction.CustomerId != null)
                {
                    customer = (Customer)DBContext<ApplicationUser>.GetById(transaction.CustomerId.Value);
                }

                switch (command.Key)
                {
                    case ConsoleKey.F1:
                        customer = customerManagementUIService.DialogGetEntityFromList<Customer>();
                        if (customer == null) break;

                        transaction.CustomerId = customer.Uid;
                        break;
                    case ConsoleKey.F2:
                        ZConsole.DrawClosedPipe(21, pageStartX + 2, pageStartX + 40);
                        ZConsole.Write("SET ADDITIONAL FEES", pageStartX + 2, 22, pageStartX + 40, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                        ZConsole.DrawClosedPipe(23, pageStartX + 2, pageStartX + 40);
                        ZConsole.Write("Enter Value: ", pageStartX + 3, 24, pageStartX + 40, null);

                        var _additionalFees = Console.ReadLine();
                        decimal additionalFees;
                        try
                        {
                            if (!decimal.TryParse(_additionalFees, out additionalFees)) throw new Exception("Please Input A Valid Value for \"Additional Fees\"!");
                            transaction.AdditionalFees = additionalFees;
                        }
                        catch (Exception ex)
                        {
                            exceptionHandlerService.Handle(ex, ApplicationStateFlagsEnum.CREATE, typeof(Entities.Transaction));
                            continue;
                        }
                        break;
                    case ConsoleKey.F3:
                        var selectedStock = productStockManagementUIService.DialogGetEntityFromList<ProductStock>();

                        if (selectedStock == null) break;

                        var product = DBContext<ProductProfile>.GetById(selectedStock.ProductId);

                        Console.Clear();

                        int _pageStartX = Console.WindowWidth / 2 - 20;
                        int _pageEndX = Console.WindowWidth / 2 + 20;
                        int quantity;

                        ZConsole.DrawBox(_pageStartX, _pageEndX, 8, 15);
                        ZConsole.Write("ZINVSYS Transaction Cart", _pageStartX, 7, _pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                        ZConsole.Write("[ADD ITEM TO CART]", _pageStartX, 9, _pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);

                        ZConsole.Write("Enter Quantity: ", _pageStartX + 2, 11, null, null);
                        var _quantity = Console.ReadLine();

                        try
                        {
                            if (!int.TryParse(_quantity, out quantity)) throw new Exception("Please Input A Valid Quantity To Add Item to Cart!");
                            if (quantity > selectedStock.Quantity) throw new Exception("Please Input A Valid Quantity (Must Be Less Than Or Equal To Qty in Stock!)");
                        }
                        catch (Exception ex)
                        {
                            exceptionHandlerService.Handle(ex, ApplicationStateFlagsEnum.CREATE, typeof(Entities.Transaction));
                            break;
                        }

                        transaction.AddTransactionItem(selectedStock.ProductId, selectedStock.Batch, quantity, product.Price, selectedStock.MfgDate, selectedStock.ExpDate);

                        break;
                    case ConsoleKey.F4:
                        ZConsole.DrawClosedPipe(21, pageStartX + 2, pageStartX + 40);
                        ZConsole.Write("SET TRANSACTION STATUS", pageStartX + 2, 22, pageStartX + 40, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                        ZConsole.DrawClosedPipe(23, pageStartX + 2, pageStartX + 40);

                        ZConsole.Write("1] PAID ", pageStartX + 3, 24, pageStartX + 40, null);
                        ZConsole.Write("2] PENDING", pageStartX + 3, 25, pageStartX + 40, null);
                        ZConsole.Write("3] CANCELLED", pageStartX + 3, 26, pageStartX + 40, null);

                        ZConsole.Write("Enter Selection: ", pageStartX + 3, 28, pageStartX + 40, null);

                        var _status = Console.ReadLine();
                        int _transactionStatus;

                        try
                        {
                            if (!int.TryParse(_status, out _transactionStatus)) throw new Exception("Please Input A Valid Value for \"Transaction Status\"!");
                            if (_transactionStatus != 1 && _transactionStatus != 2 && _transactionStatus != 3) throw new Exception("Please Input A Valid Value for \"Transaction Status\"!");
                            transaction.TransactionStatus = (TransactionStatusEnum)(_transactionStatus - 1);
                        }
                        catch (Exception ex)
                        {
                            exceptionHandlerService.Handle(ex, ApplicationStateFlagsEnum.CREATE, typeof(Entities.Transaction));
                            continue;
                        }
                        break;
                    case ConsoleKey.F5:
                        ZConsole.DrawClosedPipe(21, pageStartX + 2, pageStartX + 40);
                        ZConsole.Write("SET REMARKS", pageStartX + 2, 22, pageStartX + 40, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                        ZConsole.DrawClosedPipe(23, pageStartX + 2, pageStartX + 40);
                        ZConsole.Write("Enter Remarks: ", pageStartX + 3, 24, pageStartX + 40, null);
                        Console.SetCursorPosition(pageStartX + 5, 25);
                        transaction.Remarks = Console.ReadLine();
                        break;
                    case ConsoleKey.F6:
                        try
                        {
                            if (transaction.CustomerId == null) throw new Exception("Please Select Customer!");
                            if (transaction.TransactionItems.Count == 0) throw new Exception("No Items Detected in Cart! Please Set At Least 1 Item to Proceed!");

                            if (transaction.TransactionStatus == TransactionStatusEnum.CANCELLED) transaction.RollBackTransactionItems();


                            ZConsole.DrawClosedPipe(21, pageStartX + 2, pageStartX + 40);
                            ZConsole.Write("ARE YOU SURE (Y/N)?", pageStartX + 2, 22, pageStartX + 40, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                            ZConsole.DrawClosedPipe(23, pageStartX + 2, pageStartX + 40);
                            ZConsole.Write("Are you sure you would like to proceed? (Y/N): ", pageStartX + 3, 24, pageStartX + 40, 28, flag: ZConsole.ConsoleFormatFlags.TOP_LEFT);
                            var entry = Console.ReadKey();

                            if (entry.Key != ConsoleKey.Y) break;

                            transaction.BufferItems = new List<TransactionItem>();

                            DBContext<Entities.Transaction>.Add(transaction);

                            notificationService.DrawMessageBox("Successfully Saved Transaction!");
                            return;
                        }catch(Exception ex)
                        {
                            exceptionHandlerService.Handle(ex, ApplicationStateFlagsEnum.CREATE, typeof(Entities.Transaction));
                        }

                        break;
                    case ConsoleKey.Escape:
                        transaction.RollBackTransactionItems();
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
                    case ConsoleKey.Delete:
                        var cartItem = records[scrollerIndex];
                        transaction.RemoveTransactionItem(cartItem.ProductId, cartItem.Batch, cartItem.Quantity);
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
                }

                // transaction details 
                Console.Clear();

                ZConsole.DrawBox(pageStartX, pageEndX, 2, 30);
                ZConsole.Write("ZINVSYS Transaction Management Menu", pageStartX, 1, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("[CREATE NEW TRANSACTION]", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.DrawClosedPipe(4, pageStartX, pageEndX);
                ZConsole.Write("Transaction Details", pageStartX + 2, 6, pageStartX + 40, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.DrawBox(pageStartX + 2, pageStartX + 40, 5, 29);
                ZConsole.DrawClosedPipe(7, pageStartX + 2, pageStartX + 40);

                ZConsole.Write("Transaction ID: " + transaction.TransactionId, pageStartX + 3, 8, pageStartX + 40, null);
                ZConsole.Write("(F1) - Set Customer: " + ((customer == null) ? string.Empty : customer.Username), pageStartX + 3, 9, pageStartX + 40, null);
                ZConsole.Write("(F2) - Set Fees: PHP " + transaction.AdditionalFees.ToString("N2"), pageStartX + 3, 10, pageStartX + 40, null);
                ZConsole.Write($"(F3) - Add Cart Items: ", pageStartX + 3, 11, pageStartX + 40, null);
                ZConsole.Write("(F4) - Set Status: " + transaction.TransactionStatus.ToString(), pageStartX + 3, 12, pageStartX + 40, null);
                ZConsole.Write("Total Amount: PHP " + transaction.TotalAmountDue.ToString("N2"), pageStartX + 3, 14, pageStartX + 40, null);
                ZConsole.Write("(F5) - Set Remarks: ", pageStartX + 3, 16, pageStartX + 40, null);
                ZConsole.Write(transaction.Remarks ?? "", pageStartX + 5, 17, pageStartX + 40, 18, flag: ZConsole.ConsoleFormatFlags.TOP_LEFT);

                ZConsole.Write("(F6) - [Create Transaction]", pageStartX + 3, 19, pageStartX + 40, null);
                ZConsole.Write("(ESC) - [Back to Menu]", pageStartX + 3, 20, pageStartX + 40, null);

                // cart

                if (string.IsNullOrEmpty(searchQuery))
                {
                    records = transaction.TransactionItems;
                }
                else
                {
                    records = transaction.TransactionItems.Where(x => DBContext<ProductProfile>.GetById(x.ProductId).ProductName.Contains(searchQuery)).ToList();
                    ZConsole.Write($"Searching: \"{searchQuery}\"", pageStartX + 42, 6, pageEndX-1, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
                }
                
                decimal div = (decimal)records.Count() / rowPerPage;
                pageCount = int.Parse(Math.Ceiling(div).ToString());
                pageCount = (pageCount == 0) ? 1 : pageCount;

                lastPageMaxScrollerIndex = (records.Count() % rowPerPage) - 1;
                records = records.Skip(page * rowPerPage).Take(rowPerPage).ToList();

                maxScrollerIndex = records.Count;


                ZConsole.DrawBox(pageStartX + 42, pageEndX - 1, 5, 29);
                ZConsole.Write("Transaction Cart", pageStartX + 42, 6, pageEndX - 1, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write($"(Page {page + 1}/{pageCount}) ", pageStartX, 6, pageEndX-1, null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                ZConsole.DrawClosedPipe(7, pageStartX + 42, pageEndX - 1);

                int recStartLocY = 10;

                int remainingSpaces = pageEndX - (pageStartX + 52);
                int colSize = remainingSpaces / 4;

                ZConsole.Write("#", pageStartX + 42, 8, pageStartX + 46, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("BN", pageStartX + 48, 8, pageStartX + 53, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("Product Identifier", pageStartX + 53, 8, (pageStartX + 53) + (colSize * 1), null, flag: ZConsole.ConsoleFormatFlags.LEFT);
                ZConsole.Write("Quantity", (pageStartX + 53) + (colSize * 1), 8, (pageStartX + 53) + (colSize * 2), null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                ZConsole.Write("Price", (pageStartX + 53) + (colSize * 2), 8, (pageStartX + 53) + (colSize * 3), null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                ZConsole.Write("Total", (pageStartX + 53) + (colSize * 3), 8, pageEndX-2, null, flag: ZConsole.ConsoleFormatFlags.RIGHT);


                ZConsole.DrawClosedPipe(9, pageStartX + 42, pageEndX-1);
                ZConsole.DrawClosedPipe(27, pageStartX + 42, pageEndX-1);
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
                    ZConsole.Write(_entity.Quantity.ToString(), (pageStartX + 53) + (colSize * 1), recStartLocY + i, (pageStartX + 53) + (colSize * 2), null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                    ZConsole.Write("PHP " + _entity.Price.ToString("N2"), (pageStartX + 53) + (colSize * 2), recStartLocY + i, (pageStartX + 53) + (colSize * 3), null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                    ZConsole.Write("PHP " + (_entity.Quantity*_entity.Price).ToString("N2"), (pageStartX + 53) + (colSize * 3), recStartLocY + i, pageEndX-2, null, flag: ZConsole.ConsoleFormatFlags.RIGHT);

                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                command = Console.ReadKey();

            }

        }

        public void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        public T DialogGetEntityFromList<T>()
        {
            throw new NotImplementedException();
        }

        public void DrawMenu()
        {
            Program.DrawScrollerSelection("ZINVSYS Transactions Management Menu ", new ScrollerOption[]
            {
                new ScrollerOption("List Transactions", contextType, ApplicationStateFlagsEnum.LIST),
                new ScrollerOption("Create New Transaction", contextType, ApplicationStateFlagsEnum.CREATE),
                new ScrollerOption("Edit Existing Transaction", contextType, ApplicationStateFlagsEnum.EDIT),
                new ScrollerOption("Generate Transactions Report", contextType, ApplicationStateFlagsEnum.REPORTS),
                new ScrollerOption("Back", null, ApplicationStateFlagsEnum.BACK),
            });
        }

        public void GenerateReportSummary()
        {
            Console.Clear();


            var transactions = DBContext<Entities.Transaction>.GetByCustomQuery(x => x.TransactionStatus == TransactionStatusEnum.PAID).ToList();
            var flattenedData = transactions.SelectMany(x => x.TransactionItems).ToList();

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

            List<SalesSummaryViewModel> allrecords = flattenedData.GroupBy(x => x.ProductId).Select(x => new SalesSummaryViewModel
            {
                ProductIdentifier = DBContext<ProductProfile>.GetById(x.Key).ProductIdentifier,
                ProductName = DBContext<ProductProfile>.GetById(x.Key).ProductName,
                GrossAmountEarned = x.Sum(y => y.SubTotalPrice),
                TotalQuantitySold = x.Sum(y => y.Quantity)
            }).ToList();

            List<SalesSummaryViewModel> records = new List<SalesSummaryViewModel>(allrecords);

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
                    records = allrecords.Where(x => x.ProductIdentifier.Contains(searchQuery)).ToList();

                    ZConsole.Write($"Searching: \"{searchQuery}\"", pageStartX + 2, 3, (pageEndX / 2) - 5, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
                }


                switch (orderMode)
                {
                    case 0:
                        records = records.OrderBy(x => x.ProductName).ToList();
                        additionalTitle = "(ORDERED BY PRODUCT NAME)";
                        break;
                    case 1:
                        records = records.OrderByDescending(x => x.TotalQuantitySold).ToList();
                        additionalTitle = "(ORDERED BY QUANTITY SOLD)";
                        break;
                    case 2:
                        records = records.OrderByDescending(x => x.GrossAmountEarned).ToList();
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
                ZConsole.Write("Product Identifier", pageStartX + 6, 5, (pageStartX + 6) + (colSize * 1), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("Quantity Sold", (pageStartX + 6) + (colSize * 1), 5, (pageStartX + 11) + (colSize * 2), null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                ZConsole.Write("Total Amount Garnered", (pageStartX + 11) + (colSize * 2), 5, pageEndX-2, null, flag: ZConsole.ConsoleFormatFlags.RIGHT);

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
                    ZConsole.Write(_entity.ProductIdentifier.ToString(), pageStartX + 6, recStartLocY + i, (pageStartX + 6) + (colSize * 1), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    ZConsole.Write(_entity.TotalQuantitySold.ToString(), (pageStartX + 6) + (colSize * 1), recStartLocY + i, (pageStartX + 6) + (colSize * 2), null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                    ZConsole.Write("PHP " + _entity.GrossAmountEarned.ToString("N2"), (pageStartX + 6) + (colSize * 2), recStartLocY + i,pageEndX-2, null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                    
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                command = Console.ReadKey();
            }
            while (command.Value.Key != ConsoleKey.Escape);
        }

        public void ListEntity()
        {
            int pageStartX = Console.WindowWidth / 2 - 80;
            int pageEndX = Console.WindowWidth / 2 + 80;

            string additionalTitle = string.Empty;


            if (Program.navigationHistory.Peek() == ApplicationStateFlagsEnum.EDIT) additionalTitle = "(Select Record To Edit)";
            else if (Program.navigationHistory.Peek() == ApplicationStateFlagsEnum.DELETE) additionalTitle = "(Select Record To Delete)";

            ConsoleKeyInfo? command = null;

            int page = 0, rowPerPage = 18, scrollerIndex = 0, pageCount = 0;
            int maxScrollerIndex = 0, lastPageMaxScrollerIndex = 0;
            string? searchQuery = null;

            int columnWidth = pageEndX - pageStartX + 4;

            scrollerIndex = 0;

            List<Entities.Transaction> allrecords = DBContext<Entities.Transaction>.GetAll();
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

                additionalTitle = $"Showing \"{((TransactionStatusEnum)mode).ToString().ToUpper()}\" Transactions";

                Console.Clear();


                if (string.IsNullOrEmpty(searchQuery))
                {
                    records = allrecords.Where(x=>x.TransactionStatus == (TransactionStatusEnum)mode).ToList();
                }
                else
                {
                    records = allrecords.Where(x => x.TransactionStatus == (TransactionStatusEnum)mode).Where( x=> x.TransactionId.ToLower().Contains(searchQuery.ToLower()) || 
                    DBContext<ApplicationUser>.GetById(x.CustomerId.Value).Username.ToLower().Contains(searchQuery.ToLower())).ToList();

                    ZConsole.Write($"Searching: \"{searchQuery}\"", pageStartX + 2, 3, (pageEndX / 2) - 5, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
                }


                records = records.OrderByDescending(x => x.Uid).ToList();

                decimal div = (decimal)records.Count() / rowPerPage;
                pageCount = int.Parse(Math.Ceiling(div).ToString());
                pageCount = (pageCount == 0) ? 1 : pageCount;

                lastPageMaxScrollerIndex = (records.Count() % rowPerPage) - 1;
                records = records.Skip(page * rowPerPage).Take(rowPerPage).ToList();

                maxScrollerIndex = records.Count;


                ZConsole.DrawBox(pageStartX, pageEndX, 2, 27);
                ZConsole.Write("ZINVSYS Transactions Management", pageStartX, 1, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write($"[TRANSACTIONS TABLE] {additionalTitle}", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
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

        public void SearchEntity()
        {
            throw new NotImplementedException();
        }

        public void UpdateEntity()
        {
            ListEntity();

            if (selectedTransaction == null) return;

            Entities.Transaction transaction = DBContext<Entities.Transaction>.GetById(selectedTransaction.Value);

            if (transaction == null) throw new Exception("Please Select Record To Edit!");

            if (transaction.TransactionStatus == TransactionStatusEnum.CANCELLED ||
                transaction.TransactionStatus == TransactionStatusEnum.PAID) throw new Exception("Cannot Edit Non-Pending Transactions!");

            transaction.TransactionId = RandomStringGeneratorService.GenerateRandomString(8);
            transaction.AdditionalFees = 0;
            transaction.TransactionStatus = TransactionStatusEnum.PENDING;

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
                    case ConsoleKey.F1:
                        customer = customerManagementUIService.DialogGetEntityFromList<Customer>();
                        if (customer == null) break;

                        transaction.CustomerId = customer.Uid;
                        break;
                    case ConsoleKey.F2:
                        ZConsole.DrawClosedPipe(21, pageStartX + 2, pageStartX + 40);
                        ZConsole.Write("SET ADDITIONAL FEES", pageStartX + 2, 22, pageStartX + 40, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                        ZConsole.DrawClosedPipe(23, pageStartX + 2, pageStartX + 40);
                        ZConsole.Write("Enter Value: ", pageStartX + 3, 24, pageStartX + 40, null);

                        var _additionalFees = Console.ReadLine();
                        decimal additionalFees;
                        try
                        {
                            if (!decimal.TryParse(_additionalFees, out additionalFees)) throw new Exception("Please Input A Valid Value for \"Additional Fees\"!");
                            transaction.AdditionalFees = additionalFees;
                        }
                        catch (Exception ex)
                        {
                            exceptionHandlerService.Handle(ex, ApplicationStateFlagsEnum.CREATE, typeof(Entities.Transaction));
                            continue;
                        }
                        break;
                    case ConsoleKey.F3:
                        var selectedStock = productStockManagementUIService.DialogGetEntityFromList<ProductStock>();

                        if (selectedStock == null) break;

                        var product = DBContext<ProductProfile>.GetById(selectedStock.ProductId);

                        Console.Clear();

                        int _pageStartX = Console.WindowWidth / 2 - 20;
                        int _pageEndX = Console.WindowWidth / 2 + 20;
                        int quantity;

                        ZConsole.DrawBox(_pageStartX, _pageEndX, 8, 15);
                        ZConsole.Write("ZINVSYS Transaction Cart", _pageStartX, 7, _pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                        ZConsole.Write("[ADD ITEM TO CART]", _pageStartX, 9, _pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);

                        ZConsole.Write("Enter Quantity: ", _pageStartX + 2, 11, null, null);
                        var _quantity = Console.ReadLine();

                        try
                        {
                            if (!int.TryParse(_quantity, out quantity)) throw new Exception("Please Input A Valid Quantity To Add Item to Cart!");
                            if (quantity > selectedStock.Quantity) throw new Exception("Please Input A Valid Quantity (Must Be Less Than Or Equal To Qty in Stock!)");
                        }
                        catch (Exception ex)
                        {
                            exceptionHandlerService.Handle(ex, ApplicationStateFlagsEnum.CREATE, typeof(Entities.Transaction));
                            break;
                        }

                        transaction.AddTransactionItem(selectedStock.ProductId, selectedStock.Batch, quantity, product.Price, selectedStock.MfgDate, selectedStock.ExpDate);

                        break;
                    case ConsoleKey.F4:
                        ZConsole.DrawClosedPipe(21, pageStartX + 2, pageStartX + 40);
                        ZConsole.Write("SET TRANSACTION STATUS", pageStartX + 2, 22, pageStartX + 40, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                        ZConsole.DrawClosedPipe(23, pageStartX + 2, pageStartX + 40);

                        ZConsole.Write("1] PAID ", pageStartX + 3, 24, pageStartX + 40, null);
                        ZConsole.Write("2] PENDING", pageStartX + 3, 25, pageStartX + 40, null);
                        ZConsole.Write("3] CANCELLED", pageStartX + 3, 26, pageStartX + 40, null);

                        ZConsole.Write("Enter Selection: ", pageStartX + 3, 28, pageStartX + 40, null);

                        var _status = Console.ReadLine();
                        int _transactionStatus;

                        try
                        {
                            if (!int.TryParse(_status, out _transactionStatus)) throw new Exception("Please Input A Valid Value for \"Transaction Status\"!");
                            if (_transactionStatus != 1 && _transactionStatus != 2 && _transactionStatus != 3) throw new Exception("Please Input A Valid Value for \"Transaction Status\"!");
                            transaction.TransactionStatus = (TransactionStatusEnum)(_transactionStatus - 1);
                        }
                        catch (Exception ex)
                        {
                            exceptionHandlerService.Handle(ex, ApplicationStateFlagsEnum.CREATE, typeof(Entities.Transaction));
                            continue;
                        }
                        break;
                    case ConsoleKey.F5:
                        ZConsole.DrawClosedPipe(21, pageStartX + 2, pageStartX + 40);
                        ZConsole.Write("SET REMARKS", pageStartX + 2, 22, pageStartX + 40, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                        ZConsole.DrawClosedPipe(23, pageStartX + 2, pageStartX + 40);
                        ZConsole.Write("Enter Remarks: ", pageStartX + 3, 24, pageStartX + 40, null);
                        Console.SetCursorPosition(pageStartX + 5, 25);
                        transaction.Remarks = Console.ReadLine();
                        break;
                    case ConsoleKey.F6:
                        try
                        {
                            if (transaction.CustomerId == null) throw new Exception("Please Select Customer!");
                            if (transaction.TransactionItems.Count == 0) throw new Exception("No Items Detected in Cart! Please Set At Least 1 Item to Proceed!");

                            if (transaction.TransactionStatus == TransactionStatusEnum.CANCELLED) transaction.RollBackTransactionItems();


                            ZConsole.DrawClosedPipe(21, pageStartX + 2, pageStartX + 40);
                            ZConsole.Write("ARE YOU SURE (Y/N)?", pageStartX + 2, 22, pageStartX + 40, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                            ZConsole.DrawClosedPipe(23, pageStartX + 2, pageStartX + 40);
                            ZConsole.Write("Are you sure you would like to proceed? (Y/N): ", pageStartX + 3, 24, pageStartX + 40, 28, flag:ZConsole.ConsoleFormatFlags.TOP_LEFT);
                            var entry = Console.ReadKey();

                            if (entry.Key != ConsoleKey.Y) break;

                            transaction.BufferItems = new List<TransactionItem>();


                            DBContext<Entities.Transaction>.Update(transaction);

                            notificationService.DrawMessageBox("Successfully Edited Transaction!");
                            return;
                        }
                        catch (Exception ex)
                        {
                            exceptionHandlerService.Handle(ex, ApplicationStateFlagsEnum.CREATE, typeof(Entities.Transaction));
                        }

                        break;
                    case ConsoleKey.Escape:
                        transaction.RollBackTransactionItems();
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
                    case ConsoleKey.Delete:
                        var cartItem = records[scrollerIndex];
                        transaction.RemoveTransactionItem(cartItem.ProductId, cartItem.Batch, cartItem.Quantity);
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
                }

                // transaction details 
                Console.Clear();

                ZConsole.DrawBox(pageStartX, pageEndX, 2, 30);
                ZConsole.Write("ZINVSYS Transaction Management Menu", pageStartX, 1, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("[EDIT TRANSACTION]", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.DrawClosedPipe(4, pageStartX, pageEndX);
                ZConsole.Write("Transaction Details", pageStartX + 2, 6, pageStartX + 40, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.DrawBox(pageStartX + 2, pageStartX + 40, 5, 29);
                ZConsole.DrawClosedPipe(7, pageStartX + 2, pageStartX + 40);

                ZConsole.Write("Transaction ID: " + transaction.TransactionId, pageStartX + 3, 8, pageStartX + 40, null);
                ZConsole.Write("(F1) - Set Customer: " + ((customer == null) ? string.Empty : customer.Username), pageStartX + 3, 9, pageStartX + 40, null);
                ZConsole.Write("(F2) - Set Fees: PHP " + transaction.AdditionalFees.ToString("N2"), pageStartX + 3, 10, pageStartX + 40, null);
                ZConsole.Write($"(F3) - Add Cart Items: ", pageStartX + 3, 11, pageStartX + 40, null);
                ZConsole.Write("(F4) - Set Status: " + transaction.TransactionStatus.ToString(), pageStartX + 3, 12, pageStartX + 40, null);
                ZConsole.Write("Total Amount: PHP " + transaction.TotalAmountDue.ToString("N2"), pageStartX + 3, 14, pageStartX + 40, null);
                ZConsole.Write("(F5) - Set Remarks: ", pageStartX + 3, 16, pageStartX + 40, null);
                ZConsole.Write(transaction.Remarks ?? "", pageStartX + 5, 17, pageStartX + 40, 18, flag: ZConsole.ConsoleFormatFlags.TOP_LEFT);

                ZConsole.Write("(F6) - [Update Transaction]", pageStartX + 3, 19, pageStartX + 40, null);
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
                int colSize = remainingSpaces / 4;

                ZConsole.Write("#", pageStartX + 42, 8, pageStartX + 46, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("BN", pageStartX + 48, 8, pageStartX + 53, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("Product Identifier", pageStartX + 53, 8, (pageStartX + 53) + (colSize * 1), null, flag: ZConsole.ConsoleFormatFlags.LEFT);
                ZConsole.Write("Quantity", (pageStartX + 53) + (colSize * 1), 8, (pageStartX + 53) + (colSize * 2), null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                ZConsole.Write("Price", (pageStartX + 53) + (colSize * 2), 8, (pageStartX + 53) + (colSize * 3), null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                ZConsole.Write("Total", (pageStartX + 53) + (colSize * 3), 8, pageEndX - 2, null, flag: ZConsole.ConsoleFormatFlags.RIGHT);


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
                    ZConsole.Write(_entity.Quantity.ToString(), (pageStartX + 53) + (colSize * 1), recStartLocY + i, (pageStartX + 53) + (colSize * 2), null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                    ZConsole.Write("PHP " + _entity.Price.ToString("N2"), (pageStartX + 53) + (colSize * 2), recStartLocY + i, (pageStartX + 53) + (colSize * 3), null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                    ZConsole.Write("PHP " + (_entity.Quantity * _entity.Price).ToString("N2"), (pageStartX + 3) + (colSize * 3), recStartLocY + i, pageEndX - 2, null, flag: ZConsole.ConsoleFormatFlags.RIGHT);

                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                command = Console.ReadKey();

            }


        }
        public void ViewEntityDetails()
        {
            if (selectedTransaction == null) throw new Exception("Please Select Record To View!");
            Entities.Transaction transaction = DBContext<Entities.Transaction>.GetById(selectedTransaction.Value);

            int pageStartX = Console.WindowWidth / 2 - 90;
            int pageEndX = Console.WindowWidth / 2 + 90;

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
                ZConsole.Write("ZINVSYS Transaction Management Menu", pageStartX, 1, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("[VIEW TRANSACTION DETAILS]", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
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
                int colSize = remainingSpaces / 4;

                ZConsole.Write("#", pageStartX + 42, 8, pageStartX + 46, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("BN", pageStartX + 48, 8, pageStartX + 53, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("Product Identifier", pageStartX + 53, 8, (pageStartX + 53) + (colSize * 1), null, flag: ZConsole.ConsoleFormatFlags.LEFT);
                ZConsole.Write("Quantity", (pageStartX + 53) + (colSize * 1), 8, (pageStartX + 53) + (colSize * 2), null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                ZConsole.Write("Price", (pageStartX + 53) + (colSize * 2), 8, (pageStartX + 53) + (colSize * 3), null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                ZConsole.Write("Total", (pageStartX + 53) + (colSize * 3), 8, pageEndX - 2, null, flag: ZConsole.ConsoleFormatFlags.RIGHT);


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
                    ZConsole.Write(_entity.Quantity.ToString(), (pageStartX + 53) + (colSize * 1), recStartLocY + i, (pageStartX + 53) + (colSize * 2), null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                    ZConsole.Write("PHP " + _entity.Price.ToString("N2"), (pageStartX + 53) + (colSize * 2), recStartLocY + i, (pageStartX + 53) + (colSize * 3), null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                    ZConsole.Write("PHP " + (_entity.Quantity * _entity.Price).ToString("N2"), (pageStartX + 53) + (colSize * 3), recStartLocY + i, pageEndX - 2, null, flag: ZConsole.ConsoleFormatFlags.RIGHT);

                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                command = Console.ReadKey();
            }
            selectedTransaction = null;

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
    }
}
