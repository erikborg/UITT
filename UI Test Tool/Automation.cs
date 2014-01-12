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

        }
    }
}
