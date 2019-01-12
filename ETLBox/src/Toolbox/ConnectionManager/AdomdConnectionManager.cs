using Microsoft.AnalysisServices.AdomdClient;
using System;
using System.Data;

namespace ALE.ETLBox.ConnectionManager {
    /// <summary>
    /// Connection manager for Adomd connection to a sql server analysis server.
    /// </summary>
    /// <example>
    /// <code>
    /// ControlFlow.CurrentDbConnection = new AdmoConnectionManager(new ConnectionString("..connection string.."));
    /// </code>
    /// </example>
    public class AdomdConnectionManager : DbConnectionManager<AdomdConnection, AdomdCommand> {

        public AdomdConnectionManager() : base() { }

        public AdomdConnectionManager(SqlConnectionString connectionString) : base(connectionString) { }

        public override void BulkInsert(IDataReader data, IColumnMappingCollection columnMapping, string tableName) {
            throw new NotImplementedException();
        }

        public override IDbConnectionManager Clone() {
            AdomdConnectionManager clone = new AdomdConnectionManager((SqlConnectionString)ConnectionString) {
                MaxLoginAttempts = this.MaxLoginAttempts
            };
            return clone;

        }

    }
}
