using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Sys = Cosmos.System;
using XyBeeDOS.BuiltinApps;
using XyBeeDOS.Drivers;

namespace XyBeeDOS
{
    public class Kernel : Sys.Kernel
    {
        public static string GetVersion()
        {
            return "0.0.2";
        }

        protected override void BeforeRun()
        {
            TerminalDriver.Init();

            FileSystem.Init();

            TerminalCommandManager.Init();
            AppManager.Init();
        }

        protected override void Run()
        {
            AppManager.MainLoop();
        }
    }
}
