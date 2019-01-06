using System.Data.SqlClient;

namespace ALE.ETLBox {
    public class ConnectionString {
        //static string PATTERNBEGIN = $@"(.*)(";
        //static string PATTERNEND = $@"\s*=\s*)(.*?)(;|$)(.*)";
        //static string DATASOURCE = $@"{PATTERNBEGIN}Data Source{PATTERNEND}";
        //static string INITIALCATALOG = $@"{PATTERNBEGIN}Initial Catalog{PATTERNEND}";
        //static string PROVIDER = $@"{PATTERNBEGIN}Provider{PATTERNEND}";
        //static string CURRENTLANGUAGE = $@"{PATTERNBEGIN}Current Language{PATTERNEND}";
        //static string AUTOTRANSLATE = $@"{PATTERNBEGIN}Auto Translate{PATTERNEND}";

        //static string VALIDCONNECTIONSTRING = @"[\w\s]+=([\w\s-_.+*&%$#&!§]+|"".*? "")(;|$)"; //Attention: double quotes in Regex are quoted with double quotes

        SqlConnectionStringBuilder _builder; 

        //string _ConnectionString;
        public string Value {
            get {
                return _builder?.ConnectionString;
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
