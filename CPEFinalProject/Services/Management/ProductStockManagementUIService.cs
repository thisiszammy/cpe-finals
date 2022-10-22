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
    public class ProductStockManagementUIService : IManagementGUIService
    {
        readonly INotificationService notificationService;
        readonly IManagementGUIService managementGUIService;
        readonly Type contextType;
        int? selectedProduct = null;
        int? selectedProductStock = null;
        bool showListedStocks = true;

        public ProductStockManagementUIService(INotificationService notificationService,
            IManagementGUIService managementGUIService)
        {
            this.notificationService = notificationService;
            contextType = typeof(ProductStock);
            this.managementGUIService = managementGUIService;
        }

        public void CreateEntity()
        {
            ProductProfile profile = managementGUIService.DialogGetEntityFromList<ProductProfile>();

            if (profile == null) return;


            Console.Clear();

            int pageStartX = Console.WindowWidth / 2 - 30;
            int pageEndX = Console.WindowWidth / 2 + 30;

            ZConsole.DrawBox(pageStartX, pageEndX, 2, 20);
            ZConsole.Write("ZINVSYS Product Stock Management Menu", pageStartX, 1, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.Write("[ADD NEW PRODUCT STOCK]", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);

            int quantity;
            DateTime MfgDate, ExpDate;

            ZConsole.Write($"Product: {profile.ProductIdentifier}", pageStartX + 2, 5, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            
            ZConsole.Write("Enter Quantity: ", pageStartX + 2, 6, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            var _quantity = Console.ReadLine();
            if (!int.TryParse(_quantity, out quantity)) throw new Exception("Please Enter A Valid Quantity!");

            ZConsole.Write("Enter Manufacturing Date: ", pageStartX + 2, 7, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            var _mfgDate = Console.ReadLine();
            if (!DateTime.TryParse(_mfgDate, out MfgDate)) throw new Exception("Please Enter A Valid Manufacturing Date!");

            ZConsole.Write("Enter Expiry Date: ", pageStartX + 2, 8, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            var _expDate = Console.ReadLine();
            if (!DateTime.TryParse(_expDate, out ExpDate)) throw new Exception("Please Enter A Valid Expiry Date!");

            if (ExpDate < MfgDate) throw new Exception("Expiration Date Must Be Greater Than Manufacturing Date!");

            profile.AddStock(quantity, MfgDate, ExpDate, Program.currentUser.Uid);
            DBContext<ProductProfile>.Update(profile);

            notificationService.DrawMessageBox($"Successfully Stocked Product \"{profile.ProductIdentifier}\" (x{quantity})");

        }

        public void DeleteEntity()
        {
            Console.Clear();

            ListEntity();

            if (selectedProductStock == null || selectedProduct == null) return;

            var selectedProductEntity = DBContext<ProductProfile>.GetById(selectedProduct.Value);

            Console.Clear();

            var product = DBContext<ProductProfile>.GetById(selectedProduct.Value);
            var selectedProductStockEntity = product.ProductStocks.Where(x => x.Batch == selectedProductStock).FirstOrDefault();

            ApplicationUser? receivedBy = DBContext<ApplicationUser>.GetById(selectedProductStockEntity.ReceivedBy ?? -1);

            int pageStartX = Console.WindowWidth / 2 - 30;
            int pageEndX = Console.WindowWidth / 2 + 30;

            ZConsole.DrawBox(pageStartX, pageEndX, 2, 22);
            ZConsole.Write("ZINVSYS Product Stocks Management", pageStartX, 1, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.Write("[REMOVE PRODUCT STOCK]", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);

            ZConsole.Write("SKU: " + product.SKU, pageStartX + 2, 5, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Product Name: " + product.ProductName, pageStartX + 2, 6, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Batch: " + selectedProductStockEntity.Batch, pageStartX + 2, 7, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Quantity: " + selectedProductStockEntity.Quantity, pageStartX + 2, 8, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Mfg Date: " + selectedProductStockEntity.MfgDate.ToString("MM/dd/yyyy"), pageStartX + 2, 9, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Expiry Date: " + selectedProductStockEntity.ExpDate.ToString("MM/dd/yyyy hh:mm tt"), pageStartX + 2, 10, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Received On: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"), pageStartX + 2, 11, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Received By: " + ((receivedBy == null) ? "[unable to retrieve user]" : receivedBy.Username), pageStartX + 2, 12, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);

            ZConsole.DrawClosedPipe(13, pageStartX, pageEndX);
            ZConsole.Write("Enter Quantity To Remove", pageStartX, 14, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.DrawClosedPipe(15, pageStartX, pageEndX);

            int quantity;
            string remarks;

            ZConsole.Write("Quantity: ", pageStartX + 2, 16, pageEndX, null);
            var _quantity = Console.ReadLine();

            if (!int.TryParse(_quantity, out quantity)) throw new Exception("Please Enter A Valid Quantity To Remove!");
            if (quantity > selectedProductStockEntity.Quantity) throw new Exception("Please Enter A Valid Quantity! (Value entered was larger than value in stock)");

            ZConsole.Write("Removal Reason/Remarks: ", pageStartX + 2, 17, pageEndX, null);
            remarks = Console.ReadLine();

            if (string.IsNullOrEmpty(remarks)) throw new Exception("Please Enter Removal Reason/Remarks");

            ZConsole.Write("Are you sure you want to proceed? (Y/N): ", pageStartX + 1, 18, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            var selection = Console.ReadKey();

            if (selection.Key == ConsoleKey.Y)
            {
                selectedProductEntity.UnlistStock(quantity, selectedProductStockEntity.MfgDate, selectedProductStockEntity.ExpDate, selectedProductStockEntity.Batch, remarks, Program.currentUser.Uid, selectedProductStockEntity.ReceivedBy);
                DBContext<ProductProfile>.Update(selectedProductEntity);
                notificationService.DrawMessageBox($"Successfully Removed Stock \"{selectedProductEntity.ProductIdentifier}\" (x{quantity})");
            }

            selectedProduct = null;
            selectedProductStock = null; 
            DeleteEntity();
        }

        public T DialogGetEntityFromList<T>()
        {
            ListEntity();
            if (selectedProduct == null) return default(T);
            var entity = DBContext<ProductProfile>.GetById(selectedProduct.Value);
            selectedProduct = null;
            int temp = selectedProductStock.Value;
            selectedProductStock = null;
            return (T)(object)entity.ProductStocks.Where(x=>x.Batch == temp).FirstOrDefault();
        }

        public void DrawMenu()
        {
            Program.DrawScrollerSelection("ZINVSYS Product Stock Management", new ScrollerOption[]
            {
                new ScrollerOption("List All Stocks", contextType, ApplicationStateFlagsEnum.LIST),
                new ScrollerOption("Add New Stock", contextType, ApplicationStateFlagsEnum.CREATE),
                new ScrollerOption("Remove Stock", contextType, ApplicationStateFlagsEnum.DELETE),
                new ScrollerOption("Back", null, ApplicationStateFlagsEnum.BACK),
            });
        }

        public void GenerateReportSummary()
        {
            throw new NotImplementedException();
        }

        public void ListEntity()
        {
            showListedStocks = true;
            int pageStartX = Console.WindowWidth / 2 - 85;
            int pageEndX = Console.WindowWidth / 2 + 85;

            string additionalTitle = string.Empty;

            if (Program.navigationHistory.Peek() == ApplicationStateFlagsEnum.DELETE) additionalTitle = "(Select Stock To Remove)";
            

            ConsoleKeyInfo? command = null;

            int page = 0, rowPerPage = 18, scrollerIndex = 0, pageCount = 0;
            int maxScrollerIndex = 0, lastPageMaxScrollerIndex = 0;
            string? searchQuery = null;

            int columnWidth = pageEndX - pageStartX + 4;

            scrollerIndex = 0;

            List<ProductStocksViewModel> allRecords = null;
            List<ProductStocksViewModel> records = null;

            var allProducts = DBContext<ProductProfile>.GetAllExisting();
            var productListedStocks = allProducts.SelectMany(x => x.ProductStocks).Where(x => x.Quantity > 0);
            var productUnlistedStocks = allProducts.SelectMany(x => x.UnlistedProductStock).Where(x => x.Quantity > 0);

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
                            if (Program.navigationHistory.Peek() == ApplicationStateFlagsEnum.DELETE)
                            {
                                showListedStocks = true;
                            }
                            else
                            {
                                showListedStocks = !showListedStocks;
                            }
                            scrollerIndex = 0;
                            page = 0;
                            break;
                        case ConsoleKey.Enter:
                            selectedProductStock = records[scrollerIndex].Batch;
                            selectedProduct = records[scrollerIndex].ProductId;
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

                if (showListedStocks)
                {
                    allRecords = productListedStocks.Select(x => new ProductStocksViewModel
                    {
                        ProductId = x.ProductId,
                        Batch = x.Batch,
                        ExpiryDate = x.ExpDate,
                        MfgDate = x.MfgDate,
                        Quantity = x.Quantity,
                        ProductName = DBContext<ProductProfile>.GetById(x.ProductId).ProductIdentifier,
                        ReceivedOn = x.ReceivedOn
                    }).ToList();

                }
                else
                {
                    allRecords = productUnlistedStocks.Select(x => new ProductStocksViewModel
                    {
                        ProductId = x.ProductId,
                        Batch = x.Batch,
                        ExpiryDate = x.ExpDate,
                        MfgDate = x.MfgDate,
                        Quantity = x.Quantity,
                        ProductName = DBContext<ProductProfile>.GetById(x.ProductId).ProductIdentifier,
                        ReceivedOn = x.ReceivedOn,
                        RemovedOn = x.UnlistedOn
                    }).ToList();

                }

                if (string.IsNullOrEmpty(searchQuery))
                {
                    records = allRecords.ToList();
                }
                else
                {
                    records = allRecords.Where(x => (x.ProductName.ToLower().Contains(searchQuery.ToLower()))).ToList();

                    ZConsole.Write($"Searching: \"{searchQuery}\"", pageStartX + 2, 3, (pageEndX / 2) - 5, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
                }

                records.Reverse();

                decimal div = (decimal)records.Count() / rowPerPage;
                pageCount = int.Parse(Math.Ceiling(div).ToString());
                pageCount = (pageCount == 0) ? 1 : pageCount;

                lastPageMaxScrollerIndex = (records.Count() % rowPerPage) - 1;
                records = records.Skip(page * rowPerPage).Take(rowPerPage).ToList();

                maxScrollerIndex = records.Count;


                if (showListedStocks) additionalTitle = "(Showing Listed/Available Stocks)";
                else if (!showListedStocks) additionalTitle = "(Showing Unlisted/Removed Stocks)";

                if (Program.navigationHistory.Peek() == ApplicationStateFlagsEnum.DELETE) additionalTitle += " (Select Stock To Remove)";

                ZConsole.DrawBox(pageStartX, pageEndX, 2, 27);
                ZConsole.Write("ZINVSYS Product Stock Management", pageStartX, 1, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write($"[PRODUCT STOCKS TABLE] {additionalTitle}", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write($"(Page {page + 1}/{pageCount}) ", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.RIGHT);

                int recStartLocY = 7;

                ZConsole.DrawClosedPipe(4, pageStartX, pageEndX);

                int remainingSpaces = pageEndX - (pageStartX + 11);
                int colSize;

                if(showListedStocks) colSize = remainingSpaces / 5;
                else colSize = remainingSpaces / 6;

                ZConsole.Write("#", pageStartX + 1, 5, pageStartX + 4, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("BN", pageStartX + 6, 5, pageStartX + 11, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("Product", pageStartX + 11, 5, (pageStartX + 11) + (colSize * 1), null, flag: ZConsole.ConsoleFormatFlags.LEFT);
                ZConsole.Write("Quantity", (pageStartX + 11) + (colSize * 1), 5, (pageStartX + 11) + (colSize * 2), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("Mfg Date", (pageStartX + 11) + (colSize * 2), 5, (pageStartX + 11) + (colSize * 3), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("Exp Date", (pageStartX + 11) + (colSize * 3), 5, (pageStartX + 11) + (colSize * 4), null, flag: ZConsole.ConsoleFormatFlags.CENTER);

                if (showListedStocks)
                {
                    ZConsole.Write("Received Date", (pageStartX + 11) + (colSize * 4), 5, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                }
                else
                {
                    ZConsole.Write("Received Date", (pageStartX + 11) + (colSize * 4), 5, (pageStartX + 11) + (colSize * 5), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    ZConsole.Write("Removed Date", (pageStartX + 11) + (colSize * 5), 5, pageEndX+1, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                }

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

                    ZConsole.Write((i + 1).ToString(), pageStartX + 1, recStartLocY + i, pageStartX + 4, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    ZConsole.Write(_entity.Batch.ToString(), pageStartX + 6, recStartLocY + i, pageStartX + 11, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    ZConsole.Write(_entity.ProductName, pageStartX + 11, recStartLocY + i, (pageStartX + 11) + (colSize * 1), null, flag: ZConsole.ConsoleFormatFlags.LEFT);
                    ZConsole.Write(_entity.Quantity.ToString(), (pageStartX + 11) + (colSize * 1), recStartLocY + i, (pageStartX + 11) + (colSize * 2), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    ZConsole.Write(_entity.MfgDate.ToString("MM/dd/yyyy"), (pageStartX + 11) + (colSize * 2), recStartLocY + i, (pageStartX + 11) + (colSize * 3), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    ZConsole.Write(_entity.ExpiryDate.ToString("MM/dd/yyyy"), (pageStartX + 11) + (colSize * 3), recStartLocY + i, (pageStartX + 11) + (colSize * 4), null, flag: ZConsole.ConsoleFormatFlags.CENTER);

                    if (showListedStocks)
                    {
                        ZConsole.Write(_entity.ReceivedOn.ToString("MM/dd/yyyy hh:mm tt"), (pageStartX + 11) + (colSize * 4), recStartLocY + i, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    }
                    else
                    {
                        ZConsole.Write(_entity.ReceivedOn.ToString("MM/dd/yyyy HH:mm"), (pageStartX + 11) + (colSize * 4), recStartLocY + i, (pageStartX + 11) + (colSize * 5), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                        ZConsole.Write(_entity.RemovedOn.Value.ToString("MM/dd/yyyy HH:mm"), (pageStartX + 11) + (colSize * 5), recStartLocY + i, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    }

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
            throw new NotImplementedException();
        }
        public void ViewEntityDetails()
        {
            Console.Clear();

            if (selectedProductStock == null || selectedProduct == null) throw new Exception("No Entity Selected!");

            var product = DBContext<ProductProfile>.GetById(selectedProduct.Value);
            ProductStock selectedProductStockEntity;

            if (showListedStocks)
            {
                selectedProductStockEntity = product.ProductStocks.Where(x => x.Batch == selectedProductStock).FirstOrDefault();
            }
            else
            {
                selectedProductStockEntity = product.UnlistedProductStock.Where(x => x.Batch == selectedProductStock).FirstOrDefault();
            }

            string mode = (showListedStocks) ? "LISTED" : "REMOVED"; 

            ApplicationUser? receivedBy = DBContext<ApplicationUser>.GetById(selectedProductStockEntity.ReceivedBy ?? -1);

            int pageStartX = Console.WindowWidth / 2 - 30;
            int pageEndX = Console.WindowWidth / 2 + 30;

            ZConsole.DrawBox(pageStartX, pageEndX, 2, 22);
            ZConsole.Write("ZINVSYS Product Stocks Management", pageStartX, 1, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.Write($"[VIEW {mode} PRODUCT STOCK DETAILS]", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.Write("SKU: " + product.SKU, pageStartX + 2, 5, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Product Name: " + product.ProductName, pageStartX + 2, 6, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Batch: " + selectedProductStockEntity.Batch, pageStartX + 2, 7, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Quantity: " + selectedProductStockEntity.Quantity, pageStartX + 2, 8, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Mfg Date: " + selectedProductStockEntity.MfgDate.ToString("MM/dd/yyyy"), pageStartX + 2, 9, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Expiry Date: " + selectedProductStockEntity.ExpDate.ToString("MM/dd/yyyy hh:mm tt"), pageStartX + 2, 10, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Received On: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"), pageStartX + 2, 12, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Received By: " + ((receivedBy == null) ? "[unable to retrieve user]" : receivedBy.Username), pageStartX + 2, 13, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);

            if (!showListedStocks)
            {
                var unlistedProductStock = (UnlistedProductStock)selectedProductStockEntity;
                ApplicationUser? removedBy = DBContext<ApplicationUser>.GetById(unlistedProductStock.RemovedBy.Value);

                ZConsole.Write("Removed On: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"), pageStartX + 2, 14, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
                ZConsole.Write("Removed By: " + removedBy.Username, pageStartX + 2, 15, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
                ZConsole.Write("Removal Reason/Remarks: ", pageStartX + 2, 17, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
                ZConsole.Write(unlistedProductStock.Remarks, pageStartX + 4, 18, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            }

            ZConsole.Write("Press Any Key To Go Back...", pageStartX + 1, 21, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            Console.ReadKey();

            selectedProduct = null;
            selectedProductStock = null;
            showListedStocks = true;
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
