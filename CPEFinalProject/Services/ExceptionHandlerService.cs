using CPEFinalProject.Entities;
using CPEFinalProject.Entities.Enums;
using CPEFinalProject.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.Services
{
    public class ExceptionHandlerService : IExceptionHandlerService
    {

        public void Handle(Exception ex, ApplicationStateFlagsEnum applicationStateFlagsEnum, Type T)
        {
            Console.Clear(); 

            int pageStartX = Console.WindowWidth / 2 - 40;
            int pageEndX = Console.WindowWidth / 2 + 40;

            ZConsole.DrawBox(pageStartX, pageEndX, 2, 15);
            ZConsole.Write("Zinvsys Exception Service", pageStartX, 1, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.Write("An Error has occurred!", pageStartX, 3, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);

            string _action = string.Empty;
            string _entity = string.Empty;

            if (T == typeof(AdministrativeUser)) _entity = "Admin Users";
            else if (T == typeof(Customer)) _entity = "Customers";
            else if (T == typeof(ProductProfile)) _entity = "Product Profiles";
            else if (T == typeof(ProductStock)) _entity = "Product Stocks";
            else if (T == typeof(ProductStock)) _entity = "Unlisted Product Stocks";
            else if (T == typeof(Transaction)) _entity = "Transactions";
            else if (T == typeof(Transaction)) _entity = "Transaction Items";
            else if (T == typeof(Transaction)) _entity = "Refunded Items";

            switch (applicationStateFlagsEnum)
            {
                case ApplicationStateFlagsEnum.START:
                    _entity = string.Empty;
                    _action = "starting the application";
                    break;
                case ApplicationStateFlagsEnum.AUTHENTICATE:
                    _entity = string.Empty;
                    _action = "authenticating user";
                    break;
                case ApplicationStateFlagsEnum.MENU_GENERAL:
                    _action = "loading menu options ";
                    break;
                case ApplicationStateFlagsEnum.LIST:
                    _action = "loading list of ";
                    break;
                case ApplicationStateFlagsEnum.CREATE:
                    _action = "creating ";
                    break;
                case ApplicationStateFlagsEnum.EDIT:
                    _action = "editing ";
                    break;
                case ApplicationStateFlagsEnum.DELETE:
                    _action = "deleting ";
                    break;
            }

            ZConsole.Write("Error " + _action + _entity + ": " + ex.Message, pageStartX + 2, 5, pageEndX, 15, flag: ZConsole.ConsoleFormatFlags.TOP_LEFT);
            ZConsole.Write("Press any key to continue ...", pageStartX+2, 14, null, null, flag: ZConsole.ConsoleFormatFlags.LEFT);

            Console.ReadKey();
        }

    }
}
