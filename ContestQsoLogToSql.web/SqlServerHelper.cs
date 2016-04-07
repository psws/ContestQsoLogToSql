using System;
using System.Collections.Generic;
using System.Data.Sql;
using System.Data;
using System.Data.SqlClient;

namespace ContestQsoLogToSql.web
{
    public static class SqlServerHelper
    {
        public static List<String> EnumerateServers()
        {
            // NO LOCAL DB instamces are retuned
            // see https://msdn.microsoft.com/en-us/library/hh245842(v=sql.120).aspx

            var instances = SqlDataSourceEnumerator.Instance.GetDataSources();
            //instances = SqlDataSourceEnumerator.Instance.GetDataSources();
            if ((instances == null) || (instances.Rows.Count < 1)) return null;

            var result = new List<String>();
            foreach (DataRow instance in instances.Rows)
            {
                //if ((string)instance["ServerName"] != Environment.MachineName)
                //{//SqlLocalServerHelper findx servers on the running machine
                    var serverName = instance["ServerName"].ToString();
                    var instanceName = instance["InstanceName"].ToString();
                    result.Add(String.IsNullOrEmpty(instanceName) ? serverName : String.Format(@"{0}\{1}", serverName, instanceName));
                //}
            }
            return result;
        }

        public static List<String> EnumerateDatabases(String connectionString)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var databases = connection.GetSchema("Databases");
                    connection.Close();
                    if ((databases == null) || (databases.Rows.Count < 1)) return null;

                    var result = new List<String>();
                    foreach (DataRow database in databases.Rows)
                    {
                        result.Add(database["database_name"].ToString());
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static List<String> EnumerateDatabaseTableNames(String connectionString )
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    List<string> result = null;
                    connection.Open();
                    DataTable Tables = connection.GetSchema("Tables");
                    connection.Close();
                    if (!(Tables == null) || (Tables.Rows.Count < 1))
                    {
                        result = new List<String>();
                        foreach (DataRow row in Tables.Rows)
                        {
                            result.Add(row["table_name"].ToString());
                        }
                    }
                    else
                    {
                        //SQlDialogMessageTextBlock.Te
                    }



                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }

}