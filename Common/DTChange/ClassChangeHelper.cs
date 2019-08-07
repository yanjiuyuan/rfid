using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.ClassChange
{
    public class ClassChangeHelper
    {
        public static DataTable ToDataTable<T>(List<T> items, List<string> vs = null)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            int length = props.Length;
            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                if (vs != null)
                {
                    if (!vs.Contains(prop.Name))
                    {
                        tb.Columns.Add(prop.Name, t);
                    }
                    else
                    {
                        length = length - 1;
                    }
                }
                else
                {
                    tb.Columns.Add(prop.Name, t);
                }

            }

            foreach (T item in items)
            {
                int j = 0;
                var values = new object[length];
                for (int i = 0; i < props.Length; i++)
                {
                    if (vs != null)
                    {
                        if (!vs.Contains(props[i].Name))
                        {
                            values[j] = props[i].GetValue(item, null);
                            j++;
                        }
                    }
                    else
                    {
                        values[j] = props[i].GetValue(item, null);
                        j++;
                    }
                }
                tb.Rows.Add(values);
            }

            return tb;
        }

        /// <summary>
        /// Determine of specified type is nullable
        /// </summary>
        public static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// Return underlying type if type is Nullable otherwise return the type
        /// </summary>
        public static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
        }
    }
}
