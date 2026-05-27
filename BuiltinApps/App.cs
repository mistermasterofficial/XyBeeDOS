using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XyBeeDOS.BuiltinApps
{
    interface App
    {
        int BeforeRun();
        int Run();
        int Quit();
    }

    class AppManager
    {
        static int runStatus = 0;
        static Stack<App> apps = new Stack<App>();
        static Queue<App> new_apps_queue = new Queue<App>();
        static Dictionary<string,string> sharedSpace = new Dictionary<string,string>();

        static bool is_running = false;
        static App runningApp = null;

        public static void Init()
        {
            startApp(new TerminalApp(@"0:\"));
            apps.Push(new_apps_queue.Dequeue());
        }

        public static int startApp(App app) 
        {
            int status;
            try
            {
                status = app.BeforeRun();
            }
            catch { status = -1; }
            if (status == 0) new_apps_queue.Enqueue(app);
            return status;
        }

        static int runApp()
        {
            runningApp = apps.Pop();
            is_running = true;

            int status;
            try
            {
                status = runningApp.Run();
            }
            catch { status = -1; }

            if (status == 0 && is_running)
            {
                apps.Push(runningApp);
            }

            for(int i = 0; i < new_apps_queue.Count; i++)
            {
                apps.Push(new_apps_queue.Dequeue());
            }

            return status;
        }

        public static bool isPaused()
        {
            return new_apps_queue.Count > 0;
        }

        public static bool isRunning()
        {
            return is_running;
        }

        public static int quitApp()
        {
            int status;
            try
            {
                status = runningApp.Quit();
            }
            catch { status = -1; }
            is_running = false;
            return status;
        }

        public static string getValue(string key)
        {
            string value = "";
            if (sharedSpace.ContainsKey(key))
            {
                value = sharedSpace.GetValueOrDefault(key, "");
                sharedSpace.Remove(key);
            }
            return value;
        }

        public static void setValue(string key, string value)
        {
            sharedSpace.Add(key, value);
        }

        public static int getErrors()
        {
            int status = runStatus;
            runStatus = 0;
            return status;
        }

        public static void MainLoop()
        {
            int status = runApp();
            if (runStatus == 0) runStatus = status;
        }
    }
}
