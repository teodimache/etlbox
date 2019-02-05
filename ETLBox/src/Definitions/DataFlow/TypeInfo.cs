using System;
using System.Reflection;

namespace ALE.ETLBox.DataFlow {
    internal class TypeInfo {
        internal PropertyInfo[] PropertyInfos { get; set; }
        internal int PropertyLength { get; set; }
        internal bool IsArray { get; set; } = true;

        internal TypeInfo() {

        }

        internal TypeInfo(Type typ) {
            GatherTypeInfos(typ);
        }
        private void GatherTypeInfos(Type typ) {            
            IsArray = typ.IsArray;
            if (!typ.IsArray) {
                PropertyInfos = typ.GetProperties();
                PropertyLength = PropertyInfos.Length;
            }
        }

        public static object CastPropertyValue(PropertyInfo property, string value) {
            if (property == null || String.IsNullOrEmpty(value))
                return null;
            if (property.PropertyType == typeof(bool))
                return value == "1" || value == "true" || value == "on" || value == "checked";
            else
                return Convert.ChangeType(value, property.PropertyType);
        }
    }
}
