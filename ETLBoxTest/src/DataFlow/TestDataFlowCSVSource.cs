using ALE.ETLBox;
using ALE.ETLBox.ConnectionManager;
using ALE.ETLBox.ControlFlow;
using ALE.ETLBox.DataFlow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ALE.ETLBoxTest {
    [TestClass]
    public class TestDataFlowCSVSource {
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
         * Table without key columns
         */
        [TestMethod]
        public void CSV_DB() {
            TableDefinition stagingTable = new TableDefinition("test.Staging", new List<TableColumn>() {
                new TableColumn("Col1", "nvarchar(100)", allowNulls: false),
                new TableColumn("Col2", "int", allowNulls: true)
            });
            stagingTable.CreateTable();
            CSVSource source = new CSVSource("src/DataFlow/Simple_CSV2DB.csv");
            DBDestination<string[]> dest = new DBDestination<string[]>() { DestinationTableDefinition = stagingTable };
            source.LinkTo(dest);

            source.Execute();
            dest.Wait(); 

            Assert.AreEqual(3, SqlTask.ExecuteScalar<int>("Check staging table", $"select count(*) from test.Staging where Col1 Like '%ValueRow%' and Col2 <> 1"));
        }

        /*
         * CSVSource (out: string[]) -> DBDestination (in: string[])
         * Table with key column (at different positions)
         */
        [TestMethod]
        public void CSV_DB_WithKeyPosition1() {
            CSV_DB_WithKey(0);
        }

        [TestMethod]
        public void CSV_DB_WithKeyPosition2() {
            CSV_DB_WithKey(1);
        }

        [TestMethod]
        public void CSV_DB_WithKeyPosition3() {
            CSV_DB_WithKey(2);
        }

        public void CSV_DB_WithKey(int keyPosition) {
            List<TableColumn> columns = new List<TableColumn>() {
                new TableColumn("Col1", "nvarchar(100)", allowNulls: false),
                new TableColumn("Col2", "int", allowNulls: true) };
            columns.Insert(keyPosition, new TableColumn("Key", "int", allowNulls: false, isPrimaryKey: true) { IsIdentity = true });
            TableDefinition stagingTable = new TableDefinition($"test.Staging{keyPosition}", columns);
            stagingTable.CreateTable();
            CSVSource source = new CSVSource("src/DataFlow/Simple_CSV2DB.csv");
            DBDestination<string[]> dest = new DBDestination<string[]>() { DestinationTableDefinition = stagingTable };
            source.LinkTo(dest);

            source.Execute();
            dest.Wait(); 

            Assert.AreEqual(3, SqlTask.ExecuteScalar<int>("Check staging table", $"select count(*) from test.Staging{keyPosition} where Col1 Like '%ValueRow%' and Col2 <> 1"));
        }

        /*
         * CSVSource (out: string[]) -> DBDestination (in: string[])
         */
        [TestMethod]
        public void CSV_DB_WithBatchChanges() {
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
    }

}
