using ALE.ETLBox;
using ALE.ETLBox.ConnectionManager;
using ALE.ETLBox.ControlFlow;
using ALE.ETLBox.DataFlow;
using ALE.ETLBox.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;

namespace ALE.ETLBoxTest {
    [TestClass]
    public class TestDataFlowBigData {
        public TestContext TestContext { get; set; }
        public string ConnectionStringParameter => TestContext?.Properties["connectionString"].ToString();
        public string DBNameParameter => TestContext?.Properties["dbName"].ToString();

        [ClassInitialize]
        public static void ClassInit(TestContext testContext) {
            TestHelper.RecreateDatabase(testContext);
            ControlFlow.CurrentDbConnection = new SqlConnectionManager(new ConnectionString(testContext.Properties["connectionString"].ToString()));
            CreateSchemaTask.Create("test");
        }

        [TestInitialize]
        public void TestInit() {
            CleanUpSchemaTask.CleanUp("test");
        }

        /*
        * CSVSource (out: string[]) -> DBDestination (in: string[])
        * Table without key columns (HEAP)
        * 1.000.000 Rows with 8007 bytes per Row (8000 bytes data + 7 bytes for sql server) 
        */
        [TestMethod]
        public void BigData_CSV_DB() {
            BigData_CSV_DB(100000);                        
        }

        public void BigData_CSV_DB(int numberOfRows) {
            Stopwatch watch = new Stopwatch();                        
            TableDefinition stagingTable = new TableDefinition("test.Staging", new List<TableColumn>() {
                new TableColumn("Col1", "nchar(1000)", allowNulls: false),
                new TableColumn("Col2", "nchar(1000)", allowNulls: false),
                new TableColumn("Col3", "nchar(1000)", allowNulls: false),
                new TableColumn("Col4", "nchar(1000)", allowNulls: false),
            });
            stagingTable.CreateTable();
            string fileName = "src/DataFlow/BigData_CSV2DB.csv";
            BigDataHelper bigData = new BigDataHelper() {
                FileName = fileName,
                NumberOfRows = numberOfRows,
                TableDefinition = stagingTable
            };
            watch.Start();
            LogTask.Info($"Create .csv file {fileName} with {numberOfRows} Rows");
            bigData.CreateBigDataCSV();
            LogTask.Info($"Needed {watch.Elapsed.TotalMinutes} to create .csv file");
            watch.Reset();
            
            CSVSource source = new CSVSource(fileName);
            DBDestination<string[]> dest = new DBDestination<string[]>(1000) { DestinationTableDefinition = stagingTable };
            source.LinkTo(dest);

            watch.Start();
            source.Execute();
            LogTask.Info($"Needed {watch.Elapsed.TotalMinutes} to read everything into memory (while constantly writing)");
            LogTask.Info($"Already {RowCountTask.Count("test.Staging", RowCountOptions.QuickQueryMode)} inserted into table");
            dest.Wait(); //TODO Wait should be part of source
            LogTask.Info($"Needed {watch.Elapsed.TotalMinutes} to write everything into database");

            Assert.AreEqual(numberOfRows, SqlTask.ExecuteScalar<int>("Check staging table", $"select count(*) from test.Staging"));
        }

    }

}
