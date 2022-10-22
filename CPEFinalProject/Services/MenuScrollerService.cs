using CPEFinalProject.Entities.Enums;
using CPEFinalProject.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.Services
{
    public class MenuScrollerService
    {
        private int SelectedIndex;
        private ScrollerOption[] Options;
        private string Title;

        public MenuScrollerService(string title, ScrollerOption[] options)
        {
            Title = title;
            Options = options;
            SelectedIndex = 0;
        }

        private void DisplayOptions()
        {
            ZConsole.Init(Console.WindowWidth, Console.WindowHeight);

            ZConsole.DrawBox(0, Console.WindowWidth - 1, 0, 2);
            ZConsole.Write(Title, 0, 0, null, null, flag: ZConsole.ConsoleFormatFlags.TOP_CENTER, yOffset: 1);

            ZConsole.DrawBox(0, Console.WindowWidth - 1, 3, 6);
            ZConsole.Write("Navigate through the options by pressing the ARROW Keys or WASD Keys.", 0, 0, null, null, flag: ZConsole.ConsoleFormatFlags.TOP_LEFT, yOffset: 4, xOffset: 2);
            ZConsole.Write("Press ENTER to select an option.", 0, 0, null, null, flag: ZConsole.ConsoleFormatFlags.TOP_LEFT, yOffset: 5, xOffset: 2);

            ZConsole.DrawBox(0, Console.WindowWidth - 1, 7, Console.WindowHeight - 2);

            for (int i = 0; i < Options.Length; i++)
            {
                string currentOption = Options[i].Label;
                string prefix;

                if (i == SelectedIndex)
                {
                    prefix = ">";
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                }
                else
                {
                    prefix = " ";
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                }

                ZConsole.Write($"{prefix} [{currentOption}]", 0, 0, null, null, flag: ZConsole.ConsoleFormatFlags.TOP_LEFT, yOffset: 9 + i, xOffset: 2);
            }
            Console.ResetColor();
        }

        public ScrollerOption Run()
        {
            ConsoleKey keyPressed;
            Console.Clear();
            do
            {
                Console.CursorVisible = false;
                DisplayOptions();

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                keyPressed = keyInfo.Key;

                //Update SelectedIndex based on ARROW keys or WASD keys.
                if (keyPressed == ConsoleKey.UpArrow || keyPressed == ConsoleKey.W)
                {
                    SelectedIndex--;
                    if (SelectedIndex == -1)
                    {
                        SelectedIndex = Options.Length - 1;
                    }
                }
                else if (keyPressed == ConsoleKey.DownArrow || keyPressed == ConsoleKey.S)
                {
                    SelectedIndex++;
                    if (SelectedIndex == Options.Length)
                    {
                        SelectedIndex = 0;
                    }
                }
            } while (keyPressed != ConsoleKey.Enter);


            return Options[SelectedIndex];
        }
    }
}
