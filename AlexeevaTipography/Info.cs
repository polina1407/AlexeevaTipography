using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AlexeevaTipography
{
    public partial class Info : Form
    {
        private static string connectionString = "Server=POLINA;Database=Типография;Trusted_Connection=True;";
        private int userId;  

        public Info(int userId)
        {
            InitializeComponent();
            this.userId = userId;  
        }
        public static class UserSession
        {
            public static int UserId { get; set; }
            public static string UserRole { get; set; }
        }
        private void Info_Load(object sender, EventArgs e)
        {
            LoadUserProfile(UserSession.UserId);  
        }


        public void LoadUserProfile(int userId)
        {
            string query = "SELECT Логин, Фамилия, Имя, Отчество FROM Пользователи WHERE ID = @UserId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Заполняем поля формы данными из базы
                        textBoxLogin.Text = reader["Логин"].ToString();
                        textBoxFirstName.Text = reader["Имя"].ToString();
                        textBoxLastName.Text = reader["Фамилия"].ToString();
                        textBoxMiddleName.Text = reader["Отчество"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("Пользователь не найден.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке профиля: " + ex.Message);
                }
            }
        }



        private void SaveChangesButton_Click(object sender, EventArgs e)
        {
            string login = textBoxLogin.Text.Trim();
            string firstName = textBoxFirstName.Text.Trim();
            string lastName = textBoxLastName.Text.Trim();
            string middleName = textBoxMiddleName.Text.Trim();

            if (string.IsNullOrEmpty(login))
            {
                MessageBox.Show("Логин не может быть пустым.");
                return;
            }

            string query = "UPDATE Пользователи SET Логин = @Login, Имя = @FirstName, Фамилия = @LastName, Отчество = @MiddleName WHERE ID = @UserId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Login", login);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@MiddleName", middleName);
                cmd.Parameters.AddWithValue("@UserId", userId);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Данные профиля обновлены.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении изменений: " + ex.Message);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
