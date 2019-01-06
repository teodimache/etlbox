using System.Data.SqlClient;

namespace ALE.ETLBox {
    /// <summary>
    /// A helper class for encapsulating a conection string in an object.
    /// Internally the SqlConnectionStringBuilder is used to access the values of the given connection string.
    /// </summary>
    public class ConnectionString {

        SqlConnectionStringBuilder _builder; 

        public string Value {
            get {
                return _builder?.ConnectionString.Replace("Integrated Security=true", "Integrated Security=SSPI", System.StringComparison.InvariantCultureIgnoreCase);
            }
            set {
                _builder = new SqlConnectionStringBuilder(value);
            }
        }

        public SqlConnectionStringBuilder SqlConnectionString => _builder;
        
        public ConnectionString() {
            _builder = new SqlConnectionStringBuilder();
        }

        public ConnectionString(string connectionString) {
            this.Value = connectionString;
        }

        public ConnectionString GetMasterConnection() {
            SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(Value);
            con.InitialCatalog = "master";
            return new ConnectionString(con.ConnectionString);
        }

        public ConnectionString GetConnectionWithoutCatalog() {
            SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(Value);
            con.InitialCatalog = "";
            return new ConnectionString(con.ConnectionString);
        }

        public static implicit operator ConnectionString(string v) {
            return new ConnectionString(v);
        }

        public override string ToString() {
            return Value;
        }
    }
}
