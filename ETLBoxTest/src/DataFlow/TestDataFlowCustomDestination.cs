using ALE.ETLBox;
using ALE.ETLBox.ConnectionManager;
using ALE.ETLBox.ControlFlow;
using ALE.ETLBox.DataFlow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ALE.ETLBoxTest {
    [TestClass]
    public class TestDataFlowCustomDestination {
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

        public class MySimpleRow {
            public string Value1 { get; set; }
            public int Value2 { get; set; }
        }

        /*
         * DSBSource (out: object) -> CustomDestination (in: object)
         */
        [TestMethod]
        public void DB_CustDest() {
            TableDefinition sourceTableDefinition = CreateSourceTable("test.Source");
            TableDefinition destinationTableDefinition = CreateDestinationTable("test.Destination");

            DBSource<MySimpleRow> source = new DBSource<MySimpleRow>() { SourceTableDefinition = sourceTableDefinition };
            CustomDestination<MySimpleRow> dest = new CustomDestination<MySimpleRow>(
                row => {
                    SqlTask.ExecuteNonQuery("Insert row", $"insert into test.Destination values('{row.Value1}',{row.Value2})");
                    }
            );
            source.LinkTo(dest);
            source.Execute();
            dest.Wait();
            Assert.AreEqual(3, SqlTask.ExecuteScalar<int>("Check destination table", "select count(*) from test.Destination"));
        }

        private static TableDefinition CreateSourceTable(string tableName) {
            TableDefinition sourceTableDefinition = new TableDefinition(tableName, new List<TableColumn>() {
                new TableColumn("Col1", "nvarchar(100)", allowNulls: false),
                new TableColumn("Col2", "int", allowNulls: true)
            });
            sourceTableDefinition.CreateTable();
            SqlTask.ExecuteNonQuery("Insert demo data", $"insert into {tableName} values('Test1',1)");
            SqlTask.ExecuteNonQuery("Insert demo data", $"insert into {tableName} values('Test2',2)");
            SqlTask.ExecuteNonQuery("Insert demo data", $"insert into {tableName} values('Test3',3)");
            return sourceTableDefinition;
        }

        private static TableDefinition CreateDestinationTable(string tableName) {
            TableDefinition destinationTableDefinition = new TableDefinition(tableName, new List<TableColumn>() {
                new TableColumn("Col1", "nvarchar(100)", allowNulls: false),
                new TableColumn("Col2", "int", allowNulls: true)
            });
            destinationTableDefinition.CreateTable();
            return destinationTableDefinition;
        }

        

    }

}
