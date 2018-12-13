using ALE.ETLBox;
using System;
using System.IO;
using System.Linq;

namespace ALE.ETLBoxTest {
    public class BigDataHelper {
        public string FileName { get; set; }
        public TableDefinition TableDefinition { get; set; }
        public int NumberOfRows { get; set; }
        public void CreateBigDataCSV() {
            using (FileStream stream = File.Open(FileName,FileMode.Create))
            using (StreamWriter writer = new StreamWriter(stream)) {
                string header = String.Join(",", TableDefinition.Columns.Select(col => col.Name));
                writer.WriteLine(header);
                for (int i = 0; i < NumberOfRows; i++) {
                    string line = String.Join(",",TableDefinition.Columns.Select(col => {
                        int length = DataTypeConverter.GetStringLengthFromCharString(col.DataType);
                        return TestHelper.RandomString(length);
                    }));
                    writer.WriteLine(line);
                }
            }
        }
    }
}
