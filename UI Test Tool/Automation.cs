using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI_Test_Tool
{
    class Automation
    {
        Server server;
        int messageCount;
        Log.Log log;

        public Automation(Server server, Log.Log log)
        {
            this.server = server;
            this.log = log;

            messageCount = 1;
        }

        public void Run()
        {
            //send commands to chrome extension.

            while (true)
            {
                if (server.clientList.Count != 0 && server.sendmessage)
                {
                    string message = "command";

                    log.WriteLine(Log.LogLevel.Info, "Sending message number: " + messageCount);
                    log.WriteLine(Log.LogLevel.Info, "Message content: " + message);
                    Console.WriteLine("Sending message number: " + messageCount);
                    Console.WriteLine("Message content: " + message);

                    server.sendString(message);
                    messageCount++;
                    System.Threading.Thread.Sleep(5000);
                }

                //upon receiving a message, set sendmessage = true. 
                //this will make the server send a new message.
            }
        }
    }
}
