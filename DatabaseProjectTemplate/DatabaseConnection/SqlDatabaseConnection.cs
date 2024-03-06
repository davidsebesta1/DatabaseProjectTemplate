using DatabaseProjectTemplate.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DatabaseProjectTemplate.DatabaseConnection
{
    public class SqlDatabaseConnection
    {
        private static SqlDatabaseConnection _instance;
        public static SqlDatabaseConnection Instance
        {
            get
            {
                Config config = ConfigHandler.Config;
                _instance ??= new SqlDatabaseConnection(config.IP, config.InitialCatalog, config.UID, config.Password);
                return _instance;
            }
        }

        private readonly string _connectionString;

        private SqlDatabaseConnection(string ip, string databaseName, string UID, string pw)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = ip;
            builder.InitialCatalog = databaseName;
            builder.UserID = UID;
            builder.Password = pw;
            builder.Pooling = true;
            _connectionString = builder.ToString();
        }

        #region Queries Methods
        public int ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        return command.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return 0;
        }

        public DataTable? ExecuteQuery(string query, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return null;
        }

        public int ExecuteScalarInt(string query, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int intValue))
                        {
                            return intValue;
                        }

                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return 0;
        }

        #endregion
    }
}
