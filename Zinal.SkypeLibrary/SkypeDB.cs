using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace Zinal.SkypeLibrary
{
    internal class SkypeDB
    {
        SQLiteConnection _conn;

        internal static SkypeDB Instance { get; private set; }

        public SkypeDB(String DBFile)
        {
            try
            {
                _conn = new SQLiteConnection("Data Source=" + DBFile + ";Version=3;Read Only=True;");

                if(Instance == null)
                    Instance = this;
            }
            catch (SQLiteException sex)
            {
                Console.WriteLine(sex.Message);
            }
        }

        public void Open()
        {
            _conn.Open();
        }

        public void Close()
        {
            _conn.Close();
        }

        public bool isOpen()
        {
            return _conn.State == System.Data.ConnectionState.Open || _conn.State == System.Data.ConnectionState.Fetching || _conn.State == System.Data.ConnectionState.Executing;
        }

        public SkypeDBRow[] GetCustom(String tableName, String[] Fields, String whereClause = null, String orderByClause = null)
        {
            List<SkypeDBRow> data = new List<SkypeDBRow>();

            try
            {
                //PRAGMA table_info('messages')

                String FieldsStr = String.Join(",", Fields);

                if (!String.IsNullOrEmpty(whereClause))
                    whereClause = "WHERE " + whereClause;
                else
                    whereClause = "";

                if (!String.IsNullOrEmpty(orderByClause))
                    orderByClause = "ORDER BY " + orderByClause;
                else
                    orderByClause = "";

                SQLiteCommand cmd = new SQLiteCommand("SELECT " + FieldsStr + " FROM " + tableName + " " + whereClause + " " + orderByClause, _conn);
                SQLiteDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        SkypeDBRow xData = new SkypeDBRow();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            String fieldName = reader.GetName(i);
                            Object value = reader[i];
                            xData.Data.Add(fieldName, value);
                        }

                        data.Add(xData);
                    }
                }

                reader.Dispose();
                cmd.Dispose();
            }
            catch (SQLiteException sex)
            {
                Console.WriteLine(sex.Message);
            }


            return data.ToArray();
        }

        public Dictionary<String, String> GetTableFields(String tableName)
        {
            Dictionary<String, String> fields = new Dictionary<String, String>();

            try
            {
                SQLiteCommand cmd = new SQLiteCommand("PRAGMA table_info('" + tableName + "')", _conn);
                SQLiteDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        fields.Add((String)reader[1], (String)reader[2]);
                    }
                }

                reader.Dispose();
                cmd.Dispose();
            }
            catch (SQLiteException sex)
            {
                Console.WriteLine(sex.Message);
            }

            return fields;
        }

        public String[] ListTables()
        {
            List<String> tables = new List<String>();
            try
            {
                //PRAGMA table_info('messages')
                SQLiteCommand cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table'", _conn);
                SQLiteDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tables.Add((String)reader["name"]);
                    }
                }

                reader.Dispose();
                cmd.Dispose();
            }
            catch (SQLiteException sex)
            {
                Console.WriteLine(sex.Message);
            }


            return tables.ToArray();
        }

        public String[] GetChatNames()
        {
            List<String> ChatNames = new List<String>();

            try
            {
                SQLiteCommand cmd = new SQLiteCommand("SELECT name, friendlyname, activity_timestamp FROM Chats ORDER BY activity_timestamp DESC", _conn);

                SQLiteDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Object status = reader["activity_timestamp"];
                        String ChatName = (String)reader["friendlyname"];
                        String Name = (String)reader["name"];

                        if (!ChatNames.Contains(Name))
                            ChatNames.Add(ChatName);
                    }
                }

                reader.Dispose();
                cmd.Dispose();

            }
            catch (SQLiteException sex)
            {

            }

            return ChatNames.ToArray();
        }

        public void GetMessages(String chatName)
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM messages WHERE chatname = '" + chatName + "' ORDER BY id DESC", _conn);

                SQLiteDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine("Hmm");
                    }
                }

                reader.Dispose();
                cmd.Dispose();

            }
            catch (SQLiteException sex)
            {

            }
        }

    }
}
