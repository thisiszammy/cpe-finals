using CPEFinalProject;
using CPEFinalProject.DAL;
using CPEFinalProject.Entities;
using CPEFinalProject.Entities.Enums;
using CPEFinalProject.Entities.ViewModels;
using CPEFinalProject.Services;
using CPEFinalProject.Services.Interfaces;
using Spectre.Console;


namespace CPEFinalProject
{
    public static class Program
    {

        public static ApplicationTypeEnum currentApplicationType;
        public static ApplicationUser currentUser;
        public static Stack<ApplicationStateFlagsEnum> navigationHistory;
        public static IAuthenticationService authenticationService;
        public static IExceptionHandlerService exceptionHandlerService;
        public static INotificationService notificationService;
        static ApplicationWorker applicationWorker;

        static void Main()
        {
            Console.BufferHeight = 45;
            Console.BufferWidth = 200;



            Console.WindowHeight = 50;
            Console.WindowWidth = 210;

            Database.Init(new Type[]
            {
                typeof(ApplicationUser),
                typeof(ProductProfile),
                typeof(Transaction)
            });



            authenticationService = new AuthenticationService();
            exceptionHandlerService = new ExceptionHandlerService();
            navigationHistory = new Stack<ApplicationStateFlagsEnum>();
            notificationService = new NotificationService();

            applicationWorker = new ApplicationWorker(Console.WindowWidth, Console.WindowHeight);


            Run();
        }


        static void Run()
        {
            NextPage(ApplicationStateFlagsEnum.START, null, false);
        }


        public static async void NextPage(ApplicationStateFlagsEnum destination, Type? T, bool isPrev)
        {
            applicationWorker.SetActiveDrawingService(T);
            try
            {
                if (!isPrev && destination != ApplicationStateFlagsEnum.BACK) navigationHistory.Push(destination);

                switch (destination)
                {
                    case ApplicationStateFlagsEnum.START:
                        DrawStartMenu();
                        break;
                    case ApplicationStateFlagsEnum.MENU_GENERAL:
                        applicationWorker.DrawMenu();
                        break;
                    case ApplicationStateFlagsEnum.MENU_CUSTOMER:
                        DrawCustomerMenu();
                        break;
                    case ApplicationStateFlagsEnum.MENU_MANAGEMENT:
                        DrawManagementMenu();
                        break;
                    case ApplicationStateFlagsEnum.AUTHENTICATE:
                        var authenticationProcedure = (T == typeof(Customer)) ? ApplicationUserTypeEnum.CUSTOMER : ApplicationUserTypeEnum.ADMIN;
                        authenticationService.Authenticate(authenticationProcedure);

                        navigationHistory.Pop();

                        if(authenticationProcedure == ApplicationUserTypeEnum.CUSTOMER) NextPage(ApplicationStateFlagsEnum.MENU_CUSTOMER, null, false);
                        else NextPage(ApplicationStateFlagsEnum.MENU_MANAGEMENT, null, false);
                        break;
                    case ApplicationStateFlagsEnum.CREATE:
                        applicationWorker.DrawCreateEntity();
                        NextPage(ApplicationStateFlagsEnum.BACK, T, false);
                        break;
                    case ApplicationStateFlagsEnum.EDIT:
                        applicationWorker.DrawUpdateEntity();
                        NextPage(ApplicationStateFlagsEnum.BACK, T, false);
                        break;
                    case ApplicationStateFlagsEnum.DELETE:
                        applicationWorker.DrawDeleteEntity();
                        NextPage(ApplicationStateFlagsEnum.BACK, T, false);
                        break;
                    case ApplicationStateFlagsEnum.LIST:
                        applicationWorker.DrawEntityTable();
                        NextPage(ApplicationStateFlagsEnum.BACK, T, false);
                        break;
                    case ApplicationStateFlagsEnum.EXIT:
                        Exit();
                        return;
                    case ApplicationStateFlagsEnum.BACK:
                        navigationHistory.Pop();
                        var next = navigationHistory.Peek();
                        NextPage(next, T, true);
                        break;
                    case ApplicationStateFlagsEnum.REPORTS:
                        applicationWorker.DrawEntityReportSummary();
                        NextPage(ApplicationStateFlagsEnum.BACK, T, false);
                        break;
                    case ApplicationStateFlagsEnum.CUST_ORDER_HIST:
                        applicationWorker.DrawCustomerTransactionHistory();
                        NextPage(ApplicationStateFlagsEnum.BACK, T, false);
                        break;
                    case ApplicationStateFlagsEnum.CUST_PROFILE:
                        applicationWorker.DrawCustomerProfile();
                        NextPage(ApplicationStateFlagsEnum.BACK, T, false);
                        break;
                    case ApplicationStateFlagsEnum.CUST_ORDER_DET:
                        applicationWorker.DrawCustomerTransactionDetails();
                        NextPage(ApplicationStateFlagsEnum.BACK, T, false);
                        break;

                }

            }
            catch(Exception ex)
            {
                exceptionHandlerService.Handle(ex, destination, T);
                NextPage(ApplicationStateFlagsEnum.BACK, T, false);
            }

            
        }

        static void DrawStartMenu()
        {
            ZConsole.DrawBox(0, 20, 0, 2);
            ZConsole.Write("ZINVSYS", 0, 1, 20, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            Console.SetCursorPosition(0, 20);

            DrawScrollerSelection("ZINVSYS", new ScrollerOption[]
            {
                new ScrollerOption("Management Module", typeof(AdministrativeUser), ApplicationStateFlagsEnum.AUTHENTICATE),
                new ScrollerOption("Customer Module", typeof(Customer), ApplicationStateFlagsEnum.AUTHENTICATE),
                new ScrollerOption("Exit App", null, ApplicationStateFlagsEnum.EXIT)
            });
        }

        static void DrawManagementMenu()
        {
            currentApplicationType = ApplicationTypeEnum.MANAGEMENT;

            ZConsole.DrawBox(0, 20, 0, 2);
            ZConsole.Write("ZINVSYS MANAGER", 0, 1, 20, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            Console.SetCursorPosition(0, 20);

            DrawScrollerSelection("ZINVSYS ADMIN MENU", new ScrollerOption[]
            {
                new ScrollerOption("Manage Admins", typeof(AdministrativeUser), ApplicationStateFlagsEnum.MENU_GENERAL),
                new ScrollerOption("Manage Customers", typeof(Customer), ApplicationStateFlagsEnum.MENU_GENERAL),
                new ScrollerOption("Manage Product Profiles", typeof(ProductProfile), ApplicationStateFlagsEnum.MENU_GENERAL),
                new ScrollerOption("Manage Product Stocks", typeof(ProductStock), ApplicationStateFlagsEnum.MENU_GENERAL),
                new ScrollerOption("Manage Transactions", typeof(Transaction), ApplicationStateFlagsEnum.MENU_GENERAL),
                new ScrollerOption("Sign Out", null, ApplicationStateFlagsEnum.BACK),
            });
        }

        static void DrawCustomerMenu()
        {
            currentApplicationType = ApplicationTypeEnum.CUSTOMER;

            ZConsole.DrawBox(0, 20, 0, 2);
            ZConsole.Write("ZINVSYS CUSTOMER", 0, 1, 20, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            Console.SetCursorPosition(0, 20);

            DrawScrollerSelection("ZINVSYS CUSTOMER MENU", new ScrollerOption[]
            {
                new ScrollerOption("Transaction History", null, ApplicationStateFlagsEnum.CUST_ORDER_HIST),
                new ScrollerOption("Edit Profile", null, ApplicationStateFlagsEnum.CUST_PROFILE),
                new ScrollerOption("Sign Out", null, ApplicationStateFlagsEnum.BACK)
            });
        }


        static void Exit()
        {
            Console.Clear();
            Console.WriteLine("Thank you for using this product");
        }

        public static void DrawScrollerSelection(string title, ScrollerOption[] options)
        {
            var scroller = new MenuScrollerService(title, options);
            var scrollerSelection = scroller.Run();
            NextPage(scrollerSelection.DestinationStateFlagEnum, scrollerSelection.DestinationType, scrollerSelection.DestinationStateFlagEnum == ApplicationStateFlagsEnum.BACK);
        }

    }
}





