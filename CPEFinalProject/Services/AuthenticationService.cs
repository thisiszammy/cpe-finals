using CPEFinalProject.DAL;
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
    public class AuthenticationService : IAuthenticationService
    {

        public void Authenticate(ApplicationUserTypeEnum userType)
        {
            Console.Clear();


            int pageStartX = Console.WindowWidth / 2 - 20;
            int pageEndX = Console.WindowWidth / 2 + 20;

            ZConsole.DrawBox( pageStartX, pageEndX, 8, 15);
            ZConsole.Write("ZINVSYS Authentication Service", pageStartX, 7, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.Write("[AUTHENTICATE TO PROCEED]", pageStartX, 9, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);


            switch (userType)
            {
                case ApplicationUserTypeEnum.ADMIN:
                    ZConsole.Write("LOGGING IN AS ADMIN", pageStartX, 10, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    break;
                case ApplicationUserTypeEnum.CUSTOMER:
                    ZConsole.Write("LOGGING IN AS CUSTOMER", pageStartX, 10, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
                    break;
            }

            string username, password = string.Empty;

            ZConsole.Write("Username: ", pageStartX + 2, 12, null, null);
            username = Console.ReadLine();

            ZConsole.Write("Password: ", pageStartX + 2, 13, null, null);
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

            var _user = DBContext<ApplicationUser>.GetByCustomQuery(x => x.Username == username && !x.IsDeleted).FirstOrDefault();

            if (_user == null) throw new Exception("Invalid User Login!");
            if (!_user.AuthenticateUser(password)) throw new Exception("Invalid User Login!");

            if (_user.UserType != userType) throw new Exception("UnAuthorized Access!");

            Program.currentUser = _user;
        }
    }
}
