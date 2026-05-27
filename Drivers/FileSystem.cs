using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys = Cosmos.System;

namespace XyBeeDOS.Drivers
{
    internal class FileSystem
    {
        static readonly int minPartitionSizeMB = 1024;

        static Sys.FileSystem.CosmosVFS fs;

        public static Sys.FileSystem.CosmosVFS GetFS() { return fs; }

        public static void Init()
        {
            fs = new Sys.FileSystem.CosmosVFS();

            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);

            if (!EnsureDiskIsReady(fs))
            {
                Console.WriteLine("Press Enter to reboot...");
                Console.ReadLine(); Sys.Power.Reboot();
            }

            Console.Clear();
        }

        public static string formatPath(string path)
        {
            var is_disk = false;
            var _path = path.Replace("/", @"\").Replace(@"\\", @"\");
            if (_path.Contains(":"))
            {
                is_disk = true;
            }

            if (!is_disk)
            {
                string fullpath = Directory.GetCurrentDirectory().Replace("/", @"\").Replace(@"\\", @"\");

                if (fullpath[fullpath.Length - 1].Equals(@"\")) fullpath += _path;
                else fullpath += @"\" + _path;

                _path = fullpath;
            }

            var dirs = _path.Split(@"\");
            for (int i = 0; i < dirs.Length; i++)
            {
                if (dirs.Length == 1) break;
                if (dirs[i] == "..")
                {
                    dirs[i] = "";
                    dirs[i - 1] = "";
                }
                else if (dirs[i] == ".")
                {
                    dirs[i] = "";
                }
            }
            var formattedPath = dirs[0];
            for (int i = 1; i < dirs.Length; i++)
            {
                if (dirs[i] != "") formattedPath += @"\" + dirs[i];
            }

            return formattedPath;
        }

        private static bool EnsureDiskIsReady(Sys.FileSystem.CosmosVFS fs)
        {
            Console.WriteLine("Проверка диска 0:\\...");

            try
            {
                fs.GetFileSystemType(@"0:\");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Диск 0:\\ не распознан: {ex.Message}");
                Console.WriteLine("Требуется форматирование. Нажмите Enter для продолжения...");
                Console.ReadLine();

                if (fs.Disks.Count == 0)
                {
                    Console.WriteLine("Ошибка: физические диски не найдены.");
                    return false;
                }

                try
                {
                    fs.Disks[0].Clear();

                    fs.Disks[0].CreatePartition(minPartitionSizeMB);

                    fs.Disks[0].FormatPartition(0, "FAT32", true);

                    Console.WriteLine("Форматирование успешно завершено!");
                }
                catch (Exception formatEx)
                {
                    Console.WriteLine($"Ошибка форматирования: {formatEx.Message}");
                }
                return false;
            }

            try
            {
                Directory.GetFiles(@"0:\");
                fs.GetTotalSize(@"0:\");
                fs.GetAvailableFreeSpace(@"0:\");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Диск 0:\\ повреждён или содержит ошибки: {ex.Message}");
                return false;
            }

            return true;
        }
    }
}
