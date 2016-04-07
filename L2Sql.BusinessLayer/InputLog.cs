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

        public InputLog(String LogName, long Size)
        {
            this.LogName = LogName;
            this.Size = Size;
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