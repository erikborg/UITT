using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace UI_Test_Tool.Log
{
    public class Log
    {
        string logFile;

        /// <summary>
        /// Constructs a new instance of Log, creating a new logfile in the process.
        /// </summary>
        public Log()
        {
            this.logFile = Path.Combine(Directory.GetCurrentDirectory(), String.Concat(DateTime.Now.ToString("yyyyMMddHHmmss"), "_logfile.txt"));
        }

        /// <summary>
        /// Method used to write a line in our current logfile
        /// </summary>
        /// <param name="level">Log level: Info, Warning or Error</param>
        /// <param name="message">Log message</param>
        public void WriteLine(LogLevel level, string message)
        {
            message = String.Concat(DateTime.Now.ToString(), " -- ", message);

            using (StreamWriter writer = new StreamWriter(logFile, true))
            {
                writer.WriteLine();
                writer.WriteLine(level.ToString());
                writer.WriteLine(message);
            }
        }
    }
}
