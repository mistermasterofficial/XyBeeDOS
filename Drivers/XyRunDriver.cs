using Cosmos.System.Network.IPv4;
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
    class XyRunDriver
    {
        static Dictionary<int,FileStream> filetreams = new Dictionary<int,FileStream>();

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
        static void sys_open_filestream(ref Stack<int> operand)
        {
            var path = FileSystem.formatPath(GetStringFromStack(ref operand));
            var filemode = operand.Pop();
            int address = 0;

            for(int i = 0; i<filetreams.Count+1; i++) 
            { 
                if(!filetreams.ContainsKey(i))
                {
                    filetreams.Add(i, new FileStream(path, (FileMode)filemode));
                    address = i; break;
                }
            }

            operand.Push(address);
        }
        static void sys_is_filestream_can_read(ref Stack<int> operand)
        {
            int address = operand.Pop();
            operand.Push(Convert.ToInt32(filetreams[address].CanRead));
        }
        static void sys_is_filestream_can_write(ref Stack<int> operand)
        {
            int address = operand.Pop();
            operand.Push(Convert.ToInt32(filetreams[address].CanWrite));
        }
        static void sys_get_filestream_length(ref Stack<int> operand)
        {
            int address = operand.Pop();
            operand.Push(Convert.ToInt32(filetreams[address].Length));
        }
        static void sys_set_filestream_length(ref Stack<int> operand)
        {
            int address = operand.Pop();
            filetreams[address].SetLength(operand.Pop());
        }
        static void sys_get_filestream_position(ref Stack<int> operand)
        {
            int address = operand.Pop();
            operand.Push(Convert.ToInt32(filetreams[address].Position));
        }
        static void sys_set_filestream_position(ref Stack<int> operand)
        {
            int address = operand.Pop();
            filetreams[address].Position = operand.Pop();
        }
        static void sys_seek_filestream_position(ref Stack<int> operand)
        {
            int address = operand.Pop();
            int offset = operand.Pop();
            SeekOrigin origin = (SeekOrigin)operand.Pop();
            filetreams[address].Seek(offset, origin);
        }
        static void sys_read_byte_filestream(ref Stack<int> operand)
        {
            int address = operand.Pop();
            operand.Push(filetreams[address].ReadByte());
        }
        static void sys_write_byte_filestream(ref Stack<int> operand)
        {
            int address = operand.Pop();
            filetreams[address].WriteByte((byte)operand.Pop());
        }
        static void sys_close_filestream(ref Stack<int> operand)
        {
            int address = operand.Pop();
            filetreams[address].Close();
            filetreams.Remove(address);
        }
        static void sys_set_filestream_read_timeout(ref Stack<int> operand)
        {
            int address = operand.Pop();
            filetreams[address].ReadTimeout = operand.Pop();
        }
        static void sys_set_filestream_write_timeout(ref Stack<int> operand)
        {
            int address = operand.Pop();
            filetreams[address].WriteTimeout = operand.Pop();
        }
        static void sys_get_filestream_read_timeout(ref Stack<int> operand)
        {
            int address = operand.Pop();
            operand.Push(filetreams[address].ReadTimeout);
        }
        static void sys_get_filestream_write_timeout(ref Stack<int> operand)
        {
            int address = operand.Pop();
            operand.Push(filetreams[address].WriteTimeout);
        }
        static void sys_filestream_flush(ref Stack<int> operand)
        {
            int address = operand.Pop();
            filetreams[address].Flush();
        }
        static void sys_get_filestream_path(ref Stack<int> operand)
        {
            int address = operand.Pop();
            SetStringToStack(filetreams[address].Name, ref operand);
        }
        static void sys_fileystem_format_path(ref Stack<int> operand)
        {
            SetStringToStack(FileSystem.formatPath(GetStringFromStack(ref operand)), ref operand);
        }
        static void sys_file_create(ref Stack<int> operand)
        {
            File.Create(GetStringFromStack(ref operand));
        }
        static void sys_file_copy(ref Stack<int> operand)
        {
            var source = GetStringFromStack(ref operand);
            var dest = GetStringFromStack(ref operand);
            File.Copy(source, dest);
        }
        static void sys_file_delete(ref Stack<int> operand)
        {
            File.Delete(GetStringFromStack(ref operand));
        }
        static void sys_is_filestream_exists(ref Stack<int> operand)
        {
            File.Exists(GetStringFromStack(ref operand));
        }
        //static void sys_get_file_creationtime(ref Stack<int> operand)
        //{
        //    var path = GetStringFromStack(ref operand);
        //    operand.Push((int)File.GetCreationTime(path).Ticks); 
        //}
        //static void sys_get_file_lastaccesstime(ref Stack<int> operand)
        //{
        //    var path = GetStringFromStack(ref operand);
        //    operand.Push((int)File.GetLastAccessTime(path).Ticks); 
        //}
        //static void sys_get_file_lastwritetime(ref Stack<int> operand)
        //{
        //    var path = GetStringFromStack(ref operand);
        //    operand.Push((int)File.GetLastWriteTime(path).Ticks);
        //}
        //static void sys_set_file_creationtime(ref Stack<int> operand)
        //{
        //    var path = GetStringFromStack(ref operand);
        //    File.SetCreationTime(path, new DateTime(operand.Pop()));
        //}
        //static void sys_set_file_lastaccesstime(ref Stack<int> operand)
        //{
        //    var path = GetStringFromStack(ref operand);
        //    File.SetLastAccessTime(path, new DateTime(operand.Pop()));
        //}
        //static void sys_set_file_lastwritetime(ref Stack<int> operand)
        //{
        //    var path = GetStringFromStack(ref operand);
        //    File.SetLastWriteTime(path, new DateTime(operand.Pop()));
        //}
        static void sys_directory_create(ref Stack<int> operand)
        {
            Directory.CreateDirectory(GetStringFromStack(ref operand));
        }
        static void sys_directory_delete(ref Stack<int> operand)
        {
            Directory.Delete(GetStringFromStack(ref operand));
        }
        static void sys_is_directory_exists(ref Stack<int> operand)
        {
            Directory.Exists(GetStringFromStack(ref operand));
        }
        static void sys_get_directory_files(ref Stack<int> operand)
        {
            var files = Directory.GetFiles(GetStringFromStack(ref operand));
            files.Reverse();
            foreach(var file in files)
            {
                SetStringToStack(file, ref operand);
            }
            operand.Push(files.Length);
        }
        static void sys_get_directory_directories(ref Stack<int> operand)
        {
            var directories = Directory.GetDirectories(GetStringFromStack(ref operand));
            directories.Reverse();
            foreach (var directory in directories)
            {
                SetStringToStack(directory, ref operand);
            }
            operand.Push(directories.Length);
        }
        static void sys_get_current_directory(ref Stack<int> operand)
        {
            SetStringToStack(Directory.GetCurrentDirectory(), ref operand);
        }
        static void sys_set_current_directory(ref Stack<int> operand)
        {
            Directory.SetCurrentDirectory(GetStringFromStack(ref operand));
        }
        static void sys_get_directory_root(ref Stack<int> operand)
        {
            var path = GetStringFromStack(ref operand);
            SetStringToStack(Directory.GetDirectoryRoot(path), ref operand);
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
                case 40:
                    sys_open_filestream(ref operand); break;
                case 41:
                    sys_is_filestream_can_read(ref operand); break;
                case 42:
                    sys_is_filestream_can_write(ref operand); break;
                case 43:
                    sys_get_filestream_length(ref operand); break;
                case 44:
                    sys_set_filestream_length(ref operand); break;
                case 45:
                    sys_get_filestream_position(ref operand); break;
                case 46:
                    sys_set_filestream_position(ref operand); break;
                case 47:
                    sys_seek_filestream_position(ref operand); break;
                case 48:
                    sys_read_byte_filestream(ref operand); break;
                case 49:
                    sys_write_byte_filestream(ref operand); break;
                case 50:
                    sys_close_filestream(ref operand); break;
                case 51:
                    sys_set_filestream_read_timeout(ref operand); break;
                case 52:
                    sys_set_filestream_write_timeout(ref operand); break;
                case 53:
                    sys_get_filestream_read_timeout(ref operand); break;
                case 54:
                    sys_get_filestream_write_timeout(ref operand); break;
                case 55:
                    sys_filestream_flush(ref operand); break;
                case 56:
                    sys_get_filestream_path(ref operand); break;
                case 57:
                    sys_fileystem_format_path(ref operand); break;
                case 58:
                    sys_file_create(ref operand); break;
                case 59:
                    sys_file_copy(ref operand); break;
                case 60:
                    sys_file_delete(ref operand); break;
                case 61:
                    sys_is_filestream_exists(ref operand); break;
                case 62:
                    sys_directory_create(ref operand); break;
                case 63:
                    sys_directory_delete(ref operand); break;
                case 64:
                    sys_is_directory_exists(ref operand); break;
                case 65:
                    sys_get_directory_files(ref operand); break;
                case 66:
                    sys_get_directory_directories(ref operand); break;
                case 67:
                    sys_get_current_directory(ref operand); break;
                case 68:
                    sys_set_current_directory(ref operand); break;
                case 69:
                    sys_get_directory_root(ref operand); break;
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
