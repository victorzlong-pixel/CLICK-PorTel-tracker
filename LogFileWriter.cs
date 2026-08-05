using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GS_Tracking_KR
{
    public class LogFileWriter // This class creates a log that can be passed between forms.
    {
        private StreamWriter sw;
        public LogFileWriter()
        {
            DateTime t = DateTime.UtcNow;
            string fn = "Log" + t.ToString("yyyyMMdd_HHmmss.ff") + ".txt";
            sw = new StreamWriter(fn, true);
        }

        public void CloseLog()
        {
            sw.Close();
        }

        public void WriteLogLine(string _msg)
        {
            string str = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss.ff") + ": " + _msg;
            sw.WriteLine(str);
            return;
        }
    }
}
