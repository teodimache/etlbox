using ALE.ETLBox;
using ALE.ETLBox.ConnectionManager;
using ALE.ETLBox.ControlFlow;
using ALE.ETLBox.DataFlow;
using ALE.ETLBox.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ALE.ETLBoxTest {
    [TestClass]
    public class TestOdbcConnectionManager {
        public TestContext TestContext { get; set; }
        public string ConnectionStringParameter => TestContext?.Properties["connectionString"].ToString();
        public string OdbcConnectionStringParameter => TestContext?.Properties["odbcConnectionString"].ToString();
        public string DBNameParameter => TestContext?.Properties["dbName"].ToString();

        [ClassInitialize]
        public static void TestInit(TestContext testContext) {
            TestHelper.RecreateDatabase(testContext);
            ControlFlow.CurrentDbConnection = new OdbcConnectionManager(new OdbcConnectionString(testContext.Properties["odbcConnectionString"].ToString()));
        }

        [TestMethod]
        public void TestSqlTaskWithOdbcConnection() {

            OdbcConnectionManager con = new OdbcConnectionManager(new OdbcConnectionString(OdbcConnectionStringParameter));
            new SqlTask($"Test statement", $@"
                    CREATE TABLE dbo.test (
                        Col1 nvarchar(50)
                    )
                    INSERT INTO dbo.test values('Lorem ipsum Lorem ipsum Lorem ipsum Lorem') ") {
                ConnectionManager = con,
                DisableLogging = true
            }.ExecuteNonQuery();
        }

        [TestMethod]
        public void CSV_DB_WithOdbcConnection() {
            CreateSchemaTask.Create("test");
            TableDefinition stagingTable = new TableDefinition("test.Staging", new List<TableColumn>() {
                new TableColumn("Col1", "nvarchar(100)", allowNulls: false),
                new TableColumn("Col2", "int", allowNulls: true)
            });
            stagingTable.CreateTable();
            CSVSource source = new CSVSource("src/DataFlow/Simple_CSV2DB.csv");
            DBDestination<string[]> dest = new DBDestination<string[]>(batchSize: 2) {
                DestinationTableDefinition = stagingTable,
                BeforeBatchWrite =
                rowArray => {
                    rowArray[0][0] = "NewValue";
                    return rowArray;
                }
            };
            source.LinkTo(dest);

            source.Execute();
            dest.Wait();

            Assert.AreEqual(1, SqlTask.ExecuteScalar<int>("Check staging table", $"select count(*) from test.Staging where Col1 Like '%ValueRow%' and Col2 <> 1"));
            Assert.AreEqual(2, SqlTask.ExecuteScalar<int>("Check staging table", $"select count(*) from test.Staging where Col1 = 'NewValue'"));
        }

        [TestMethod]
        public void TestLoggingWithOdbc() {
            //Logging currently not supported in nlog with .net core and odbc
            CreateLogTablesTask.CreateLog();
            LogTask.Info("Info");
            //Assert.AreEqual(1, SqlTask.ExecuteScalar<int>("Check if default log works", "select count(*) from etl.Log where Message in ('Error','Warn','Info')"));
        }


    }
}
