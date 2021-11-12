using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace Parkering
{
    class DatabasHantering
    {
        private static string connectionString = "";
        private static string database = "Parking";
        private static string tabell = "pplats";

        public static string DB()
        {
            return String.Format("[{0}].[dbo].[{1}]",database,tabell);
        }
        public static string Connection
        {
            set { connectionString = value; }
        }
        public static string Database
        {
            set { database = value; }
        }
        public static string Tabell
        {
            set { tabell = value; }
        }
        public static string SendSqlQuery(string queryString)
        {
            int rows;
            //Här skickas en fråga rakt in i databasen.
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using SqlCommand command = new SqlCommand(queryString, connection);
                {
                    command.Connection.Open();
                    rows = command.ExecuteNonQuery();
                    command.Connection.Close();
                }
            }
            return "Number of rows affected " + rows.ToString();
        }
        public static string GetStringFromQuery(string queryString)
        {
            //Hämtar all information från tabellen och ger tillbaka en CSV liknande text med header.
            StringBuilder sb = new StringBuilder();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using SqlCommand command = new SqlCommand(queryString, connection);
                {
                    command.Connection.Open();
                    using SqlDataReader reader = command.ExecuteReader();
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if ((i + 1) == reader.FieldCount)
                                sb.Append(reader.GetName(i) + "\n");
                            else
                                sb.Append(reader.GetName(i) + ",");
                        }
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if ((i + 1) == reader.FieldCount)
                                    sb.Append(reader.GetValue(i) + "\n");
                                else
                                    sb.Append(reader.GetValue(i) + ",");
                            }
                        }
                        reader.Close();
                        command.Connection.Close();
                    }
                }
            }
            return sb.ToString();
        }
        public static string[] GetCSVFromQuery(string queryString)
        {
            //Hämtar all information från tabellen och ger tillbaka en CSV liknande text med header.
            string[] resultat;
            StringBuilder sb = new StringBuilder();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using SqlCommand command = new SqlCommand(queryString, connection);
                {
                    command.Connection.Open();
                    using SqlDataReader reader = command.ExecuteReader();
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if ((i + 1) == reader.FieldCount)
                                sb.Append(reader.GetName(i) + "\n");
                            else
                                sb.Append(reader.GetName(i) + ",");
                        }
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if ((i + 1) == reader.FieldCount)
                                    sb.Append(reader.GetValue(i) + "\n");
                                else
                                    sb.Append(reader.GetValue(i) + ",");
                            }
                        }
                        reader.Close();
                        command.Connection.Close();
                        resultat = sb.ToString().Split('\n');
                    }
                }
            }
            return resultat;
        }
    }
}
