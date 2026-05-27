using Cosmos.Core;
using Cosmos.HAL.BlockDevice;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XyBeeDOS.BuiltinApps;
using XyBeeDOS.Drivers;
using Sys = Cosmos.System;

namespace XyBeeDOS.BuiltinApps
{
    interface TerminalCommand
    {
        public string Description { get; }
        public List<string> arguments { get; }
        public void Execute(List<string> args);
    }

    class TerminalCommandAction : TerminalCommand
    {
        public Action action { get; }
        public string Description { get; }
        public List<string> arguments { get; }

        public TerminalCommandAction(string description, Action action)
        {
            this.Description = description;
            this.action = action;
            this.arguments = new List<string>();
        }

        public void Execute(List<string> args)
        {
            action();
        }
    }

    class TerminalCommandLs : TerminalCommand
    {
        public string Description => "view all files and directories";

        public List<string> arguments => new List<string>() { "[dirpath]" };

        public void Execute(List<string> args)
        {
            var path = Directory.GetCurrentDirectory();
            if (args[0].Length > 0) path = FileSystem.formatPath(args[0]);

            foreach (var dir in Directory.GetDirectories(path))
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write($"{dir}/");
                Console.ResetColor();
                Console.Write(" ");
            }
            foreach (var file in Directory.GetFiles(path))
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"{file}");
                Console.ResetColor();
                Console.Write(" ");
            }
            Console.WriteLine();
        }
    }

    class TerminalCommandCd : TerminalCommand
    {
        public string Description => "change current directory";

        public List<string> arguments => new List<string>() { "path" };

        public void Execute(List<string> args)
        {
            if (args[0].Length == 0) throw new ArgumentException();

            string path = FileSystem.formatPath(args[0]);
            if (Directory.Exists(path))
            {
                Directory.SetCurrentDirectory(path);
            }
            else
            {
                TerminalCommandManager.ShowError("Path is not exist");
            }
        }
    }

    class TerminalCommandMkfile : TerminalCommand
    {
        public string Description => "make file";

        public List<string> arguments => new List<string> { "filename" };

        public void Execute(List<string> args)
        {
            if (args[0].Length == 0) throw new ArgumentException();

            string filename = FileSystem.formatPath(args[0]);
            if (!File.Exists(filename) && !Directory.Exists(filename)) File.Create(filename);
            else TerminalCommandManager.ShowError("File (or directory with same path) is already exist");
        }
    }

    class TerminalCommandMkdir : TerminalCommand
    {
        public string Description => "make directory";

        public List<string> arguments => new List<string> { "dirname" };

        public void Execute(List<string> args)
        {
            if (args[0].Length == 0) throw new ArgumentException();

            string dirname = FileSystem.formatPath(args[0]);
            if (!Directory.Exists(dirname) && !File.Exists(dirname)) Directory.CreateDirectory(dirname);
            else TerminalCommandManager.ShowError("Directory (or file with same path) is already exist");
        }
    }

    class TerminalCommandRm : TerminalCommand
    {
        public string Description => "delete directory (recursively) or file";

        public List<string> arguments => new List<string> { "path" };

        public void Execute(List<string> args)
        {
            if (args[0].Length == 0) throw new ArgumentException();

            string path = FileSystem.formatPath(args[0]);
            if (File.Exists(path)) File.Delete(path);
            else if (Directory.Exists(path)) Directory.Delete(path, true);
            else TerminalCommandManager.ShowError("Path is not exist");
        }
    }

    class TerminalCommandCat : TerminalCommand
    {
        public string Description => "view file content";

        public List<string> arguments => new List<string> { "filepath" };

        public void Execute(List<string> args)
        {
            if (args[0].Length == 0) throw new ArgumentException();
            
            string filepath = FileSystem.formatPath(args[0]);
            if (File.Exists(filepath))
            {
                var stream = File.OpenRead(filepath);
                for (int i = 0; i < stream.Length; i++)
                {
                    Console.Write(Convert.ToChar(stream.ReadByte()));
                }
                stream.Close();
                Console.WriteLine();
            }
            else
            {
                TerminalCommandManager.ShowError("File is not exist");
            }
        }
    }

    class TerminalCommandQuestionMark : TerminalCommand
    {
        public string Description => "show command description with arguments";

        public List<string> arguments => new List<string> { "commandName" };

        public void Execute(List<string> args)
        {
            var commands = TerminalCommandManager.GetCommands();
            Console.ResetColor();
            string commandName = args[0];
            if (!commands.ContainsKey(commandName))
            {
                TerminalCommandManager.ShowError("Unknown command");
            }
            else
            {
                Console.Write($"{commandName} ");
                Console.ForegroundColor = ConsoleColor.Gray;
                foreach (string arg in commands[commandName].arguments)
                {
                    if (arg[0] == '[') Console.Write(arg+" ");
                    else Console.Write($"<{arg}> ");
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"- {commands[commandName].Description}");
            }
        }
    }

    class TerminalCommandSystemInfo : TerminalCommand
    {
        public string Description => "show system info";

        public List<string> arguments => new List<string>();

        public void Execute(List<string> args)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== CPU Information ===");
            Console.ResetColor();

            var vendor = CPU.GetCPUVendorName();
            var brand = CPU.GetCPUBrandString();

            Console.Write("Vendor: "); Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(vendor); Console.ResetColor();
            Console.Write("Brand: "); Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(brand); Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n=== Memory Information ===");
            Console.ResetColor();

            var availableRam = GCImplementation.GetAvailableRAM();

            var usedRam = GCImplementation.GetUsedRAM();
            Console.Write("Available RAM: "); Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine($"{availableRam} MB"); Console.ResetColor();
            Console.Write("Used RAM: "); Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine($"{usedRam} bytes"); Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n=== Disk Information ===");
            Console.ResetColor();

            for(int i = 0; i<FileSystem.GetFS().Disks.Count; i++)
            {
                var disk = FileSystem.GetFS().Disks[i];
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Disk {i}:"+@"\");
                Console.ResetColor();
                Console.Write("Disk Size: "); Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine($"{disk.Size} bytes"); Console.ResetColor();
                Console.Write("Is Contains MBR: "); Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine($"{disk.IsMBR}"); Console.ResetColor();
                if (disk.IsMBR)
                {
                    var partitions = disk.Partitions;
                    for (int j = 0; i < partitions.Count; i++)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine($"Partition {j + 1}: StartSector={partitions[i].Host.StartingSector}, Size={partitions[i].MountedFS.Size} sectors");
                        Console.ResetColor();
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n=== Date/Time ===");
            Console.ResetColor();
            Console.Write("Current Date/Time: "); Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(DateTime.Now);
            Console.ResetColor();
        }
    }

    class TerminalCommandEncoding : TerminalCommand
    {
        public string Description => "set encoding by number or get current encoding";

        public List<string> arguments => new List<string>() { "[encodingNum]" };

        public void Execute(List<string> args)
        {
            if (args[0].Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Current encoding: ");
                Console.WriteLine(TerminalDriver.GetCurrentEncodingNum());
                return;
            }

            int encodingNum = int.Parse(args[0]);
            Encoding encoding = XyBeeEncodingProvider.Instance.GetEncoding(encodingNum);

            if (encoding == null) { TerminalCommandManager.ShowError("Unknown encoding"); return; }
            else
            {
                TerminalDriver.SetEncoding(encodingNum);
            }
        }
    }

    //class TerminalCommandBuiltinApp : TerminalCommand
    //{
    //    public string Description => "run builtin app";

    //    public List<string> arguments => new List<string>() { "appName", "[arguments]" };

    //    public void Execute(List<string> args)
    //    {
    //        try
    //        {
    //            int status;
    //            string appName = args[0];

    //            if (args.Count > 1)
    //            {
    //                args.RemoveAt(0);
    //                status = AppList.runAppByName(appName, args);
    //            }
    //            else
    //            {
    //                status = AppList.runAppByName(appName, new List<string>() { "" });
    //            }

    //            if (status != 0) TerminalCommandManager.ShowError($"Builtin app start error. Status code {status}");
    //        }
    //        catch(Exception ex)
    //        {
    //            TerminalCommandManager.ShowError(ex.Message);
    //        }
    //    }
    //}

    class TerminalCommandXyRun : TerminalCommand
    {
        public string Description => "run XyRun app";

        public List<string> arguments => new List<string>() { "path" };

        public void Execute(List<string> args)
        {
            AppManager.startApp(new XyRun(FileSystem.formatPath(args[0]), new string[1] { "hello" }));
        }
    }

    class TerminalCommandLanguage : TerminalCommand
    {
        public string Description => "change language by num or show all available languages";

        public List<string> arguments => new List<string>() { "[languageNum]" };

        public void Execute(List<string> args)
        {
            if (args[0].Length == 0)
            {
                Console.ResetColor();
                Console.WriteLine("Language list:");
                for(int i = 0; i < Language.LanguageCount(); i++)
                {
                    Console.WriteLine($"{i+1}. {Language.GetName(i)}");
                }
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Current language: ");
                Console.Write((Language.GetCurrentLanguageNum()+1)+". ");
                Console.WriteLine(Language.GetName(Language.GetCurrentLanguageNum()));
                Console.ResetColor();
                return;
            }

            Language.SetLanguage(int.Parse(args[0])-1);
        }
    }

    class TerminalCommandManager
    {
        static Dictionary<string, TerminalCommand> commands = new Dictionary<string, TerminalCommand>();

        public static void ShowError(string message)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void ShowWarn(string message)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void Init()
        {
            commands.Add("ls", new TerminalCommandLs());
            commands.Add("reboot", new TerminalCommandAction("reboot DOS", () => { Sys.Power.Reboot(); }));
            commands.Add("shutdown", new TerminalCommandAction("shutdown DOS", () => { Sys.Power.Shutdown(); }));
            commands.Add("cls", new TerminalCommandAction("clear screen", () => { Console.Clear(); }));
            commands.Add("cd", new TerminalCommandCd());
            commands.Add("mkfile", new TerminalCommandMkfile());
            commands.Add("mkdir", new TerminalCommandMkdir());
            commands.Add("rm", new TerminalCommandRm());
            commands.Add("cat", new TerminalCommandCat());
            commands.Add("listcmd", new TerminalCommandAction("show list of commands", () =>
            {
                List<string> list = GetCommands().Keys.ToList();
                Console.ResetColor();
                foreach (string key in list)
                {
                    Console.Write($"{key} ");
                }
                Console.WriteLine();
            }));
            commands.Add("help", new TerminalCommandAction("show commands list with description and arguments", () =>
            {
                var commands = GetCommands();
                List<string> list = commands.Keys.ToList();
                foreach (string key in list)
                {
                    List<string> arg = new List<string>() { key };
                    commands["?"].Execute(arg);
                }
            }));
            commands.Add("?", new TerminalCommandQuestionMark());
            commands.Add("system", new TerminalCommandSystemInfo());
            commands.Add("encoding", new TerminalCommandEncoding());
            //commands.Add("bapp", new TerminalCommandBuiltinApp());
            //commands.Add("exit", new TerminalCommandAction("close this terminal app instance or shutdown DOS", () =>
            //{
            //    AppManager.quitApp();
            //}));
            commands.Add(".", new TerminalCommandLanguage());
            commands.Add("run", new TerminalCommandXyRun());
        }

        public static Dictionary<string, TerminalCommand> GetCommands()
        {
            return commands;
        }

        public static void Execute(string input)
        {
            if (input.Length != 0)
            {
                if (!input.Contains(" ")) input += " ";
                string[] argsArray = input.Split(" ");
                List<string> args = argsArray.ToList();
                string command = args[0].ToLower();
                args.RemoveAt(0);
                if (commands.ContainsKey(command))
                {
                    try
                    {
                        commands[command].Execute(args);
                        Console.ResetColor();
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex.Message);
                        commands["?"].Execute(new List<string>() { command });
                    }
                }
                else
                {
                    ShowError("Unknown command");
                }
            }
        }
    }

    class TerminalApp : App
    {
        string current_dir;

        public TerminalApp(string current_dir)
        {
            this.current_dir = FileSystem.formatPath(current_dir);
        }

        void writeLabel()
        {
            int sourceX, sourceY;
            ConsoleColor sourceBackground, sourceForeground;
            sourceX = Console.CursorLeft; sourceY = Console.CursorTop;
            sourceBackground = Console.BackgroundColor; sourceForeground = Console.ForegroundColor;

            if (sourceY <= 1) { sourceY = 1; sourceX = 0; }

            Console.CursorLeft = 0; Console.CursorTop = 0;
                        
            if (sourceY <= 1) { sourceY = 2; sourceX = 0; }
            Console.BackgroundColor = ConsoleColor.Blue; Console.ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < TerminalDriver.GetWidth(); i++) Console.Write(" ");
            string label = $"XyBeeDOS v{Kernel.GetVersion()}";
            Console.CursorTop = 0; Console.CursorLeft = TerminalDriver.GetWidth() - label.Length; Console.CursorLeft /= 2;
            Console.Write(label);
            Console.CursorTop = 1; Console.CursorLeft = 0;
            
            Console.BackgroundColor = ConsoleColor.Cyan; Console.ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < TerminalDriver.GetWidth(); i++) Console.Write(" ");
            Console.CursorTop -= 1;
            string curdir = Directory.GetCurrentDirectory();
            int shift = curdir.Length-TerminalDriver.GetWidth(); if (shift < 0) shift = 0;
            for (int i = shift; i < curdir.Length; i++) Console.Write(curdir[i]);
            if (shift > 0) 
            {
                Console.CursorLeft = 0;
                Console.CursorTop -= 1;
                Console.Write('~');
            }

            Console.CursorLeft = sourceX; Console.CursorTop = sourceY;
            Console.BackgroundColor = sourceBackground; Console.ForegroundColor = sourceForeground;
        }

        public int BeforeRun()
        {
            App intro = new Intro();
            intro.BeforeRun();

            if (Directory.Exists(current_dir)) Directory.SetCurrentDirectory(current_dir);

            Console.BackgroundColor = ConsoleColor.Black; Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            TerminalDriver.SetMode(Cosmos.HAL.VGADriver.TextSize.Size90x30, 866);
            TerminalDriver.SetEncoding(437);

            return 0;
        }

        public int Quit()
        {
            Sys.Power.Shutdown();
            return 0;
        }

        public int Run()
        {
            Directory.SetCurrentDirectory(current_dir);

            writeLabel();

            var error = AppManager.getErrors();
            if (error != 0) TerminalCommandManager.ShowError($"App run error. Status code {error}\n"+AppManager.getValue("EXCEPTION"));

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(">> ");
            Console.ResetColor();
            var input = TerminalInput.ReadLine();
            TerminalCommandManager.Execute(input);
            if (!AppManager.isPaused()) current_dir = Directory.GetCurrentDirectory();

            return 0;
        }
    }
}
