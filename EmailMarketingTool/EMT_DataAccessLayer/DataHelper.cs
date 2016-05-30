using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMT_DataAccessLayer
{
    public static class DataHelper
    {
        //private static string connectionString;//
        public static string connectionString { get; set; }

        public static void setConnectionString()
        {
            AppSettingsReader MyReader = new AppSettingsReader();
            connectionString = MyReader.GetValue("Connection", typeof(string)).ToString();
        }
        public static SqlConnection CreateOpenConnection()
        {
            setConnectionString();
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }

        public static SqlTransaction CreateTransactionObject(SqlConnection connection)
        {
            SqlTransaction transaction;
            transaction = connection.BeginTransaction();
            return transaction;
        }


        public static int ExecuteNonQuery(SqlConnection connection, SqlTransaction transaction, CommandType cmdType, string cmdText,
           params SqlParameter[] commandParameters)
        {
            connection = CreateOpenConnection();
            try
            {

                var command = new SqlCommand(cmdText, connection);
                command.CommandType = cmdType;

                command.Parameters.AddRange(commandParameters);
                //connection.Open();

                command.Transaction = transaction;
                command.ExecuteNonQuery();

                return (int)command.Parameters["@ID"].Value;
            }
            finally
            {
                //connection.Close();
            }
            // }

        }




        public static void ExecuteNonQuery(CommandType cmdType, string cmdText,
            params SqlParameter[] commandParameters)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(cmdText, connection))
                {
                    try
                    {
                        command.CommandType = cmdType;

                        command.Parameters.AddRange(commandParameters);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public static SqlDataReader ExecuteReader(CommandType cmdType,
            string cmdText, params SqlParameter[] commandParameters)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(cmdText, connection))
                {
                    try
                    {
                        command.CommandType = cmdType;
                        command.Parameters.AddRange(commandParameters);
                        connection.Open();
                        return command.ExecuteReader();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public static object ExecuteScalar(CommandType cmdType, string cmdText,
            params SqlParameter[] commandParameters)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(cmdText, connection))
                {
                    try
                    {
                        command.CommandType = cmdType;
                        command.Parameters.AddRange(commandParameters);
                        connection.Open();
                        return command.ExecuteScalar();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        //Anupama code
        public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType cmdType, string cmdText)
        {

            using (var command = new SqlCommand(cmdText, connection))
            {
                try
                {
                    command.CommandType = cmdType;
                    return command.ExecuteReader();
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
        }

        public static DataSet ExecuteDataSet(SqlConnection connection, CommandType cmdType, string cmdText)
        {
            using (var command = new SqlCommand(cmdText, connection))
            {
                try
                {
                    command.CommandType = cmdType;
                    SqlDataAdapter adapter = new SqlDataAdapter(cmdText, connection);
                    DataSet customers = new DataSet();
                    adapter.Fill(customers);
                    return customers;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
        }


        public static DataSet ExecuteDataSet(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            using (var command = new SqlCommand(cmdText, connection))
            {
                try
                {
                    command.CommandType = cmdType;
                    command.Parameters.AddRange(commandParameters);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataSet customers = new DataSet();
                    adapter.Fill(customers);
                    return customers;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
        }



        public static DataSet ExecuteDataSet(SqlConnection connection, SqlTransaction transaction, CommandType cmdType, string cmdText)
        {
            using (var command = new SqlCommand(cmdText, connection))
            {
                try
                {
                    command.CommandType = cmdType;
                    command.Transaction = transaction;
                    SqlDataAdapter adapter = new SqlDataAdapter(cmdText, connection);
                    DataSet customers = new DataSet();
                    adapter.Fill(customers);
                    return customers;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
        }

        public static int ExecuteNonQuery(SqlConnection connection, CommandType cmdType, string cmdText,
          params SqlParameter[] commandParameters)
        {

            //using (var command = new SqlCommand(cmdText, connection))
            //{
            try
            {

                var command = new SqlCommand(cmdText, connection);
                command.CommandType = cmdType;

                command.Parameters.AddRange(commandParameters);
                //connection.Open();

                //command.Transaction = transaction;
                return command.ExecuteNonQuery();

                // return (int)command.Parameters["@ID"].Value;
            }
            finally
            {
                //connection.Close();
            }
            // }

        }

        public static object ExecuteScalar(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {

            using (var command = new SqlCommand(cmdText, connection))
            {
                try
                {
                    command.CommandType = cmdType;
                    command.Parameters.AddRange(commandParameters);
                    return command.ExecuteScalar();
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
        }

        public static object ExecuteScalar(SqlConnection connection, CommandType cmdType, string cmdText)
        {

            using (var command = new SqlCommand(cmdText, connection))
            {
                try
                {
                    command.CommandType = cmdType;
                    //command.Parameters.AddRange(commandParameters);
                    return command.ExecuteScalar();
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
        }
    }
}
