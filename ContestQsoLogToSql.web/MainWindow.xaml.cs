using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using L2Sql.BusinessLayer;

using WpfModalDialog;


namespace ContestQsoLogToSql.web
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public List<string> Files { get; set; }
      
        public MainWindow()
        {
            InitializeComponent();
            ((App)(Application.Current)).InitApplication();
            ModalMsgDialog.SetParent(LayoutRoot);

        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            using (((App)(Application.Current)).AppRegHKLM.rkRun)
            {
                try
                {
                    //ms-help://MS.VSCC.v90/MS.MSDNQTR.v90.en/fxref_mscorlib/html/572761c2-764e-c7a5-9968-ce8e7ffd027c.htm
                    LogsTextBox.Text = ((App)(Application.Current)).AppRegHKLM.rkRun.GetValue("LogSrcDirectory").ToString();
                    if (!string.IsNullOrEmpty(LogsTextBox.Text))
                    {
                        if (!Directory.Exists(LogsTextBox.Text))
                        {
                            LogsTextBox.Text = "";
                            ((App)(Application.Current)).AppRegHKLM.rkRun.DeleteSubKey("LogSrcDirectory");
                        }
                    }
                }
                catch (Exception)
                {
                    //no actioon
                }

                try 
	            {	        
                    CtyTextBox.Text = ((App)(Application.Current)).AppRegHKLM.rkRun.GetValue("CtyFileName").ToString();
                    if (!string.IsNullOrEmpty(CtyTextBox.Text))
                    {
                        if (!File.Exists(CtyTextBox.Text))
                        {
                            CtyTextBox.Text = "";
                            ((App)(Application.Current)).AppRegHKLM.rkRun.DeleteValue("CtyFileName");
                        }
                    }
	            }
	            catch (Exception)
	            {
                    //no actioon
                }

                try
                {
                    string SqlTable = ((App)(Application.Current)).AppRegHKLM.rkRun.GetValue("SqlQsoTable").ToString();
                    if (!string.IsNullOrEmpty(SqlTable))
                    {
                        SqlQsoTablesComboBox.Items.Add(SqlTable);
                        SqlQsoTablesComboBox.SelectedIndex = 0;
                    }

                }
                catch (Exception)
                {
                    //no actioon
                }

                try
                {
                    string SqlDatabase = ((App)(Application.Current)).AppRegHKLM.rkRun.GetValue("SqlDatabase").ToString();
                    if (!string.IsNullOrEmpty(SqlDatabase))
                    {
                        SqlDatabaseCombobox.Items.Add(SqlDatabase);
                        SqlDatabaseCombobox.SelectedIndex = 0;
                    }
                }
                catch (Exception)
                {
                    //no key
                }

                try
                {
                    string SqlServerInstance = ((App)(Application.Current)).AppRegHKLM.rkRun.GetValue("SqlServerInstance").ToString();
                    if (!string.IsNullOrEmpty(SqlServerInstance))
                    {
                        SqlServerInstanceCombobox.Items.Add(SqlServerInstance);
                        SqlServerInstanceCombobox.SelectedIndex = 0;
                    }
                }
                catch (Exception)
                {
                    //no key
                }

            }
        }

        void Main_Closing(object sender, CancelEventArgs e)
        {
            using (((App)(Application.Current)).AppRegHKLM.rkRun)
            {
                try
                {
                    //http://www.codeproject.com/KB/cs/dotnet_registry.aspx
                    // get reference to the HKLM registry key...
                    //ms-help://MS.VSCC.v90/MS.MSDNQTR.v90.en/fxref_mscorlib/html/572761c2-764e-c7a5-9968-ce8e7ffd027c.htm
                    ((App)(Application.Current)).AppRegHKLM.rkRun.SetValue("LogSrcDirectory", LogsTextBox.Text, Microsoft.Win32.RegistryValueKind.String);
                    ((App)(Application.Current)).AppRegHKLM.rkRun.SetValue("CtyFileName", CtyTextBox.Text, Microsoft.Win32.RegistryValueKind.String);
                    if (SqlServerInstanceCombobox.SelectedIndex != -1)
                    {
                        ((App)(Application.Current)).AppRegHKLM.rkRun.SetValue("SqlServerInstance", SqlServerInstanceCombobox.SelectedValue.ToString(), Microsoft.Win32.RegistryValueKind.String);
                    }
                    if (SqlDatabaseCombobox.SelectedIndex != -1)
                    {
                        ((App)(Application.Current)).AppRegHKLM.rkRun.SetValue("SqlDatabase", SqlDatabaseCombobox.SelectedValue.ToString(), Microsoft.Win32.RegistryValueKind.String);
                    }
                    if (SqlQsoTablesComboBox.SelectedIndex != -1)
                    {
                        ((App)(Application.Current)).AppRegHKLM.rkRun.SetValue("SqlQsoTable", SqlQsoTablesComboBox.SelectedValue.ToString(), Microsoft.Win32.RegistryValueKind.String);
                    }
                }
                catch
                {
                    // ....
                }
            }


        }

        //http://stackoverflow.com/questions/4007882/select-folder-dialog-wpf/17712949#17712949
        private void BrowseLogSourceButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "Select Log Source Directory";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = "C:\\";

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = "C:\\";
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (!string.IsNullOrEmpty(LogsTextBox.Text))
            {
                if (Directory.Exists(LogsTextBox.Text))
                {
                    dlg.DefaultDirectory = System.IO.Path.GetDirectoryName(LogsTextBox.Text);
                }
                else
                {
                    LogsTextBox.Text = "";
                }
            }
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                LogsTextBox.Text = dlg.FileName;

                using (((App)(Application.Current)).AppRegHKLM.rkRun)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(LogsTextBox.Text))
                        {
                            ((App)(Application.Current)).AppRegHKLM.rkRun.SetValue("LogSrcDirectory", LogsTextBox.Text, Microsoft.Win32.RegistryValueKind.String);
                        }
                    }
                    catch
                    {
                        // ....
                    }
                }
            }

        }

        private void BrowseCtyFileButton_Click(object sender, RoutedEventArgs e)
        {

            var dlg = new CommonOpenFileDialog();
            dlg.Filters.Add(new CommonFileDialogFilter("Cty Files (*.dat)", ".dat"));
            dlg.Title = "Select Cty file";
            dlg.IsFolderPicker = false;
            dlg.InitialDirectory = "C:\\";
            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = "C:\\";
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;


            if (!string.IsNullOrEmpty(CtyTextBox.Text))
            {
                if (Directory.Exists(CtyTextBox.Text))
                {
                    dlg.DefaultDirectory = System.IO.Path.GetDirectoryName(CtyTextBox.Text);
                }
                else
                {
                    CtyTextBox.Text = "";
                }
            }
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                CtyTextBox.Text = dlg.FileName;

                using (((App)(Application.Current)).AppRegHKLM.rkRun)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(CtyTextBox.Text))
                        {
                            ((App)(Application.Current)).AppRegHKLM.rkRun.SetValue("CtyFileName", CtyTextBox.Text, Microsoft.Win32.RegistryValueKind.String);
                        }
                    }
                    catch
                    {
                        // ....
                    }
                }
            }

        }

        private void BrowseServerInstance_Click(object sender, RoutedEventArgs e)
        {

            // List<string> RegistryServers = SqlLocalServerHelper.EnumerateSqlExpressServers();
            List<string> Servers = SqlServerHelper.EnumerateServers();
            SqlServerInstanceCombobox.ItemsSource = null;
            SqlServerInstanceCombobox.Items.Clear();
            SqlServerInstanceCombobox.ItemsSource = Servers;

            if (SqlServerInstanceCombobox.Items.Count != 0)
            {
                SqlServerInstanceCombobox.IsDropDownOpen = true;
            }

        }



        private void SqlServerInstanceCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void GetInstanceDatabases_Click(object sender, RoutedEventArgs e)
        {
            string Login = SQLLoginTextbox.Text;
            string Pwd = SqlPasswordTextbox.Text;
            if (string.IsNullOrEmpty(Login) == true)
            {//check registry
                try
                {
                    Login = ((App)(Application.Current)).AppRegHKLM.rkRun.GetValue("SqlDatabaseLogin").ToString();
                }
                catch (Exception)
                {
                    // ignore
                }
            }
            if (string.IsNullOrEmpty(Pwd) == true)
            {//check registry
                try
                {
                    Pwd = ((App)(Application.Current)).AppRegHKLM.rkRun.GetValue("SqlDatabasePwd").ToString();
                }
                catch (Exception)
                {
                    // ignore
                }
            }

            string ConnectionString = CreateConnectionString(Login, Pwd);
            if (ConnectionString != null)
            {
                //save user/pwd
                if (string.IsNullOrEmpty(SQLLoginTextbox.Text) == false)
                {
                    ((App)(Application.Current)).AppRegHKLM.rkRun.SetValue("SqlDatabaseLogin", SQLLoginTextbox.Text, Microsoft.Win32.RegistryValueKind.String);
                }
                if (string.IsNullOrEmpty(SqlPasswordTextbox.Text) == false)
                {
                    ((App)(Application.Current)).AppRegHKLM.rkRun.SetValue("SqlDatabasePwd", SqlPasswordTextbox.Text, Microsoft.Win32.RegistryValueKind.String);
                }

                List<string> Databases = SqlServerHelper.EnumerateDatabases(ConnectionString);

                SqlDatabaseCombobox.ItemsSource = null;
                SqlDatabaseCombobox.Items.Clear();

                SqlDatabaseCombobox.ItemsSource = Databases;
                if (SqlDatabaseCombobox.Items.Count != 0)
                {
                    SqlDatabaseCombobox.IsDropDownOpen = true;
                    //SqlDatabaseCombobox.SelectedIndex = 0;
                    //force table load
                    //SqlQsoTablesComboBox.SelectedIndex = -1;

                }
            }

        }



        private void SqlQsoTablesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void GetSqlQsoTables_Click(object sender, RoutedEventArgs e)
        {
            string Login= SQLLoginTextbox.Text;
            string Pwd = SqlPasswordTextbox.Text;

            if (string.IsNullOrEmpty(Login) == true)
            {//check registry
                try
                {
                    Login = ((App)(Application.Current)).AppRegHKLM.rkRun.GetValue("SqlDatabaseLogin").ToString();
                }
                catch (Exception)
                {
                    // ignore
                }
            }
            if (string.IsNullOrEmpty(Pwd) == true)
            {//check registry
                try
                {
                    Pwd = ((App)(Application.Current)).AppRegHKLM.rkRun.GetValue("SqlDatabasePwd").ToString();
                }
                catch (Exception)
                {
                    // ignore
                }
            }

            string ConnectionString = CreateConnectionString(Login, Pwd);
            if (ConnectionString != null)
            {
                if (string.IsNullOrEmpty(ConnectionString) == false)
                {
                    List<string> Tables = SqlServerHelper.EnumerateDatabaseTableNames(ConnectionString);
                    if (Tables.Count != 0)
                    {
                        SqlQsoTablesComboBox.ItemsSource = null;
                        SqlQsoTablesComboBox.Items.Clear();
                        SqlQsoTablesComboBox.ItemsSource = Tables;
                        SqlQsoTablesComboBox.IsDropDownOpen = true;
                    }

                }
            }

        }


        private void SqlDatabaseCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        public string CreateConnectionString(string Login, string Pwd)
        {
            string ConnectionString = null;
            string instance = null; ;

            if ( SqlServerInstanceCombobox.SelectedItem != null )
            {
                instance = SqlServerInstanceCombobox.SelectedItem.ToString();
                if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Pwd))
                {
                    ModalMsgDialog.ShowHandlerDialog(string.Format("Please specify the Login and Password for the {0} instabce", instance));
                }
                else
                {
                    ConnectionString = "Data Source=" + instance + ";";
                    ConnectionString += " User ID=" + Login + "; Password=" + Pwd + ";";
                    string tmp = SqlDatabaseCombobox.SelectedIndex.ToString();
                    if (SqlDatabaseCombobox.SelectedIndex != -1 && string.IsNullOrEmpty(SqlDatabaseCombobox.SelectedValue.ToString()) == false)
                    {
                        ConnectionString += " Initial Catalog=" + SqlDatabaseCombobox.SelectedValue.ToString() + ";";

                    }
                }
            }
            else
            {
                ModalMsgDialog.ShowHandlerDialog(string.Format("Please set the Sql Server Instance by selecting the associated Get button"));
            }
            return ConnectionString;
        }
        private void ImportCalls_Click(object sender, RoutedEventArgs e)
        {
            string instance = null;
            if (SqlServerInstanceCombobox.SelectedValue != null &&
                string.IsNullOrEmpty(SqlServerInstanceCombobox.SelectedValue.ToString()) == false)
            {
                instance = SqlServerInstanceCombobox.SelectedValue.ToString();
                string Login = SQLLoginTextbox.Text;
                string Pwd = SqlPasswordTextbox.Text;

                if (string.IsNullOrEmpty(Login) == true)
                {//check registry
                    try
                    {
                        Login = ((App)(Application.Current)).AppRegHKLM.rkRun.GetValue("SqlDatabaseLogin").ToString();
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }
                if (string.IsNullOrEmpty(Pwd) == true)
                {//check registry
                    try
                    {
                        Pwd = ((App)(Application.Current)).AppRegHKLM.rkRun.GetValue("SqlDatabasePwd").ToString();
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }

                if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Pwd))
                {
                    ModalMsgDialog.ShowHandlerDialog(string.Format("Please specify the Login and Password for the {0} instabce", instance));
                    //MessageBox.Show(string.Format("Unable to create ctyobj {0}", "test"));
                }
                else
                {
                    try
                    {
                        //((InputLogs)(LogListbox.ItemsSource)).Add(new InputLog("cn3a.log", 1024));
                        //((InputLogs)(LogListbox.ItemsSource)).Insert(0, new InputLog("d4b.log", 65536));
#if false
                        ProcessLogs ProcessLogsobj = new ProcessLogs(CtyTextBox.Text, LogsTextBox.Text, (InputLogs)(LogListbox.ItemsSource),
                            SqlServerInstanceCombobox.SelectedValue.ToString(),
                            SqlDatabaseCombobox.SelectedValue.ToString(),
                            SqlQsoTablesComboBox.SelectedValue.ToString());
                        ProcessLogsobj.LogsToDatabase();
#else

                        ProcessLogs ProcessLogsobj = new ProcessLogs(CtyTextBox.Text, LogsTextBox.Text,
                           SqlServerInstanceCombobox.SelectedValue.ToString(),
                           SqlDatabaseCombobox.SelectedValue.ToString(),
                           SqlQsoTablesComboBox.SelectedValue.ToString());
                        //http://stackoverflow.com/questions/5483565/how-to-use-wpf-background-worker
                        BackgroundWorker worker = new BackgroundWorker();
                        worker.WorkerReportsProgress = true;
                        worker.DoWork += worker_DoWorkCall;
                        worker.ProgressChanged += worker_ProgressChanged;
                        worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                        ImportLogs.IsEnabled = false;
                        ImportCalls.IsEnabled = false;
                        CalculateMult_Points.IsEnabled = false;


                        worker.RunWorkerAsync(ProcessLogsobj);
#endif
                    }
                    catch (Exception ex)
                    {
                        ModalMsgDialog.ShowHandlerDialog(string.Format("Unable to create ctyobj {0}", ex.Message));
                        //MessageBox.Show(string.Format("Unable to create ctyobj {0}", ex.Message));
                    }
                }
            }

        }

        private void worker_DoWorkCall(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            //Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    ProcessLogs ProcessLogsobj = new ProcessLogs(CtyTextBox.Text, LogsTextBox.Text,
            //        SqlServerInstanceCombobox.SelectedValue.ToString(),
            //        SqlDatabaseCombobox.SelectedValue.ToString(),
            //        SqlQsoTablesComboBox.SelectedValue.ToString());
            //})); 
            //Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            ProcessLogs ProcessLogsobj = e.Argument as ProcessLogs;
            ProcessLogsobj.CallsToDatabase(worker);

        }

        private void ImportLogs_Click(object sender, RoutedEventArgs e)
        {
            string instance = null;
            if (SqlServerInstanceCombobox.SelectedValue != null &&
                string.IsNullOrEmpty(SqlServerInstanceCombobox.SelectedValue.ToString()) == false )
            {
                instance = SqlServerInstanceCombobox.SelectedValue.ToString();
                string Login = SQLLoginTextbox.Text;
                string Pwd = SqlPasswordTextbox.Text;

                if (string.IsNullOrEmpty(Login) == true)
                {//check registry
                    try
                    {
                        Login = ((App)(Application.Current)).AppRegHKLM.rkRun.GetValue("SqlDatabaseLogin").ToString();
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }
                if (string.IsNullOrEmpty(Pwd) == true)
                {//check registry
                    try
                    {
                        Pwd = ((App)(Application.Current)).AppRegHKLM.rkRun.GetValue("SqlDatabasePwd").ToString();
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }

                if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Pwd))
                {
                    ModalMsgDialog.ShowHandlerDialog(string.Format("Please specify the Login and Password for the {0} instabce", instance));
                    //MessageBox.Show(string.Format("Unable to create ctyobj {0}", "test"));
                }
                else
                {
                    try
                    {
                        //((InputLogs)(LogListbox.ItemsSource)).Add(new InputLog("cn3a.log", 1024));
                        //((InputLogs)(LogListbox.ItemsSource)).Insert(0, new InputLog("d4b.log", 65536));
#if false
                        ProcessLogs ProcessLogsobj = new ProcessLogs(CtyTextBox.Text, LogsTextBox.Text, (InputLogs)(LogListbox.ItemsSource),
                            SqlServerInstanceCombobox.SelectedValue.ToString(),
                            SqlDatabaseCombobox.SelectedValue.ToString(),
                            SqlQsoTablesComboBox.SelectedValue.ToString());
                        ProcessLogsobj.LogsToDatabase();
#else

                        ProcessLogs ProcessLogsobj = new ProcessLogs(CtyTextBox.Text, LogsTextBox.Text,
                           SqlServerInstanceCombobox.SelectedValue.ToString(),
                           SqlDatabaseCombobox.SelectedValue.ToString(),
                           SqlQsoTablesComboBox.SelectedValue.ToString());
                        //http://stackoverflow.com/questions/5483565/how-to-use-wpf-background-worker
                        BackgroundWorker worker = new BackgroundWorker();
                        worker.WorkerReportsProgress = true;
                        worker.DoWork += worker_DoWorkLog;
                        worker.ProgressChanged += worker_ProgressChanged;
                        worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                        ImportLogs.IsEnabled = false;
                        ImportCalls.IsEnabled = false;
                        CalculateMult_Points.IsEnabled = false;

                        worker.RunWorkerAsync(ProcessLogsobj);
#endif
                    }
                    catch (Exception ex)
                    {
                        ModalMsgDialog.ShowHandlerDialog(string.Format("Unable to create ctyobj {0}", ex.Message));
                        //MessageBox.Show(string.Format("Unable to create ctyobj {0}", ex.Message));
                    }

                }
            }
            else
            {
                ModalMsgDialog.ShowHandlerDialog(string.Format("Please set the Sql Server Instance by selecting the associated Get button"));
                //MessageBox.Show(string.Format("Please set the Sql Server Instance by selecting the associated Get button")  );
            }


        }


        private void worker_DoWorkLog(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            //Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    ProcessLogs ProcessLogsobj = new ProcessLogs(CtyTextBox.Text, LogsTextBox.Text,
            //        SqlServerInstanceCombobox.SelectedValue.ToString(),
            //        SqlDatabaseCombobox.SelectedValue.ToString(),
            //        SqlQsoTablesComboBox.SelectedValue.ToString());
            //})); 
             //Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            ProcessLogs ProcessLogsobj = e.Argument as ProcessLogs;
            ProcessLogsobj.LogsToDatabase(worker);

        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
           
            InputLog InputLog = e.UserState as InputLog;
            ((InputLogs)(LogListbox.ItemsSource)).Insert(0, InputLog);

        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ImportLogs.IsEnabled = true;
            ImportCalls.IsEnabled = true;
            CalculateMult_Points.IsEnabled = true;

        }

        private void CalcMults_Click(object sender, RoutedEventArgs e)
        {
            string instance = null;
            if (SqlServerInstanceCombobox.SelectedValue != null &&
                string.IsNullOrEmpty(SqlServerInstanceCombobox.SelectedValue.ToString()) == false )
            {
                instance = SqlServerInstanceCombobox.SelectedValue.ToString();
                string Login = SQLLoginTextbox.Text;
                string Pwd = SqlPasswordTextbox.Text;

                if (string.IsNullOrEmpty(Login) == true)
                {//check registry
                    try
                    {
                        Login = ((App)(Application.Current)).AppRegHKLM.rkRun.GetValue("SqlDatabaseLogin").ToString();
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }
                if (string.IsNullOrEmpty(Pwd) == true)
                {//check registry
                    try
                    {
                        Pwd = ((App)(Application.Current)).AppRegHKLM.rkRun.GetValue("SqlDatabasePwd").ToString();
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }

                if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Pwd))
                {
                    ModalMsgDialog.ShowHandlerDialog(string.Format("Please specify the Login and Password for the {0} instabce", instance));
                    //MessageBox.Show(string.Format("Unable to create ctyobj {0}", "test"));
                }
                else
                {
                    try
                    {

                        ProcessLogs ProcessLogsobj = new ProcessLogs(CtyTextBox.Text, LogsTextBox.Text,
                           SqlServerInstanceCombobox.SelectedValue.ToString(),
                           SqlDatabaseCombobox.SelectedValue.ToString(),
                           SqlQsoTablesComboBox.SelectedValue.ToString());
                        //http://stackoverflow.com/questions/5483565/how-to-use-wpf-background-worker
                        BackgroundWorker worker = new BackgroundWorker();
                        worker.WorkerReportsProgress = true;
                        worker.DoWork += worker_DoWorkMult;
                        worker.ProgressChanged += worker_ProgressChanged;
                        worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                        ImportLogs.IsEnabled = false;
                        ImportCalls.IsEnabled = false;
                        CalculateMult_Points.IsEnabled = false;

                        worker.RunWorkerAsync(ProcessLogsobj);
                    }
                    catch (Exception ex)
                    {
                        ModalMsgDialog.ShowHandlerDialog(string.Format("Unable to create ctyobj {0}", ex.Message));
                        //MessageBox.Show(string.Format("Unable to create ctyobj {0}", ex.Message));
                    }

                }
            }
            else
            {
                ModalMsgDialog.ShowHandlerDialog(string.Format("Please set the Sql Server Instance by selecting the associated Get button"));
                //MessageBox.Show(string.Format("Please set the Sql Server Instance by selecting the associated Get button")  );
            }

        }

        private void worker_DoWorkMult(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            //Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    ProcessLogs ProcessLogsobj = new ProcessLogs(CtyTextBox.Text, LogsTextBox.Text,
            //        SqlServerInstanceCombobox.SelectedValue.ToString(),
            //        SqlDatabaseCombobox.SelectedValue.ToString(),
            //        SqlQsoTablesComboBox.SelectedValue.ToString());
            //})); 
             //Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            ProcessLogs ProcessLogsobj = e.Argument as ProcessLogs;
            ProcessLogsobj.Mults_PointsToDatabase(worker);

        }

        private void CalcUBNFX_Click(object sender, RoutedEventArgs e)
        {
            string instance = null;
            if (SqlServerInstanceCombobox.SelectedValue != null &&
                string.IsNullOrEmpty(SqlServerInstanceCombobox.SelectedValue.ToString()) == false)
            {
                instance = SqlServerInstanceCombobox.SelectedValue.ToString();
                string Login = SQLLoginTextbox.Text;
                string Pwd = SqlPasswordTextbox.Text;

                if (string.IsNullOrEmpty(Login) == true)
                {//check registry
                    try
                    {
                        Login = ((App)(Application.Current)).AppRegHKLM.rkRun.GetValue("SqlDatabaseLogin").ToString();
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }
                if (string.IsNullOrEmpty(Pwd) == true)
                {//check registry
                    try
                    {
                        Pwd = ((App)(Application.Current)).AppRegHKLM.rkRun.GetValue("SqlDatabasePwd").ToString();
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }

                if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Pwd))
                {
                    ModalMsgDialog.ShowHandlerDialog(string.Format("Please specify the Login and Password for the {0} instabce", instance));
                    //MessageBox.Show(string.Format("Unable to create ctyobj {0}", "test"));
                }
                else
                {
                    try
                    {

                        ProcessUBNDX ProcessUbndxObj = new ProcessUBNDX( LogsTextBox.Text,
                           SqlServerInstanceCombobox.SelectedValue.ToString(),
                           SqlDatabaseCombobox.SelectedValue.ToString(),
                           SqlQsoTablesComboBox.SelectedValue.ToString());
                        //http://stackoverflow.com/questions/5483565/how-to-use-wpf-background-worker
                        BackgroundWorker worker = new BackgroundWorker();
                        worker.WorkerReportsProgress = true;
                        worker.DoWork += worker_DoWorkUBNDX;
                        worker.ProgressChanged += worker_ProgressChanged;
                        worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                        ImportLogs.IsEnabled = false;
                        ImportCalls.IsEnabled = false;
                        CalculateMult_Points.IsEnabled = false;

                        worker.RunWorkerAsync(ProcessUbndxObj);
                    }
                    catch (Exception ex)
                    {
                        ModalMsgDialog.ShowHandlerDialog(string.Format("Unable to create ctyobj {0}", ex.Message));
                        //MessageBox.Show(string.Format("Unable to create ctyobj {0}", ex.Message));
                    }

                }
            }
            else
            {
                ModalMsgDialog.ShowHandlerDialog(string.Format("Please set the Sql Server Instance by selecting the associated Get button"));
                //MessageBox.Show(string.Format("Please set the Sql Server Instance by selecting the associated Get button")  );
            }

        }


        private void worker_DoWorkUBNDX(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            //Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    ProcessLogs ProcessLogsobj = new ProcessLogs(CtyTextBox.Text, LogsTextBox.Text,
            //        SqlServerInstanceCombobox.SelectedValue.ToString(),
            //        SqlDatabaseCombobox.SelectedValue.ToString(),
            //        SqlQsoTablesComboBox.SelectedValue.ToString());
            //})); 
            //Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            ProcessUBNDX ProcessUBNDXobj = e.Argument as ProcessUBNDX;
            ProcessUBNDXobj.UbndxToDatabase(worker);

        }






    }


   
}
