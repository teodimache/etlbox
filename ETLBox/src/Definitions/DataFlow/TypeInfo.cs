using System;
using System.Reflection;

namespace ALE.ETLBox {
    public class TypeInfo {
        public PropertyInfo[] PropertyInfos { get; set; }
        public int PropertyLength { get; set; }
        public bool IsArray { get; set; } = true;

        public TypeInfo() {

        }

        public TypeInfo(Type typ) {
            GatherTypeInfos(typ);
        }
        private void GatherTypeInfos(Type typ) {            
            IsArray = typ.IsArray;
            if (!typ.IsArray) {
                PropertyInfos = typ.GetProperties();
                PropertyLength = PropertyInfos.Length;
            }
        }
    }
}
