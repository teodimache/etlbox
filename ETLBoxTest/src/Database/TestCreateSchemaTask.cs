using ALE.ETLBox;
using ALE.ETLBox.ConnectionManager;
using ALE.ETLBox.ControlFlow;
using ALE.ETLBox.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ALE.ETLBoxTest {
    [TestClass]
    public class TestCreateSchemaTask {
        public TestContext TestContext { get; set; }
        public string ConnectionStringParameter => TestContext?.Properties["connectionString"].ToString();
        public string DBNameParameter => TestContext?.Properties["dbName"].ToString();

        [ClassInitialize]
        public static void TestInit(TestContext testContext) {
            TestHelper.RecreateDatabase(testContext);
            ControlFlow.CurrentDbConnection = new SqlConnectionManager(new SqlConnectionString(testContext.Properties["connectionString"].ToString()));
        }
      
        [TestMethod]
        public void TestCreateSchema() {
            string schemaName = "s" + TestHelper.RandomString(9);
            CreateSchemaTask.Create(schemaName);
            Assert.IsTrue(SqlTask.ExecuteScalarAsBool("Check if schema exists", $"select count(*) from sys.schemas where schema_name(schema_id) = '{schemaName}'"));
         
        }

        [TestMethod]
        public void TestLogging() {
            CreateLogTablesTask.CreateLog();
            CreateSchemaTask.Create("s" + TestHelper.RandomString(9));
            Assert.AreEqual(2, new SqlTask("Find log entry", "select count(*) from etl.Log where TaskType='CREATESCHEMA' group by TaskHash") { DisableLogging = true }.ExecuteScalar<int>());
        }


    }
}
