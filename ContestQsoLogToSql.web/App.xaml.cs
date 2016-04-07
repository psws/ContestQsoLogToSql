using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AssemblyInfo;
using RegUtilLib;

namespace ContestQsoLogToSql.web
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App : Application
    {
        public string AppCompanyName { get; set; }
        public string AppProductName { get; set; }
        public string AppProductVersion { get; set; }
        public string AppDescription { get; set; }
        public string AppTitle { get; set; }
        public string AppRegSubkey { get; set; }
        public RegUtilLib.AppRegistryHKLM AppRegHKLM { get; set; }

        public void InitApplication()
        {
            //set trace level
            CallingAssemblyInfo CallingAssemblyInfo = new AssemblyInfo.CallingAssemblyInfo(4);
            AppCompanyName = CallingAssemblyInfo.Company;
            AppTitle = CallingAssemblyInfo.Title;

            AppRegSubkey = string.Format(@"Software\{0}\{1}", AppCompanyName, AppTitle);
            AppRegHKLM = new RegUtilLib.AppRegistryHKLM(AppRegSubkey);



        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // Perform tasks at application exit
        }


    }
}
