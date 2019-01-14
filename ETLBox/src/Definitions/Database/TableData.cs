using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace ALE.ETLBox {
    public class TableData : TableData<object> {
        public TableData()  : base() { }
        public TableData(TableDefinition definition) : base() { }
        public TableData(TableDefinition definition, int estimatedBatchSize) : base() { }
    }

    public class TableData<T> : ITableData  {
        public int? EstimatedBatchSize { get; set; }
        public IColumnMappingCollection ColumnMapping {
            get {
                var mapping = new DataColumnMappingCollection();
                foreach (var col in Definition.Columns)
                    if (!col.IsIdentity)
                        mapping.Add(new DataColumnMapping(col.SourceColumn, col.DataSetColumn));
                return mapping;
            }
        }
        public bool HasIdentityColumn => IDColumnIndex != null;
        public List<T[]> Rows { get; set; }

        public T[] CurrentRow { get; set; }
        int ReadIndex { get; set; }
        TableDefinition Definition { get; set; }
        int? IDColumnIndex { get; set; }

        public TableData() {
            Rows = new List<T[]>();
        }
        public TableData(TableDefinition definition) : this() {
            Definition = definition;
        }

        public TableData(TableDefinition definition, int estimatedBatchSize) {
            Definition = definition;
            EstimatedBatchSize = estimatedBatchSize;
            Rows = new List<T[]>(estimatedBatchSize);
        }


        public string[] NewRow() => new string[Definition.Columns.Count];
        public object this[string name] => Rows[GetOrdinal(name)];
        public object this[int i] => Rows[i];
        public int Depth => 0;
        public int FieldCount => Rows.Count;
        public bool IsClosed => Rows.Count == 0;
        public int RecordsAffected => Rows.Count;
        public bool GetBoolean(int i) => Convert.ToBoolean(CurrentRow[ShiftIndexAroundIDColumn(i)]);
        public byte GetByte(int i) => Convert.ToByte(CurrentRow[ShiftIndexAroundIDColumn(i)]);
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) => 0;
        public char GetChar(int i) => Convert.ToChar(CurrentRow[ShiftIndexAroundIDColumn(i)]);
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) {
            string value = Convert.ToString(CurrentRow[ShiftIndexAroundIDColumn(i)]);
            buffer = value.Substring(bufferoffset, length).ToCharArray();
            return buffer.Length;

        }
        public DateTime GetDateTime(int i) => Convert.ToDateTime(CurrentRow[ShiftIndexAroundIDColumn(i)]);
        public IDataReader GetData(int i) => null;
        public string GetDataTypeName(int i) => Definition.Columns[ShiftIndexAroundIDColumn(i)].NETDataType.Name;
        public decimal GetDecimal(int i) => Convert.ToDecimal(CurrentRow[ShiftIndexAroundIDColumn(i)]);
        public double GetDouble(int i) => Convert.ToDouble(CurrentRow[ShiftIndexAroundIDColumn(i)]);
        public Type GetFieldType(int i) => Definition.Columns[ShiftIndexAroundIDColumn(i)].NETDataType;
        public float GetFloat(int i) => float.Parse(Convert.ToString(CurrentRow[ShiftIndexAroundIDColumn(i)]));
        public Guid GetGuid(int i) => Guid.Parse(Convert.ToString(CurrentRow[ShiftIndexAroundIDColumn(i)]));
        public short GetInt16(int i) => Convert.ToInt16(CurrentRow[ShiftIndexAroundIDColumn(i)]);
        public int GetInt32(int i) => Convert.ToInt32(CurrentRow[ShiftIndexAroundIDColumn(i)]);
        public long GetInt64(int i) => Convert.ToInt64(CurrentRow[ShiftIndexAroundIDColumn(i)]);
        public string GetName(int i) => Definition.Columns[ShiftIndexAroundIDColumn(i)].Name;
        public int GetOrdinal(string name) => Definition.Columns.FindIndex(col => col.Name == name);
        public DataTable GetSchemaTable() {
            throw new NotImplementedException();
        }
        public string GetString(int i) => Convert.ToString(CurrentRow[ShiftIndexAroundIDColumn(i)]);
        public object GetValue(int i) => CurrentRow[ShiftIndexAroundIDColumn(i)];

        int ShiftIndexAroundIDColumn(int i) {            
            if (IDColumnIndex != null) {                
                if (i > IDColumnIndex) return i - 1;
                else if (i <= IDColumnIndex) return i;
            }
            return i;

        }
        public int GetValues(object[] values) {
            values = CurrentRow as object[];
            return values.Length;
        }

        public bool IsDBNull(int i) {
            if (Definition.Columns[ShiftIndexAroundIDColumn(i)].AllowNulls)
                return CurrentRow[ShiftIndexAroundIDColumn(i)] == null;
            else
                return false;
        }

        public bool NextResult() {
            return (ReadIndex + 1) <= Rows?.Count;
        }

        public bool Read() {
            IDColumnIndex = Definition.IDColumnIndex;
            if (Rows?.Count > ReadIndex) {
                CurrentRow = Rows[ReadIndex];
                ReadIndex++;
                return true;
            } else
                return false;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    Rows.Clear();
                    Rows = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose() {
            Dispose(true);
        }

        public void Close() {
            Dispose();
        }
        #endregion
    }
}
