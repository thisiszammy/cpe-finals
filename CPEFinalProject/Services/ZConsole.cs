using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.Services
{
    internal static class ZConsole
    {
        private static int MAX_WIDTH;
        private static int MAX_HEIGHT;

        public static void Init(int max_width, int max_height)
        {
            MAX_WIDTH = max_width;
            MAX_HEIGHT = max_height;
        }


        public enum ConsoleFormatFlags
        {
            LEFT,
            CENTER,
            RIGHT,
            TOP_LEFT,
            TOP_CENTER,
            TOP_RIGHT,
            MIDDLE_LEFT,
            MIDDLE_CENTER,
            MIDDLE_RIGHT,
            BOTTOM_LEFT,
            BOTTOM_CENTER,
            BOTTOM_RIGHT
        }


        public static void SetCursorPosition(
            int msgLength,
            int y1,
            int y2,
            int x1,
            int x2,
            int xOffset = 0,
            int yOffset = 0,
            ConsoleFormatFlags flag = ConsoleFormatFlags.LEFT)
        {
            if (msgLength > (x2 - x1)) throw new StackOverflowException("Message is longer than the display area!");

            switch (flag)
            {
                case ConsoleFormatFlags.LEFT:
                    Console.SetCursorPosition(x1 + xOffset, y1 + yOffset);
                    return;
                case ConsoleFormatFlags.CENTER:
                    int msgCenter = msgLength / 2;
                    Console.SetCursorPosition(GetMidPoint(x1, x2) + xOffset - msgCenter, y1 + yOffset);
                    return;
                case ConsoleFormatFlags.RIGHT:
                    Console.SetCursorPosition(x2 - (msgLength + xOffset), y1 + yOffset);
                    return;
            }

            int x = 1, y = 1;

            switch (flag)
            {
                case ConsoleFormatFlags.TOP_LEFT:
                case ConsoleFormatFlags.MIDDLE_LEFT:
                case ConsoleFormatFlags.BOTTOM_LEFT:
                    x = x1 + xOffset;
                    break;
                case ConsoleFormatFlags.TOP_CENTER:
                case ConsoleFormatFlags.MIDDLE_CENTER:
                case ConsoleFormatFlags.BOTTOM_CENTER:
                    int msgCenter = msgLength / 2;
                    x = GetMidPoint(x1, x2) + xOffset - msgCenter;
                    break;
                case ConsoleFormatFlags.TOP_RIGHT:
                case ConsoleFormatFlags.MIDDLE_RIGHT:
                case ConsoleFormatFlags.BOTTOM_RIGHT:
                    x = x2 - (msgLength + xOffset);
                    break;
                default:
                    throw new InvalidDataException("Invalid Flag!");
            }

            switch (flag)
            {
                case ConsoleFormatFlags.TOP_LEFT:
                case ConsoleFormatFlags.TOP_CENTER:
                case ConsoleFormatFlags.TOP_RIGHT:
                    y = y1 + yOffset;
                    break;
                case ConsoleFormatFlags.MIDDLE_LEFT:
                case ConsoleFormatFlags.MIDDLE_CENTER:
                case ConsoleFormatFlags.MIDDLE_RIGHT:
                    y = GetMidPoint(y1, y2) - yOffset;
                    break;
                case ConsoleFormatFlags.BOTTOM_LEFT:
                case ConsoleFormatFlags.BOTTOM_CENTER:
                case ConsoleFormatFlags.BOTTOM_RIGHT:
                    y = y2 - yOffset;
                    break;
                default:
                    throw new InvalidDataException("Invalid Flag!");
            }

            if (y < 0 || x < 0 || x > MAX_WIDTH || y > MAX_HEIGHT) throw new Exception("Display area is too small!");

            Console.SetCursorPosition(x, y);
        }

        public static int Write(
            string message,
            int x1,
            int y1,
            int? x2,
            int? y2,
            int yOffset = 0,
            int xOffset = 0,
            int maxStringLength = 0,
            ConsoleFormatFlags flag = ConsoleFormatFlags.LEFT)
        {
            x2 ??= MAX_WIDTH;
            y2 ??= MAX_HEIGHT;

            List<string> messages = new List<string>();

            if (message.Length > (x2 - x1) && maxStringLength == 0) maxStringLength = x2.Value - x1 - 2;

            while (message.Length > maxStringLength && maxStringLength > 0)
            {
                if (maxStringLength > message.Length) maxStringLength = message.Length;
                messages.Add(message.Substring(0, maxStringLength));
                message = message.Substring(maxStringLength);
            }

            if (message.Length > 0) messages.Add(message);

            int _yOffset;
            for (int i = 0; i < messages.Count; i++)
            {
                _yOffset = yOffset + messages.Count - i - 1;
                if (flag == ConsoleFormatFlags.TOP_LEFT ||
                    flag == ConsoleFormatFlags.TOP_CENTER ||
                    flag == ConsoleFormatFlags.TOP_RIGHT)
                {
                    _yOffset = yOffset + i;
                }

                SetCursorPosition(messages[i].Length, y1, y2.Value, x1, x2.Value, xOffset, _yOffset, flag);
                Console.Write(messages[i]);

            }

            return messages.Count;
        }

        public static int Write(
            string[] messages,
            int x1,
            int y1,
            int? x2,
            int? y2,
            int yOffset = 0,
            int xOffset = 0,
            int maxStringLength = 0,
            ConsoleFormatFlags flag = ConsoleFormatFlags.LEFT)
        {
            x2 ??= MAX_WIDTH;
            y2 ??= MAX_HEIGHT;


            int _yOffset;
            for (int i = 0; i < messages.Length; i++)
            {
                _yOffset = yOffset + messages.Length - i - 1;
                if (flag == ConsoleFormatFlags.TOP_LEFT ||
                    flag == ConsoleFormatFlags.TOP_CENTER ||
                    flag == ConsoleFormatFlags.TOP_RIGHT)
                {
                    _yOffset = yOffset + i;
                }

                SetCursorPosition(messages[i].Length, y1, y2.Value, x1, x2.Value, xOffset, _yOffset, flag);
                Console.Write(messages[i]);

            }

            return messages.Length;
        }
        public static int GetMidPoint(int p1, int p2)
        {
            return p1 + ((p2 - p1) / 2);
        }

        public static void DrawBox(int x1, int x2, int y1, int y2, int? xOffset = null)
        {
            Console.SetCursorPosition(x1, y1);
            Console.Write('╔');

            Console.SetCursorPosition(x2, y1);
            Console.Write('╗');

            Console.SetCursorPosition(x2, y2);
            Console.Write("╝");

            Console.SetCursorPosition(x1, y2);
            Console.WriteLine("╚");

            xOffset ??= 0;

            DrawPipe(y1, x1 + 1 - xOffset.Value, x2);
            DrawPipe(y2, x1 + 1 - xOffset.Value, x2);


            DrawPillar(x1 - xOffset.Value, y1 + 1, y2);
            DrawPillar(x2 - xOffset.Value, y1 + 1, y2);
        }

        public static void DrawClosedPipe(int y, int x1, int x2)
        {
            DrawPipe(y, x1, x2);

            Console.SetCursorPosition(x1, y);
            Console.Write("╠");

            Console.SetCursorPosition(x2, y);
            Console.Write("╣");
        }

        public static void DrawPipe(int y, int x1, int x2, char material = '═')
        {
            for (int i = x1; i < x2; i++)
            {
                Console.SetCursorPosition(i, y);
                Console.Write('═');
            }
        }

        public static void DrawPillar(int x, int y1, int y2)
        {
            for (int i = y1; i < y2; i++)
            {
                Console.SetCursorPosition(x, i);
                Console.Write("║");
            }
        }

        public static void DrawClosedPillar(int x, int y1, int y2)
        {
            DrawPillar(x, y1, y2);

            Console.SetCursorPosition(x, y1);
            Console.Write("╦");

            Console.SetCursorPosition(x, y2);
            Console.Write("╩");
        }
    }
}
