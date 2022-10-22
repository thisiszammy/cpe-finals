using CPEFinalProject.Entities;
using CPEFinalProject.Services;
using CPEFinalProject.Services.Interfaces;
using CPEFinalProject.Services.Management;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject
{
    public class ApplicationWorker
    {
        Dictionary<Type, IManagementGUIService> services;
        readonly ICustomerGUIService customerGUIService;
        IManagementGUIService activeDrawingService;
        public ApplicationWorker(int consoleWidth, int consoleHeight)
        {
            ZConsole.Init(consoleWidth, consoleHeight);

            customerGUIService = new CustomerGUIService(Program.notificationService);

            var adminManagementService = new AdminManagementUIService(Program.notificationService);
            var customerManagementService = new CustomerManagementUIService();
            var productProfileManagementService = new ProductProfileManagementUIService();
            var productStockManagementService = new ProductStockManagementUIService(Program.notificationService, productProfileManagementService);
            var transactionManagementService = new TransactionManagementUIService(Program.notificationService, customerManagementService, Program.exceptionHandlerService, productStockManagementService);

            services = new Dictionary<Type, IManagementGUIService>()
            {
                { typeof(AdministrativeUser),  adminManagementService},
                { typeof(Customer), customerManagementService },
                { typeof(ProductProfile), productProfileManagementService},
                { typeof(ProductStock), productStockManagementService},
                { typeof(Transaction), transactionManagementService}
            };
        }

        public void SetActiveDrawingService(Type? T) => activeDrawingService = services.Where(x => x.Key == T).FirstOrDefault().Value;
        public void DrawMenu() => activeDrawingService.DrawMenu();
        public void DrawCreateEntity() => activeDrawingService.CreateEntity();
        public void DrawUpdateEntity() => activeDrawingService.UpdateEntity();
        public void DrawDeleteEntity() => activeDrawingService.DeleteEntity();
        public void DrawEntityTable() => activeDrawingService.ListEntity();
        public void DrawEntityReportSummary() => activeDrawingService.GenerateReportSummary();

        public void DrawCustomerTransactionHistory() => customerGUIService.ListOrderHistory();
        public void DrawCustomerTransactionDetails() => customerGUIService.ViewOrderDetails();
        public void DrawCustomerProfile() => customerGUIService.EditProfile();
    }
}
