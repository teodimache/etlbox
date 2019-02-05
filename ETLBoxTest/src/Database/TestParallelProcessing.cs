using ALE.ETLBox;
using ALE.ETLBox.ConnectionManager;
using ALE.ETLBox.ControlFlow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ALE.ETLBoxTest {
    [TestClass]
    public class TestParallelProcessing {
        public TestContext TestContext { get; set; }
        public string ConnectionStringParameter => TestContext?.Properties["connectionString"].ToString();
        public string DBNameParameter => TestContext?.Properties["dbName"].ToString();

        [ClassInitialize]
        public static void TestInit(TestContext testContext) {
            TestHelper.RecreateDatabase(testContext);
            
        }
      
        [TestMethod]
        public void TestFastExecutingSqlTaskInParallel() {
            ControlFlow.CurrentDbConnection = new SqlConnectionManager(new ConnectionString(TestContext.Properties["connectionString"].ToString()));
            List<int> array = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            Parallel.ForEach(array, new ParallelOptions { MaxDegreeOfParallelism = 8 },
                curNr => SqlTask.ExecuteNonQuery($"Test statement {curNr}", $"select 1")
             );
        }

        [TestMethod]
        public void TestLongExecutingSqlTaskInParallel() {
            ControlFlow.CurrentDbConnection = new SqlConnectionManager(new ConnectionString(TestContext.Properties["connectionString"].ToString()));
            List<int> array = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            Parallel.ForEach(array, new ParallelOptions { MaxDegreeOfParallelism = 8 },
                curNr => SqlTask.ExecuteNonQuery($"Test statement {curNr}", $@"
                    DECLARE @counter INT = 0;
                    CREATE TABLE dbo.test{curNr} (
                        Col1 nvarchar(50)
                    )
                    WHILE @counter <= 50000
                    BEGIN
                        SET @counter = @counter + 1;
                        INSERT INTO dbo.test{curNr} values('Lorem ipsum Lorem ipsum Lorem ipsum Lorem')
                    END
                    DROP TABLE dbo.test{curNr};
                ")
             );
        }
    }
}
