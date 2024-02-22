using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace DR.Export.Extentions {

    public static class TableExtention {
        /// Phải có description không có tự động bỏ qua, không thêm description lỗi ráng chịu
        public static DataTable ToDataTable<T>(this IList<T> data) {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++) {
                PropertyDescriptor prop = props[i];
                if (!string.IsNullOrEmpty(prop.Description)) {
                    table.Columns.Add(prop.Description, prop.PropertyType);
                }
            }
            object[] values = new object[table.Columns.Count];
            foreach (T item in data) {
                for (int i = 0; i < values.Length; i++) {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
    }
}
