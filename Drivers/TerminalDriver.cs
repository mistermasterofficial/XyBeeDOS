using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys = Cosmos.System;

namespace XyBeeDOS.Drivers
{
    class TerminalInput
    {
        static Queue<TerminalInput> terminalInputs = new Queue<TerminalInput>();

        string result = "";
        int cursorleft; int cursortop;
        private TerminalInput()
        {
            terminalInputs.Enqueue(this);
            cursorleft = Console.CursorLeft; cursortop = Console.CursorTop;
        }
        public bool update(bool visible)
        {
            if (Sys.KeyboardManager.TryReadKey(out var key))
            {
                if (key.Key == Sys.ConsoleKeyEx.Enter)
                {
                    return true;
                }
                else if (key.Key == Sys.ConsoleKeyEx.Backspace && result.Length > 0)
                {
                    result = result.Remove(result.Length - 1);
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    result += key.KeyChar;
                }
            }
            if (visible)
            {
                Console.CursorLeft = cursorleft;
                Console.CursorTop = cursortop;
                Console.Write(result);
            }
            return false;
        }
        public string getResult()
        {
            return result;
        }

        public static bool update(out string result, bool visible)
        {
            if (terminalInputs.Count == 0) new TerminalInput();

            bool end = terminalInputs.Peek().update(visible);
            result = "";
            if (end)
            {
                result = terminalInputs.Peek().getResult();
                terminalInputs.Dequeue();
            }
            return end;
        }

        public static string ReadLine()
        {
            List<char> input = new List<char>();
            int empty_char_count = 0;
            int cursor = 0;

            try
            {
                while (true)
                {
                    if (Sys.KeyboardManager.TryReadKey(out var key))
                    {
                        TerminalDriver.MoveCursor(-cursor);

                        if (key.Key == Sys.ConsoleKeyEx.Enter)
                        {
                            Console.WriteLine(input.ToArray());
                            break;
                        }
                        else if ((key.Modifiers & ConsoleModifiers.Shift) != 0 && (key.Modifiers & ConsoleModifiers.Alt) != 0 && key.Key == Sys.ConsoleKeyEx.Z)
                        {
                            Language.SwitchLanguage();
                        }
                        else if (key.Key == Sys.ConsoleKeyEx.Backspace && cursor > 0)
                        {
                            input.RemoveAt(--cursor);
                            empty_char_count++;
                        }
                        else if (key.Key == Sys.ConsoleKeyEx.Delete && cursor < input.Count)
                        {
                            input.RemoveAt(cursor);
                            empty_char_count++;
                        }
                        else if (key.Key == Sys.ConsoleKeyEx.LeftArrow && cursor > 0)
                        {
                            cursor--;
                        }
                        else if (key.Key == Sys.ConsoleKeyEx.RightArrow && cursor < input.Count)
                        {
                            cursor++;
                        }
                        else if (!char.IsControl(key.KeyChar))
                        {
                            input.Insert(cursor, key.KeyChar);
                            cursor++;
                            empty_char_count = empty_char_count - 1 >= 0 ? empty_char_count - 1 : 0;
                        }

                        Console.Write(input.ToArray()); for (int i = 0; i < empty_char_count; i++) Console.Write(Convert.ToChar(0));
                        TerminalDriver.MoveCursor(-(input.Count + empty_char_count));
                        TerminalDriver.MoveCursor(cursor);
                    }
                }
            }
            catch(Exception ex) { Console.WriteLine(ex.ToString()); }

            return new string(input.ToArray());
        }
    }

    public class TerminalDriver
    {
        static Cosmos.HAL.VGADriver.TextSize console_size;

        static int current_encoding_num;

        static Sys.KeyEvent key_info;

        public static bool UpdateKeyInfo()
        {
            return Sys.KeyboardManager.TryReadKey(out key_info);
        }

        public static Sys.KeyEvent GetKeyInfo() { return key_info; }

        public static void Init()
        {
            Encoding.RegisterProvider(XyBeeEncodingProvider.Instance);
            SetMode(Cosmos.HAL.VGADriver.TextSize.Size80x25, 866);
        }

        public static void SetMode(Cosmos.HAL.VGADriver.TextSize size, int encoding)
        {
            console_size = size;
            Sys.Graphics.VGAScreen.SetTextMode(size);
            Console.SetWindowSize(GetWidth(), GetHeight());

            SetEncoding(encoding);
        }

        public static void SetEncoding(int encoding)
        {
            current_encoding_num = encoding;
            Sys.Graphics.VGAScreen.SetFont(VGAFont.GetFontArray(current_encoding_num, GetFontHeight()), GetFontHeight());

            Console.InputEncoding = Encoding.GetEncoding(current_encoding_num);
            Console.OutputEncoding = Encoding.GetEncoding(current_encoding_num);
        }

        public static int GetCurrentEncodingNum() { return current_encoding_num; }

        public static int GetWidth()
        {
            switch (console_size)
            {
                case Cosmos.HAL.VGADriver.TextSize.Size40x25:
                    return 40;
                case Cosmos.HAL.VGADriver.TextSize.Size40x50:
                    return 40;
                case Cosmos.HAL.VGADriver.TextSize.Size80x25:
                    return 80;
                case Cosmos.HAL.VGADriver.TextSize.Size80x50:
                    return 80;
                case Cosmos.HAL.VGADriver.TextSize.Size90x30:
                    return 90;
                case Cosmos.HAL.VGADriver.TextSize.Size90x60:
                    return 90;
            }
            return 0;
        }

        public static int GetHeight()
        {
            switch (console_size)
            {
                case Cosmos.HAL.VGADriver.TextSize.Size40x25:
                    return 25;
                case Cosmos.HAL.VGADriver.TextSize.Size40x50:
                    return 50;
                case Cosmos.HAL.VGADriver.TextSize.Size80x25:
                    return 25;
                case Cosmos.HAL.VGADriver.TextSize.Size80x50:
                    return 50;
                case Cosmos.HAL.VGADriver.TextSize.Size90x30:
                    return 30;
                case Cosmos.HAL.VGADriver.TextSize.Size90x60:
                    return 60;
            }
            return 0;
        }

        public static int GetFontHeight()
        {
            switch (console_size)
            {
                case Cosmos.HAL.VGADriver.TextSize.Size40x25:
                    return 16;
                case Cosmos.HAL.VGADriver.TextSize.Size40x50:
                    return 8;
                case Cosmos.HAL.VGADriver.TextSize.Size80x25:
                    return 16;
                case Cosmos.HAL.VGADriver.TextSize.Size80x50:
                    return 8;
                case Cosmos.HAL.VGADriver.TextSize.Size90x30:
                    return 16;
                case Cosmos.HAL.VGADriver.TextSize.Size90x60:
                    return 8;
            }
            return 0;
        }

        public static Cosmos.HAL.VGADriver.TextSize GetTextSize()
        {
            return console_size;
        }

        public static void MoveCursor(int count)
        {
            int CursorLeft = (Console.CursorLeft + count) % GetWidth();
            if (CursorLeft < 0) CursorLeft += GetWidth();
            int CursorTop = (Console.CursorLeft + count) / GetWidth();
            if (count < 0 && Console.CursorLeft + count < 0) CursorTop--;
            Console.CursorLeft = CursorLeft;
            Console.CursorTop += CursorTop;
        }
    }
}
