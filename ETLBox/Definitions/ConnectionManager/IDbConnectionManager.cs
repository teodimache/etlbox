using System.Data;

namespace ALE.ETLBox {
    public interface IDbConnectionManager : IConnectionManager  {      
        int ExecuteNonQuery(string command);
        object ExecuteScalar(string command);
        IDataReader ExecuteReader(string command);
        void BulkInsert(IDataReader data, IColumnMappingCollection columnMapping, string tableName);
        IDbConnectionManager Clone();
    }
}
