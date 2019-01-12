using ALE.ETLBox;
using ALE.ETLBox.ConnectionManager;
using ALE.ETLBox.ControlFlow;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ALE.ETLBoxTest
{
    public static class ConnectionManagerHelper
    {
        public static void AssertOpenConnectionCount(int allowedOpenConnections, string connectionString)
        {
            SqlConnectionString conString = new SqlConnectionString(connectionString);
            SqlConnectionManager master = new SqlConnectionManager(conString.GetMasterConnection());
            string dbName = conString.SqlConnectionStringBuilder.InitialCatalog;
            int? openConnections =
                new SqlTask("Count open connections",
                $@"SELECT COUNT(dbid) as NumberOfConnections FROM sys.sysprocesses
                    WHERE dbid > 0 and DB_NAME(dbid) = '{dbName}'") 
                { ConnectionManager = master, DisableLogging = true }
                .ExecuteScalar<int>()
                .Value;
            Assert.AreEqual(allowedOpenConnections, openConnections);
        }
    }
}
