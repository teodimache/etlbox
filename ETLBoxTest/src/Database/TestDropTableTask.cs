using ALE.ETLBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ALE.ETLBoxTest {
    [TestClass]
    public class TestDropTableTask {
        public TestContext TestContext { get; set; }
        public string ConnectionStringParameter => TestContext?.Properties["connectionString"].ToString();
        public string DBNameParameter => TestContext?.Properties["dbName"].ToString();

        [ClassInitialize]
        public static void TestInit(TestContext testContext) {
            TestHelper.RecreateDatabase(testContext);
            ControlFlow.CurrentDbConnection = new SqlConnectionManager(new ConnectionString(testContext.Properties["connectionString"].ToString()));
            CreateSchemaTask.Create("test");
        }

        [TestMethod]
        public void TestDropTable() {
            List<TableColumn> columns = new List<TableColumn>() { new TableColumn("value", "int") };
            CreateTableTask.Create("test.Table1", columns);
            Assert.IsTrue(SqlTask.ExecuteScalarAsBool("Check if table exists", $"select count(*) from sys.objects where type = 'U' and object_id = object_id('test.Table1')"));
            DropTableTask.Drop("test.Table1");
            Assert.IsFalse(SqlTask.ExecuteScalarAsBool("Check if table exists", $"select count(*) from sys.objects where type = 'U' and object_id = object_id('test.Table1')"));

        }

        [TestMethod]
        public void TestLogging() {
            CreateLogTablesTask.CreateLog();
            CreateTableTask.Create("test.Table8", new List<TableColumn>() { new TableColumn("value", "int") });
            DropTableTask.Drop("test.Table8");
            Assert.AreEqual(2, new SqlTask("Find log entry", "select count(*) from etl.Log where TaskType='DROPTABLE' group by TaskHash") { DisableLogging = true }.ExecuteScalar<int>());
        }

    }
}
