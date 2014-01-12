using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI_Test_Tool.UI_Operations
{
    class KeyboardOperations
    {
        public static void TypeText(string text)
        {
            Program.log.WriteLine(Log.LogLevel.Info, String.Format("Typing text: {0}", text));
            SendKeys.SendWait(text);
        }

        public static void PressEnter()
        {
            Program.log.WriteLine(Log.LogLevel.Info, "Pressing key: Enter");
            SendKeys.Send("{ENTER}");
        }
    }
}
