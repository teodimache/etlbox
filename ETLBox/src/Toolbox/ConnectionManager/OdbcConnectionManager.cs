using System.Data;
using System.Data.Odbc;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System;

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

            List<string> sourceColumnNames = columnMapping.Cast<IColumnMapping>().Select(cm => cm.SourceColumn).ToList();
            List<string> destColumnNames = columnMapping.Cast<IColumnMapping>().Select(cm => cm.DataSetColumn).ToList();
            StringBuilder sb = new StringBuilder();
            sb.Append($"insert into {tableName} ({string.Join(",",sourceColumnNames)}) values ");
            while (data.Read()) {
                List<string> values = new List<string>();
                foreach (string destColumnName in destColumnNames) {
                    int colIndex = data.GetOrdinal(destColumnName);
                    string dataTypeName = data.GetDataTypeName(colIndex);
                    if (data.IsDBNull(colIndex))
                        values.Add("NULL");
                    else
                        values.Add($"'{data.GetString(colIndex)}'");

                }              
                sb.Append("("+string.Join(",", values)+")" );
                if (data.NextResult())
                    sb.Append("," + Environment.NewLine);
            }

            var cmd = DbConnection.CreateCommand();
            cmd.CommandText = sb.ToString();
            cmd.ExecuteNonQuery(); 
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
