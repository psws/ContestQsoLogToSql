using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace ContestQsoLogToSql.web
{
    //http://stackoverflow.com/questions/1221503/detect-local-sql-server-installation-with-c32-bit-as-well-as-64-bit

    public static class SqlLocalServerHelper
    {
        public static List<String> EnumerateSqlExpressServers()
        {
            List<String> Servers = new List<string>();
            RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            {
                RegistryKey instanceKey = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL", false);
                if (instanceKey != null)
                {
                    foreach (var instanceName in instanceKey.GetValueNames())
                    {
                        Servers.Add(Environment.MachineName + @"\" + instanceName);
                    }
                }
            }
            return Servers;
        }
    }
}
