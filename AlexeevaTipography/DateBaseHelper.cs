using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlexeevaTipography
{
    public static class DatabaseHelper
    {
        // Строка подключения к вашей базе данных
        private static string connectionString = "Server=POLINA;Database=Типография;Trusted_Connection=True;";

        /// <summary>
        /// Выполняет SQL-запрос и возвращает результат в виде DataTable.
        /// </summary>
        /// <param name="query">SQL-запрос</param>
        /// <returns>DataTable с результатами</returns>
        public static DataTable ExecuteQuery(string query)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выполнения запроса: {ex.Message}");
            }

            return dataTable;
        }

        /// <summary>
        /// Выполняет SQL-команду без возвращаемого результата (например, INSERT, UPDATE, DELETE).
        /// </summary>
        /// <param name="query">SQL-команда</param>
        public static void ExecuteNonQuery(string query)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выполнения команды: {ex.Message}");
            }
        }
    }
}

