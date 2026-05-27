using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XyBeeDOS.Drivers;
using Sys = Cosmos.System;

namespace XyBeeDOS.BuiltinApps
{
    public class Intro : App
    {
        string[] art; ConsoleColor[] colormap;

        public int Run()
        {
            return 0;
        }

        public int Quit()
        {
            return 0;
        }

        public int BeforeRun()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            TerminalDriver.SetMode(Cosmos.HAL.VGADriver.TextSize.Size40x25, 437);
            int x, y;
            x = TerminalDriver.GetWidth() - art[0].Length; x /= 2;
            y = TerminalDriver.GetHeight() - art.Length; y /= 2;
            Console.CursorLeft = x; Console.CursorTop = y;

            for(int i = 0; i < art.Length; i++)
            {
                for(int j = 0; j < art[0].Length; j++)
                {
                    Console.ForegroundColor = colormap[i * art[0].Length + j];
                    Console.Write(art[i][j]);
                }
                Console.CursorLeft = x;
                Console.CursorTop = y++;
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine();
            Console.WriteLine($"Welcome to XyBeeDOS v{Kernel.GetVersion()}!");

            Sys.PCSpeaker.Beep(Sys.Notes.B4, Sys.Durations.Eighth);
            Thread.Sleep((int)Sys.Durations.Eighth);
            Sys.PCSpeaker.Beep(Sys.Notes.DS5, Sys.Durations.Eighth);
            Thread.Sleep((int)Sys.Durations.Eighth);
            Sys.PCSpeaker.Beep(Sys.Notes.FS5, Sys.Durations.Eighth);
            Thread.Sleep((int)Sys.Durations.Eighth);
            Sys.PCSpeaker.Beep(Sys.Notes.B5, Sys.Durations.Eighth);

            return 0;
        }

        public Intro()
        {
            art = new string[] { 
            "                                      ",
            "  ███████                    ███████  ",
            "████████████      ░░░      ███████████",
            "████████░ ░█▓▒▒▒▒▒▒▒▒▒▒▒▒▓█▒ ░████████",
            "█████████▓░ ▒▓▒▒▒▒▒▒▒▒▒▒▓▒ ░▓█████████",
            "█████████▓▓▓░░░░░    ░░░░░▒▓▓█████████",
            "  ██████▓▓▓▓░            ░▓▓▓▓███████ ",
            "   ████▓▓▓▒░              ░▓▓▓▓████   ",
            "       ▒▒▒                  ▒▓▒▒      ",
            "       ▒▒░    ░▓      ▓░    ░▒▒▒      ",
            "       ▒▒░    ▓█      █▓    ░▒▒       ",
            "        ▒░                  ░▒▒       ",
            "         ▒░                ░▒         ",
            "          ▒░░            ░░▒          ",
            "            ▒░░       ░░░▒            ",
            "                 ░░░░                 ",
            "                                      " };
            var colormap_string = new string[]
            {
            "                                      ",
            "  ███████                    ███████  ",
            "████████████      ▒▒▒      ███████████",
            "████████   █▓▒▒▒▒▒▒▒▒▒▒▒▒▓█   ████████",
            "█████████▓  ▓▓▒▒▒▒▒▒▒▒▒▒▓▓  ▓█████████",
            "█████████▓▓▓  ▒▒▒    ▒▒▒  ▓▓▓█████████",
            "  ██████▓▓▓▓▒            ▒▓▓▓▓███████ ",
            "   ████▓▓▓▒▒              ▒▓▓▓▓████   ",
            "       ▒▒▒                  ▒▓▒▒      ",
            "       ▒▒▒    ░░      ░░    ▒▒▒▒      ",
            "       ▒▒▒    ░░      ░░    ▒▒▒       ",
            "        ▒▒                  ▒▒▒       ",
            "         ▒▒                ▒▒         ",
            "          ▒▒▒            ▒▒▒          ",
            "            ▒▒▒       ▒▒▒▒            ",
            "                 ▒▒▒▒                 ",
            "                                      "
            };
            colormap = new ConsoleColor[art.Length*art[0].Length];
            for(int i = 0; i< colormap.Length; i++)
            {
                switch (colormap_string[i / colormap_string[0].Length][i % colormap_string[0].Length])
                {
                    case '█':
                        colormap[i] = ConsoleColor.DarkGray; break;
                    case '▓':
                        colormap[i] = ConsoleColor.DarkBlue; break;
                    case '▒':
                        colormap[i] = ConsoleColor.Blue; break;
                    case '░':
                        colormap[i] = ConsoleColor.White; break;
                    case ' ':
                        colormap[i] = ConsoleColor.Black; break;
                }
            }
        }
    }
}
