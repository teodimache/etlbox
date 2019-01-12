using System.Data;
using System.Data.Odbc;
using System.Text;
using System.Linq;

namespace ALE.ETLBox.ConnectionManager
{
    /// <summary>
    /// Connection manager for an ODBC connection based on ADO.NET. ODBC can be used to connect to any ODBC able endpoint.
    /// ODBC by default does not support a Bulk Insert - inserting big amoutns of data is translated into 
    /// insert into (...) values (..),(..),(..) statementes.
    /// </summary>
    /// <example>
    /// <code>
    /// ControlFlow.CurrentDbConnection = new SqlConnectionManager(new ConnectionString("Data Source=.;"));
    /// </code>
    /// </example>
    public class OdbcConnectionManager : DbConnectionManager<OdbcConnection, OdbcCommand>
    {

        public OdbcConnectionManager() : base() { }

        public OdbcConnectionManager(OdbcConnectionString connectionString) : base(connectionString) { }

        public override void BulkInsert(IDataReader data, IColumnMappingCollection columnMapping, string tableName)
        {
            //    foreach (IColumnMapping colMap in columnMapping)
            //        bulkCopy.ColumnMappings.Add(colMap.SourceColumn, colMap.DataSetColumn);
            StringBuilder sb = new StringBuilder();
            sb.Append($"insert into {tableName} (");
            foreach (IColumnMapping c in columnMapping) {
                sb.Append($"{c.SourceColumn},");
            }
            sb.Remove(sb.Length - 1,1);
            sb.Append(") ");
            while (data.Read()) {
                using (var insertCommand = new OdbcCommand()) {
                    object[] r = new object[columnMapping.Count];
                    data.GetValues(r);
                    insertCommand.CommandText = "";
                    //insertCommand.Parameters.Add()
                }
            }
           
        }

        public override IDbConnectionManager Clone()
        {
            OdbcConnectionManager clone = new OdbcConnectionManager((OdbcConnectionString)ConnectionString)
            {
                MaxLoginAttempts = this.MaxLoginAttempts
            };
            return clone;
        }


    }
}
