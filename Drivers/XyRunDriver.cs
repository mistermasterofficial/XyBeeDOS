using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XyBeeDOS.BuiltinApps;
using Sys = Cosmos.System;

namespace XyBeeDOS.Drivers
{
    //class XyRunLibDriver
    //{
    //    Dictionary<int, Dictionary<int, int>> functions = new Dictionary<int, Dictionary<int, int>>();

    //    public XyRunLibDriver() { }

    //    public void loadLib(ref Stack<int> operand)
    //    {
    //        var lib_num = operand.Pop();
    //        string lib_path = "";
    //        int c;
    //        do
    //        {
    //            c = operand.Pop();
    //            lib_path += c;
    //        }
    //        while (c != 0);

    //        exporerLib(lib_num, lib_path);
    //    }

    //    int read(FileStream file, int step_length)
    //    {
    //        int res = 0;
    //        for (int i = 0; i < step_length; i++)
    //        {
    //            res <<= 8;
    //            res |= file.ReadByte();
    //        }
    //        return res;
    //    }

    //    void exporerLib(int lib_num, string path)
    //    {
    //        functions[lib_num] = new Dictionary<int, int>();
    //        using (var file = File.OpenRead(FileSystem.formatPath(path)))
    //        {
    //            if (read(file,4)>0) throw new 
    //        };
    //    }
    //}

    class XyRunDriver
    {
        static void sys_keyinfo_keychar(ref Stack<int> operand)
        {
            operand.Push(TerminalDriver.GetKeyInfo().KeyChar);
        }
        static void sys_keyinfo_modifiers(ref Stack<int> operand)
        {
            operand.Push((int)TerminalDriver.GetKeyInfo().Modifiers);
        }
        static void sys_is_capslock(ref Stack<int> operand)
        {
            operand.Push(Convert.ToInt32(Console.CapsLock));
        }
        static void sys_is_numberlock(ref Stack<int> operand)
        {
            operand.Push(Convert.ToInt32(Console.NumberLock));
        }
        static void sys_keyinfo_key(ref Stack<int> operand)
        {
            operand.Push((int)TerminalDriver.GetKeyInfo().Key);
        }
        static void sys_is_keyavailable(ref Stack<int> operand)
        {
            operand.Push(Convert.ToInt32(Console.KeyAvailable));
        }
        static void sys_get_cursorleft(ref Stack<int> operand)
        {
            operand.Push(Console.CursorLeft);
        }
        static void sys_get_cursortop(ref Stack<int> operand)
        {
            operand.Push(Console.CursorTop);
        }
        static void sys_get_width(ref Stack<int> operand)
        {
            operand.Push(TerminalDriver.GetWidth());
        }
        static void sys_get_height(ref Stack<int> operand)
        {
            operand.Push(TerminalDriver.GetHeight());
        }
        static void sys_get_foregroundcolor(ref Stack<int> operand)
        {
            operand.Push((int)Console.ForegroundColor);
        }
        static void sys_get_backgroundcolor(ref Stack<int> operand)
        {
            operand.Push((int)Console.BackgroundColor);
        }
        static void sys_get_cursorsize(ref Stack<int> operand)
        {
            operand.Push(Console.CursorSize);
        }
        static void sys_is_cursorvisible(ref Stack<int> operand)
        {
            operand.Push(Convert.ToInt32(Console.CursorVisible));
        }
        static void sys_get_fontheight(ref Stack<int> operand)
        {
            operand.Push(TerminalDriver.GetFontHeight());
        }
        static void sys_get_currentencodingnum(ref Stack<int> operand)
        {
            operand.Push(TerminalDriver.GetCurrentEncodingNum());
        }
        static void sys_cls(ref Stack<int> operand)
        {
            Console.Clear();
        }
        static void sys_writeline(ref Stack<int> operand)
        {
            Console.WriteLine();
        }
        static void sys_write(ref Stack<int> operand)
        {
            Console.Write(GetStringFromStack(ref operand));  
        }
        static void sys_resetcolor(ref Stack<int> operand)
        {
            Console.ResetColor();
        }
        static void sys_set_cursorleft(ref Stack<int> operand)
        {
            Console.CursorLeft = operand.Pop();
        }
        static void sys_set_cursortop(ref Stack<int> operand)
        {
            Console.CursorTop = operand.Pop();
        }
        static void sys_set_cursorvisible(ref Stack<int> operand)
        {
            Console.CursorVisible = operand.Pop() != 0;
        }
        static void sys_set_foregroundcolor(ref Stack<int> operand)
        {
            Console.ForegroundColor = (ConsoleColor)operand.Pop();
        }
        static void sys_set_backgroundcolor(ref Stack<int> operand)
        {
            Console.BackgroundColor = (ConsoleColor)operand.Pop();
        }
        static void sys_pcspeaker_beep(ref Stack<int> operand)
        {
            Sys.PCSpeaker.Beep();
        }
        static void sys_pcspeaker_fdbeep(ref Stack<int> operand)
        {
            Sys.PCSpeaker.Beep((uint)operand.Pop(), (uint)operand.Pop());
        }
        static void sys_pcspeaker_ndbeep(ref Stack<int> operand)
        {
            Sys.PCSpeaker.Beep((Sys.Notes)operand.Pop(), (Sys.Durations)operand.Pop());
        }
        static void sys_updatekeyinfo(ref Stack<int> operand)
        {
            operand.Push(Convert.ToInt32(TerminalDriver.UpdateKeyInfo()));
        }
        static void sys_power_shutdown(ref Stack<int> operand)
        {
            Sys.Power.Shutdown();
        }
        static void sys_power_reboot(ref Stack<int> operand)
        {
            Sys.Power.Reboot();
        }
        static void sys_set_encodingnum(ref Stack<int> operand)
        {
            TerminalDriver.SetEncoding(operand.Pop());
        }
        static void sys_set_mode(ref Stack<int> operand)
        {
            TerminalDriver.SetMode((Cosmos.HAL.VGADriver.TextSize)operand.Pop(), operand.Pop());
        }
        static void sys_appmanager_quitapp(ref Stack<int> operand)
        {
            AppManager.quitApp();
        }
        static void sys_set_appmanager_value(ref Stack<int> operand)
        {
            string key = GetStringFromStack(ref operand);
            string value = GetStringFromStack(ref operand);
            AppManager.setValue(key, value);
        }
        static void sys_get_appmanager_value(ref Stack<int> operand)
        {
            string key1 = GetStringFromStack(ref operand);
            SetStringToStack(AppManager.getValue(key1), ref operand);
        }
        static void sys_appmanager_startapp(ref Stack<int> operand)
        {
            string path = FileSystem.formatPath(GetStringFromStack(ref operand));
            int args_count = operand.Pop();
            string[] args = new string[args_count];
            if (args_count > 0)
            {
                for (int i = 0; i < args_count; i++)
                {
                    args[i] = GetStringFromStack(ref operand);
                }
            }
            AppManager.startApp(new XyRun(path, args));
        }
        static void sys_get_appmanager_errors(ref Stack<int> operand)
        {
            operand.Push(AppManager.getErrors());
        }
        static void sys_get_mode(ref Stack<int> operand)
        {
            operand.Push((int)TerminalDriver.GetTextSize());
        }
        static void sys_readline(ref Stack<int> operand)
        {
            var input = TerminalInput.ReadLine();
            SetStringToStack(input, ref operand);
        }

        public static void syscall(int syscall_num, ref Stack<int> operand)
        {
            switch (syscall_num)
            {
                case 0:
                    sys_keyinfo_keychar(ref operand); break;
                case 1:
                    sys_keyinfo_modifiers(ref operand); break;
                case 2:
                    sys_is_capslock(ref operand); break;
                case 3:
                    sys_is_numberlock(ref operand); break;
                case 4:
                    sys_keyinfo_key(ref operand); break;
                case 5:
                    sys_is_keyavailable(ref operand); break;
                case 6:
                    sys_get_cursorleft(ref operand); break;
                case 7:
                    sys_get_cursortop(ref operand); break;
                case 8:
                    sys_get_width(ref operand); break;
                case 9:
                    sys_get_height(ref operand); break;
                case 10:
                    sys_get_foregroundcolor(ref operand); break;
                case 11:
                    sys_get_backgroundcolor(ref operand); break;
                case 12:
                    sys_get_cursorsize(ref operand); break;
                case 13:
                    sys_is_cursorvisible(ref operand); break;
                case 14:
                    sys_get_fontheight(ref operand); break;
                case 15:
                    sys_get_currentencodingnum(ref operand); break;
                case 16:
                    sys_cls(ref operand); break;
                case 17:
                    sys_writeline(ref operand); break;
                case 18:
                    sys_write(ref operand); break;
                case 19:
                    sys_resetcolor(ref operand); break;
                case 20:
                    sys_set_cursorleft(ref operand); break;
                case 21:
                    sys_set_cursortop(ref operand); break;
                case 22:
                    sys_set_cursorvisible(ref operand); break;
                case 23:
                    sys_set_foregroundcolor(ref operand); break;
                case 24:
                    sys_set_backgroundcolor(ref operand); break;
                case 25:
                    sys_pcspeaker_beep(ref operand); break;
                case 26:
                    sys_pcspeaker_fdbeep(ref operand); break;
                case 27:
                    sys_pcspeaker_ndbeep(ref operand); break;
                case 28:
                    sys_updatekeyinfo(ref operand); break;
                case 29:
                    sys_power_shutdown(ref operand); break;
                case 30:
                    sys_power_reboot(ref operand); break;
                case 31:
                    sys_set_encodingnum(ref operand); break;
                case 32:
                    sys_set_mode(ref operand); break;
                case 33:
                    sys_appmanager_quitapp(ref operand); break;
                case 34:
                    sys_set_appmanager_value(ref operand); break;
                case 35:
                    sys_get_appmanager_value(ref operand); break;
                case 36:
                    sys_appmanager_startapp(ref operand); break;
                case 37:
                    sys_get_appmanager_errors(ref operand); break;
                case 38:
                    sys_get_mode(ref operand); break;
                case 39:
                    sys_readline(ref operand); break;
            }
        }

        public static string GetStringFromStack(ref Stack<int> operand) 
        {
            string res = "";
            char s = (char)operand.Pop();
            while (s != 0)
            {
                res += s;
                s = (char)operand.Pop();
            }
            return res;
        }

        public static void SetStringToStack(string value, ref Stack<int> operand)
        {
            Stack<int> reverse_stack = new Stack<int>();
            foreach(char s in value)
            {
                reverse_stack.Push(s);
            }
            reverse_stack.Push(0);
            while(reverse_stack.Count > 0)
            {
                operand.Push(reverse_stack.Pop());
            }
        }
    }
}
