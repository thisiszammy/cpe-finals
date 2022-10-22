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
    public class ProductProfileManagementUIService : IManagementGUIService
    {
        readonly INotificationService notificationService;
        readonly Type contextType;
        int? selectedProductProfile = null;

        public ProductProfileManagementUIService()
        {
            notificationService = Program.notificationService;
            contextType = typeof(ProductProfile);
        }

        public void CreateEntity()
        {
            Console.Clear();

            int pageStartX = Console.WindowWidth / 2 - 30;
            int pageEndX = Console.WindowWidth / 2 + 30;

            ZConsole.DrawBox(pageStartX, pageEndX, 2, 20);
            ZConsole.Write("ZINVSYS Product Profile Management Menu", pageStartX, 1, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.Write("[CREATE NEW PRODUCT PROFILE]", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);

            string productName, sku;
            decimal price;

            ZConsole.Write("Enter Product SKU: ", pageStartX + 2, 5, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            sku = Console.ReadLine();

            if (string.IsNullOrEmpty(sku)) throw new Exception("Please Enter Product SKU!");

            ZConsole.Write("Enter Product Name: ", pageStartX + 2, 6, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            productName = Console.ReadLine();

            if (string.IsNullOrEmpty(productName)) throw new Exception("Please Enter Product Name!");

            ZConsole.Write("Enter Price: ", pageStartX + 2, 7, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            var _price = Console.ReadLine();
            if (!decimal.TryParse(_price, out price)) throw new Exception("Please Enter A Valid Price!");

            if (DBContext<ProductProfile>.GetByCustomQuery(x => x.SKU == sku).Any()) throw new Exception("Product SKU Entered Is Already In-Use!");
            if (DBContext<ProductProfile>.GetByCustomQuery(x => x.ProductName == productName).Any()) throw new Exception("Product Name Entered Is Already In-Use!");

            var product = new ProductProfile(productName, price, sku);
            product.CreatedBy = Program.currentUser.Uid;

            DBContext<ProductProfile>.Add(product);

            notificationService.DrawMessageBox("Successfully Created Product Profile!");
        }

        public void DeleteEntity()
        {
            ListEntity();

            if (selectedProductProfile == null) return;

            Console.Clear();

            if (selectedProductProfile == null) throw new Exception("No Entity Selected!");
            var selectedProductProfileEntity = DBContext<ProductProfile>.GetById(selectedProductProfile.Value);

            ApplicationUser? createdBy = DBContext<ApplicationUser>.GetById(selectedProductProfileEntity.CreatedBy);
            ApplicationUser? lastModifiedBy = (selectedProductProfileEntity.LastModifiedBy == null) ? null : DBContext<ApplicationUser>.GetById(selectedProductProfileEntity.LastModifiedBy.Value);

            int pageStartX = Console.WindowWidth / 2 - 40;
            int pageEndX = Console.WindowWidth / 2 + 40;

            ZConsole.DrawBox(pageStartX, pageEndX, 2, 25);
            ZConsole.Write("ZINVSYS Product Profile Management", pageStartX, 1, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.Write("[DELETE PRODUCT PROFILE]", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.DrawClosedPipe(4, pageStartX, pageEndX);

            ZConsole.Write("UID: " + selectedProductProfileEntity.Uid, pageStartX + 2, 5, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("SKU: " + selectedProductProfileEntity.SKU, pageStartX + 2, 6, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Product Name: " + selectedProductProfileEntity.ProductName, pageStartX + 2, 7, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Price: PHP " + selectedProductProfileEntity.Price.ToString("N2"), pageStartX + 2, 8, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Stocks In Inventory: " + selectedProductProfileEntity.ProductStocks.Sum(x=>x.Quantity).ToString(), pageStartX + 2, 9, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Created On: " + selectedProductProfileEntity.CreatedOn.ToString("MM/dd/yyyy hh:mm tt"), pageStartX + 2, 10, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Created By: " + ((createdBy == null) ? "[unable to retrieve user]" : createdBy.Username), pageStartX + 2, 11, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Last Modified On: " + ((selectedProductProfileEntity.LastModifiedOn == null) ? "" : selectedProductProfileEntity.LastModifiedOn.ToString()), pageStartX + 2, 12, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Last Modified By: " + ((lastModifiedBy == null) ? "[unable to retrieve user]" : lastModifiedBy.Username), pageStartX + 2, 13, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);

            ZConsole.DrawClosedPipe(14, pageStartX, pageEndX);
            ZConsole.Write("Are you sure you want to delete this record?", pageStartX, 15, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.DrawClosedPipe(16, pageStartX, pageEndX);

            ZConsole.Write("Enter Selection (Y/N): ", pageStartX + 1, 17, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            var selection = Console.ReadKey();

            if (selection.Key == ConsoleKey.Y)
            {
                DBContext<ProductProfile>.Delete(selectedProductProfile.Value, Program.currentUser.Uid);
                notificationService.DrawMessageBox("Successfully Deleted Product Profile!");
            }

            selectedProductProfile = null;

            DeleteEntity();
        }

        public T DialogGetEntityFromList<T>()
        {
            ListEntity();
            if (selectedProductProfile == null) return default(T);
            var entity = DBContext<ProductProfile>.GetById(selectedProductProfile.Value);
            selectedProductProfile = null;
            return (T)(object)entity;
        }

        public void DrawMenu()
        {
            Program.DrawScrollerSelection("ZINVSYS Product Profile Management", new ScrollerOption[]
            {
            new ScrollerOption("List All Product Profiles", contextType, ApplicationStateFlagsEnum.LIST),
            new ScrollerOption("Create Product Profiles", contextType, ApplicationStateFlagsEnum.CREATE),
            new ScrollerOption("Edit Product Profiles", contextType, ApplicationStateFlagsEnum.EDIT),
            new ScrollerOption("Delete Product Profiles", contextType, ApplicationStateFlagsEnum.DELETE),
            new ScrollerOption("Back", null, ApplicationStateFlagsEnum.BACK),
            });
        }

        public void GenerateReportSummary()
        {
            throw new NotImplementedException();
        }

        public void ListEntity()
        {
            int pageStartX = Console.WindowWidth / 2 - 70;
            int pageEndX = Console.WindowWidth / 2 + 70;

            string additionalTitle = string.Empty;

            if (Program.navigationHistory.Peek() == ApplicationStateFlagsEnum.EDIT) additionalTitle = "(Select Record To Edit)";
            else if (Program.navigationHistory.Peek() == ApplicationStateFlagsEnum.DELETE) additionalTitle = "(Select Record To Delete)";

            ConsoleKeyInfo? command = null;

            int page = 0, rowPerPage = 18, scrollerIndex = 0, pageCount = 0;
            int maxScrollerIndex = 0, lastPageMaxScrollerIndex = 0;
            string? searchQuery = null;

            int columnWidth = pageEndX - pageStartX + 4;

            scrollerIndex = 0;

            List<ProductProfile> allrecords = DBContext<ProductProfile>.GetAllExisting();
            List<ProductProfile> records = new List<ProductProfile>(allrecords);


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
                        case ConsoleKey.Enter:
                            selectedProductProfile = records[scrollerIndex].Uid;
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
                    records = allrecords.Where(x => (x.ProductIdentifier.ToLower().Contains(searchQuery.ToLower()))).ToList();

                    ZConsole.Write($"Searching: \"{searchQuery}\"", pageStartX + 2, 3, (pageEndX / 2) - 5, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
                }

                decimal div = (decimal)records.Count() / rowPerPage;
                pageCount = int.Parse(Math.Ceiling(div).ToString());
                pageCount = (pageCount == 0) ? 1 : pageCount;

                lastPageMaxScrollerIndex = (records.Count() % rowPerPage) - 1;
                records = records.Skip(page * rowPerPage).Take(rowPerPage).ToList();

                maxScrollerIndex = records.Count;


                ZConsole.DrawBox(pageStartX, pageEndX, 2, 27);
                ZConsole.Write("ZINVSYS Product Profile Management", pageStartX, 1, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write($"[PRODUCT PROFILE TABLE] {additionalTitle}", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write($"(Page {page + 1}/{pageCount}) ", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.RIGHT);

                int recStartLocY = 7;

                ZConsole.DrawClosedPipe(4, pageStartX, pageEndX);

                int remainingSpaces = pageEndX - (pageStartX + 11);
                int colSize = remainingSpaces / 4;

                ZConsole.Write("#", pageStartX + 1, 5, pageStartX + 4, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("ID", pageStartX + 6, 5, pageStartX + 11, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("Product Identifier", pageStartX + 11, 5, (pageStartX + 11) + (colSize * 1), null, flag: ZConsole.ConsoleFormatFlags.LEFT);
                ZConsole.Write("Price", (pageStartX + 11) + (colSize * 1), 5, (pageStartX + 11) + (colSize * 2), null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                ZConsole.Write("Stocks", (pageStartX + 11) + (colSize * 2), 5, (pageStartX + 11) + (colSize * 3), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                ZConsole.Write("Created On", (pageStartX + 11) + (colSize * 3), 5, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);

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
                    ZConsole.Write(_entity.ProductIdentifier, pageStartX + 11, recStartLocY + i, (pageStartX + 11) + (colSize * 1), null, flag: ZConsole.ConsoleFormatFlags.LEFT);
                    ZConsole.Write("PHP " + _entity.Price.ToString("N2"), (pageStartX + 11) + (colSize * 1), recStartLocY + i, (pageStartX + 11) + (colSize * 2), null, flag: ZConsole.ConsoleFormatFlags.RIGHT);
                    ZConsole.Write(_entity.QtyInStock.ToString(), (pageStartX + 11) + (colSize * 2), recStartLocY + i, (pageStartX + 11) + (colSize * 3), null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    ZConsole.Write(_entity.CreatedOn.ToString("MM/dd/yyyy hh:mm tt"), (pageStartX + 11) + (colSize * 3), recStartLocY + i, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);

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


            if (selectedProductProfile == null) return;

            Console.Clear();

            if (selectedProductProfile == null) throw new Exception("No Entity Selected!");
            var selectedProductProfileEntity = DBContext<ProductProfile>.GetById(selectedProductProfile.Value);

            ApplicationUser? createdBy = DBContext<ApplicationUser>.GetById(selectedProductProfileEntity.CreatedBy);
            ApplicationUser? lastModifiedBy = (selectedProductProfileEntity.LastModifiedBy == null) ? null : DBContext<ApplicationUser>.GetById(selectedProductProfileEntity.LastModifiedBy.Value);

            int pageStartX = Console.WindowWidth / 2 - 40;
            int pageEndX = Console.WindowWidth / 2 + 40;

            ZConsole.DrawBox(pageStartX, pageEndX, 2, 25);
            ZConsole.Write("ZINVSYS Product Profile Management", pageStartX, 1, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.Write("[EDIT PRODUCT PROFILE DETAILS]", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.DrawClosedPipe(4, pageStartX, pageEndX);

            ZConsole.Write("UID: " + selectedProductProfileEntity.Uid, pageStartX + 2, 5, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("SKU: " + selectedProductProfileEntity.SKU, pageStartX + 2, 6, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Product Name: " + selectedProductProfileEntity.ProductName, pageStartX + 2, 7, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Price: PHP " + selectedProductProfileEntity.Price.ToString("N2"), pageStartX + 2, 8, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Stocks In Inventory: " + selectedProductProfileEntity.ProductStocks.Sum(x=>x.Quantity).ToString(), pageStartX + 2, 9, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Created On: " + selectedProductProfileEntity.CreatedOn.ToString("MM/dd/yyyy hh:mm tt"), pageStartX + 2, 10, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Created By: " + ((createdBy == null) ? "[unable to retrieve user]" : createdBy.Username), pageStartX + 2, 11, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Last Modified On: " + ((selectedProductProfileEntity.LastModifiedOn == null) ? "" : selectedProductProfileEntity.LastModifiedOn.ToString()), pageStartX + 2, 12, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Last Modified By: " + ((lastModifiedBy == null) ? "[unable to retrieve user]" : lastModifiedBy.Username), pageStartX + 2, 13, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);

            ZConsole.DrawClosedPipe(14, pageStartX, pageEndX);
            ZConsole.Write("Enter New Details", pageStartX, 15, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.DrawClosedPipe(16, pageStartX, pageEndX);

            string productName, sku;
            decimal price;

            ZConsole.Write("Enter Product SKU: ", pageStartX + 2, 17, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            sku = Console.ReadLine();

            if (string.IsNullOrEmpty(sku)) throw new Exception("Please Enter Product SKU!");

            ZConsole.Write("Enter Product Name: ", pageStartX + 2, 18, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            productName = Console.ReadLine();

            if (string.IsNullOrEmpty(productName)) throw new Exception("Please Enter Product Name!");

            ZConsole.Write("Enter Price: ", pageStartX + 2, 19, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            var _price = Console.ReadLine();
            if (!decimal.TryParse(_price, out price)) throw new Exception("Please Enter A Valid Price!");

            if (DBContext<ProductProfile>.GetByCustomQuery(x => x.SKU == sku && x.Uid != selectedProductProfile).Any()) throw new Exception("Product SKU Entered Is Already In-Use!");
            if (DBContext<ProductProfile>.GetByCustomQuery(x => x.ProductName == productName && x.Uid != selectedProductProfile).Any()) throw new Exception("Product Name Entered Is Already In-Use!");

            selectedProductProfileEntity.SKU = sku;
            selectedProductProfileEntity.ProductName = productName;
            selectedProductProfileEntity.Price = price;
            selectedProductProfileEntity.LastModifiedBy = Program.currentUser.Uid;

            DBContext<ProductProfile>.Update(selectedProductProfileEntity);

            notificationService.DrawMessageBox("Successfully Edited Product Profile!");
          
            selectedProductProfile = null;

            UpdateEntity();
        }

        public void ViewEntityDetails()
        {
            Console.Clear();

            if (selectedProductProfile == null) throw new Exception("No Entity Selected!");
            var selectedProductProfileEntity = DBContext<ProductProfile>.GetById(selectedProductProfile.Value);

            ApplicationUser? createdBy = DBContext<ApplicationUser>.GetById(selectedProductProfileEntity.CreatedBy);
            ApplicationUser? lastModifiedBy = (selectedProductProfileEntity.LastModifiedBy == null) ? null : DBContext<ApplicationUser>.GetById(selectedProductProfileEntity.LastModifiedBy.Value);

            int pageStartX = Console.WindowWidth / 2 - 30;
            int pageEndX = Console.WindowWidth / 2 + 30;

            ZConsole.DrawBox(pageStartX, pageEndX, 2, 22);
            ZConsole.Write("ZINVSYS Product Profile Management", pageStartX, 1, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.Write("[VIEW SELECTED PRODUCT PROFILE DETAILS]", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);

            ZConsole.Write("UID: " + selectedProductProfileEntity.Uid, pageStartX + 2, 5, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("SKU: " + selectedProductProfileEntity.SKU, pageStartX + 2, 6, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Product Name: " + selectedProductProfileEntity.ProductName, pageStartX + 2, 7, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Price: PHP " + selectedProductProfileEntity.Price.ToString("N2"), pageStartX + 2, 8, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Stocks In Inventory: " + selectedProductProfileEntity.ProductStocks.Sum(x => x.Quantity).ToString(), pageStartX + 2, 9, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Created On: " + selectedProductProfileEntity.CreatedOn.ToString("MM/dd/yyyy hh:mm tt"), pageStartX + 2, 11, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Created By: " + ((createdBy == null) ? "[unable to retrieve user]" : createdBy.Username), pageStartX + 2, 12, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Last Modified On: " + ((selectedProductProfileEntity.LastModifiedOn == null) ? "" : selectedProductProfileEntity.LastModifiedOn.ToString()), pageStartX + 2, 14, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            ZConsole.Write("Last Modified By: " + ((lastModifiedBy == null) ? "[unable to retrieve user]" : lastModifiedBy.Username), pageStartX + 2, 15, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);


            ZConsole.Write("Press Any Key To Go Back...", pageStartX + 1, 21, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            Console.ReadKey();

            selectedProductProfile = null;
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
