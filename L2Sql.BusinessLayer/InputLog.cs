//http://stackoverflow.com/questions/8911026/multicolumn-listbox-in-wpf

using System;
using System.Collections.ObjectModel;

namespace L2Sql.BusinessLayer
{
    //http://stackoverflow.com/questions/8911026/multicolumn-listbox-in-wpf

    public class InputLog
    {
        public String LogName { get; set; }
        public long Size { get; set; }
        public int LogCount { get; set; }
        public string Time{ get; set; }

        public InputLog(String LogName, long Size, int LogCount, string Time)
        {
            this.LogName = LogName;
            this.Size = Size;
            this.LogCount = LogCount;
            this.Time = Time;
        }

    }

    public class InputLogs : ObservableCollection<InputLog>
    {
        public InputLogs()
        {
            //Add(new InputLog("cn2aa.log",2048));
            //Add(new InputLog("cn2r.log",108990));
        }


    }
}