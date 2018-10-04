using ALE.ETLBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ALE.ETLBoxTest {
    [TestClass]
    public class TestDataFlowDBSource {
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
         * DSBSource (out: object) -> DBDestination (in: object)
         */
        [TestMethod]
        public void DB_DB() {
            TableDefinition sourceTableDefinition = new TableDefinition("test.Source", new List<TableColumn>() {
                new TableColumn("Col1", "nvarchar(100)", allowNulls: false),
                new TableColumn("Col2", "int", allowNulls: true)
            });
            sourceTableDefinition.CreateTable();
            SqlTask.ExecuteNonQuery("Insert demo data", "insert into test.Source values('Test1',1)");
            SqlTask.ExecuteNonQuery("Insert demo data", "insert into test.Source values('Test2',2)");
            SqlTask.ExecuteNonQuery("Insert demo data", "insert into test.Source values('Test3',3)");

            TableDefinition destinationTableDefinition = new TableDefinition("test.Destination", new List<TableColumn>() {
                new TableColumn("Col1", "nvarchar(100)", allowNulls: false),
                new TableColumn("Col2", "int", allowNulls: true)
            });
            destinationTableDefinition.CreateTable();

            DBSource<MySimpleRow> source = new DBSource<MySimpleRow>() { SourceTableDefinition = sourceTableDefinition };
            DBDestination<MySimpleRow> dest = new DBDestination<MySimpleRow>() { DestinationTableDefinition = destinationTableDefinition };
            source.LinkTo(dest);
            source.Execute();
            dest.Wait();
            Assert.AreEqual(3, SqlTask.ExecuteScalar<int>("Check destination table", "select count(*) from test.Destination"));
        }

        /*
         * DSBSource (out: object) -> DBDestination (in: object)
         */
        [TestMethod]
        public void Sql_DB() {           
            TableDefinition destinationTableDefinition = new TableDefinition("test.Destination", new List<TableColumn>() {
                new TableColumn("Col1", "nvarchar(100)", allowNulls: false),
                new TableColumn("Col2", "int", allowNulls: true)
            });
            destinationTableDefinition.CreateTable();

            DBSource<MySimpleRow> source = new DBSource<MySimpleRow>(
                $@"select * from (values ('Test1',1), ('Test2',2), ('Test',3)) AS MyTable(Col1,Col2)");
            DBDestination<MySimpleRow> dest = new DBDestination<MySimpleRow>() { DestinationTableDefinition = destinationTableDefinition };
            source.LinkTo(dest);
            source.Execute();
            dest.Wait();
            Assert.AreEqual(3, SqlTask.ExecuteScalar<int>("Check destination table", "select count(*) from test.Destination"));
        }


    }

}
