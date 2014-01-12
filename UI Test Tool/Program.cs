using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.IO;

namespace UI_Test_Tool
{
    public class Program
    {
        #region static variables

        static string path = Path.Combine(Utility.AutomationUtility.baseDir, "GooglePlayPublish.xml");

        static Server server;
        static string ip;
        static Automation automation;
        static Thread automationThread;

        public static Log.Log log;

        public static Rectangle baseRectangle;

        static IntPtr processHandle;
        static string arguments = "--new-window http://www.google.com";

        #endregion

        static void Main(string[] args)
        {
            //http://stackoverflow.com/questions/11472708/get-window-handle-of-chrome-tab-from-within-extension
            //http://stackoverflow.com/questions/18447621/how-to-get-window-handle-int-from-chrome-extension

            log = new Log.Log();
            //StartServer();

            //Read the contents of the specified XML file to our dictionary
            Utility.AutomationUtility.ReadFromXml(path);

            StartProcessChrome();
            SetWindowSizeAndRectangle();

            #region Element test code

            Rectangle testRect = new Rectangle
            {
                X = 845,
                Y = 418,
                Width = 100,
                Height = 28,
                Location = new Point(845,418),
                Size = new Size(100, 28),
            };

            Models.Element element = new Models.Element(baseRectangle, testRect);
            element.MouseClick(Enums.MouseClickType.LeftClick, Models.OffsetReference.AbsoluteCenter);

            System.Threading.Thread.Sleep(5000);

            SendKeys.SendWait("Test");
            SendKeys.SendWait("{ENTER}");

            Console.ReadLine();

            ////middle of the left screen of Erik's desktop
            //int x = ((windowRect.Left - windowRect.Right) / 2);
            //int y = ((windowRect.Bottom - windowRect.Top) / 2);
            //MouseOperations.SetCursorPosition(x, y);
            //log.WriteLine(Log.LogLevel.Info, String.Format("Mousecursor moved to x = {0}, y = {1}", x, y));

            #endregion

        }

        #region start process: chrome

        /// <summary>
        /// Method used to start the process "Chrome" and 
        /// to determine the MainWindowHandle of this process.
        /// </summary>
        public static void StartProcessChrome()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = @"chrome.exe",
                Arguments = arguments,
            };

            //start the process and get the MainWindowHandle of the last started chrome process.
            using (Process chromeProcess = Process.Start(startInfo))
            {
                //Wait a second for Chrome to start
                System.Threading.Thread.Sleep(1000);

                //loop through all processes to find the Chrome process
                foreach (Process p in Process.GetProcesses())
                {
                    if (p.ProcessName == "chrome")
                    {
                        if ((int)p.MainWindowHandle != 0)
                        {
                            //Console.WriteLine(p.MainWindowHandle.ToString());
                            processHandle = p.MainWindowHandle;
                        }
                    }
                }
            }
            //Write the MainWindowHandle to the logfile.
            log.WriteLine(Log.LogLevel.Info, String.Format("Process Chrome started with MainWindowHandle {0}", processHandle.ToString()));
        }

        #endregion

        #region set window size and rectangle

        /// <summary>
        /// Method used to determine the size and rectangle of the process' window
        /// </summary>
        public static void SetWindowSizeAndRectangle()
        {
            //Get the window size using the GetControlSize method from the user32.dll
            Size windowSize = LayoutOperations.GetControlSize(processHandle);

            //Get the window rectangle using the GetWindowRect method from the user32.dll
            LayoutOperations.RECT windowRect;
            LayoutOperations.GetWindowRect(processHandle, out windowRect);

            //Log our results to the logfile
            log.WriteLine(Log.LogLevel.Info, String.Format("Window rectangle ==> Top: {0}, Bottom: {1}, Left: {2}, Right: {3}", windowRect.Top, windowRect.Bottom, windowRect.Left, windowRect.Right));
            log.WriteLine(Log.LogLevel.Info, String.Format("Window size ==> Height: {0}, Width: {1}", windowSize.Height, windowSize.Width));

            //Save the base values of our window in the base rectangle
            baseRectangle = new Rectangle
            {
                X = windowRect.Left,
                Y = windowRect.Top,
                Height = windowRect.Bottom - windowRect.Top,
                Width = windowRect.Right - windowRect.Left,
                Location = new Point(windowRect.Left, windowRect.Top),
                Size = new Size(windowRect.Right - windowRect.Left, windowRect.Bottom - windowRect.Top),
            };
        }

        #endregion

        /// <summary>
        /// Method used to start the server.
        /// The server is used to send commands to the Chrome extension.
        /// </summary>
        public static void StartServer()
        {
            server = new Server();
            server.StartServer();
        }
    }
}
