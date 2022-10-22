using CPEFinalProject.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.Services
{
    public class NotificationService : INotificationService
    {
        public void DrawMessageBox(string message)
        {
            Console.Clear();


            int pageStartX = Console.WindowWidth / 2 - 30;
            int pageEndX = Console.WindowWidth / 2 + 30;

            ZConsole.DrawBox(pageStartX, pageEndX, 8, 20);
            ZConsole.Write("Zinvsys Notification Service", pageStartX, 7, pageEndX, null, flag: ZConsole.ConsoleFormatFlags.CENTER);
            ZConsole.Write($"[{message.ToUpper()}]", pageStartX, 9, pageEndX, 20, flag: ZConsole.ConsoleFormatFlags.MIDDLE_CENTER);

            ZConsole.Write("Press any key to continue ...", pageStartX + 2, 19, null, null, flag: ZConsole.ConsoleFormatFlags.LEFT);
            Console.ReadKey();
        }
    }
}
