using System.Collections.Generic;
using System.Linq;

namespace ALE.ETLBox {
    public class TableDefinition {
        public string Name { get; set; }
        public List<TableColumn> Columns { get; set; }
        public int? IDColumnIndex {
            get {
                TableColumn idCol = Columns.FirstOrDefault(col => col.IsIdentity);
                if (idCol != null)
                    return Columns.IndexOf(idCol);
                else
                    return null;
            }
        }

        public string AllColumnsWithoutIdentity => Columns.Where(col => !col.IsIdentity).AsString();
        

        public TableDefinition() {
            Columns = new List<TableColumn>();
        }

        public TableDefinition(string name) : this() {
            Name = name;
        }

        public TableDefinition(string name, List<TableColumn> columns) : this(name) {
            Columns = columns;
        }

        public void CreateTable() {
            CreateTableTask.Create(this);
        }

      
    }
}
