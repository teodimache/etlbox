namespace ALE.ETLBox {
    public static class StringExtension {
        public static string NullOrSqlString(this string s) => s == null ? "null" : $"'{s.Replace("'","''")}'";
    }
}
