using ALE.ETLBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ALE.ETLBoxTest {
    [TestClass]
    public class TestXMLATask {
        public TestContext TestContext { get; set; }

        [ClassInitialize]
        public static void TestInit(TestContext testContext) {
            string connectionString = testContext.Properties["connectionString"].ToString();
            ControlFlow.CurrentDbConnection = new AdomdConnectionManager(new ConnectionString(connectionString).GetConnectionWithoutCatalog());
        }


        [TestMethod]
        public void TestCreateDelete() {
            string dbName = TestContext.Properties["dbName"].ToString();
            try {
                XmlaTask.ExecuteNonQuery("Drop cube", TestHelper.DeleteCubeXMLA(dbName));
            }
            catch { }
            XmlaTask.ExecuteNonQuery("Create cube", TestHelper.CreateCubeXMLA(dbName));
            XmlaTask.ExecuteNonQuery("Delete cube", TestHelper.DeleteCubeXMLA(dbName));
        }


    }
}
