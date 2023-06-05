using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Ultils.Extensions
{
    public static class DataTableExtensions
    {
        public static T AsObject<T>(this DataRow row)
        {
            try
            {
                Type type = typeof(T);
                T obj = Activator.CreateInstance<T>();

                foreach (DataColumn column in row.Table.Columns)
                {
                    foreach(PropertyInfo pro in type.GetProperties())
                    {
                        if(pro.Name == column.ColumnName && row[column.ColumnName] != DBNull.Value)
                        {
                            try
                            {
                                pro.SetValue(obj, Convert.ChangeType(row[column.ColumnName], pro.PropertyType));
                            }
                            catch (Exception)
                            {
                                pro.SetValue(obj, null);
                            }
                        }
                    }
                }

                return obj;
            }
            catch (Exception)
            {
                return default(T)!;
            }
        }

        public static List<T>? AsListObject<T>(this DataTable table)
        {
            if (table == null)
                return null;

            var results = new List<T>();

            foreach (DataRow row in table.Rows)
            {
                var obj = row.AsObject<T>();
                results.Add(obj);
            }

            return results;
        }
    }
}
