using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Zinal.SkypeLibrary
{
    internal static class SkypeClassConverter
    {
        public static T ConvertTo<T>(SkypeDBRow data)
        {
            try
            {
                Type chatType = typeof(T);

                var Obj = typeof(T).InvokeMember("", BindingFlags.CreateInstance, null, null, new Object[0]);

                foreach (String key in data.Data.Keys)
                {
                    Object X = Obj.GetType().InvokeMember(key, BindingFlags.GetField, null, Obj, new Object[0]);
                    FieldInfo fi = Obj.GetType().GetField(key, BindingFlags.GetField | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
                    if (fi != null)
                    {
                        if (fi.FieldType == typeof(int))
                        {
                            int val;
                            if (int.TryParse(data[key].ToString(), out val))
                                fi.SetValue(Obj, val);
                        }
                        else if (fi.FieldType == typeof(String))
                            fi.SetValue(Obj, data[key].ToString());
                        else if (fi.FieldType == typeof(DateTime))
                        {
                            long timestamp;
                            if (long.TryParse(data[key].ToString(), out timestamp))
                            {
                                DateTime dateTime = UnixTimeStampToDateTime(timestamp);
                                fi.SetValue(Obj, dateTime);

                                FieldInfo longFI = Obj.GetType().GetField(key + "Long", BindingFlags.GetField | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
                                if (longFI != null)
                                    longFI.SetValue(Obj, timestamp);
                            }
                        }
                        else if (fi.FieldType == typeof(Object))
                            fi.SetValue(Obj, data[key]);
                    }
                    else
                        Console.WriteLine("Missing Field " + key + " in " + chatType.Name);
                }

                return (T)Obj;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return default(T);
        }


        private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            DateTime utcNow = DateTime.Now;
            utcNow.AddSeconds(unixTimeStamp);

            return utcNow;
        }

    }
}
