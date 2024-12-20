using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using static AlexeevaTipography.Info;

namespace AlexeevaTipography
{
    public partial class LogIn : Form
    {
        private static string connectionString = "Server=POLINA;Database=Типография;Trusted_Connection=True;";

        public LogIn()
        {
            InitializeComponent();
        }

        public static string UserRole { get; set; }
        private void LoginButton_Click(object sender, EventArgs e)
        {
            string login = textBoxLogin.Text.Trim();
            string password = textBoxPassword.Text.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль.");
                return;
            }

            string hashedPassword = HashPassword(password);

            string query = "SELECT ID_Роли, ID FROM Пользователи WHERE Логин = @Login AND Хэш_Пароль = @Password";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Login", login);
                cmd.Parameters.AddWithValue("@Password", hashedPassword);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Получаем роль и userId
                        var roleId = reader["ID_Роли"];
                        var userId = reader["ID"];

                        if (roleId != DBNull.Value && userId != DBNull.Value)
                        {
                            MessageBox.Show("Вы успешно авторизовались!");
                            UserRole = roleId.ToString();
                            UserSession.UserId = Convert.ToInt32(userId);

                            // Открываем форму List, передавая в нее роль и userId
                            List listForm = new List( UserSession.UserId,UserRole);
                            listForm.ShowDialog();  // Ожидаем закрытия формы List
                            this.Close();  // Закрываем форму логина после открытия List
                        }
                        else
                        {
                            MessageBox.Show("Ошибка авторизации.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при авторизации: " + ex.Message);
                }
            }
        }

        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);

                StringBuilder result = new StringBuilder();
                foreach (byte b in hash)
                {
                    result.Append(b.ToString("x2"));
                }
                return result.ToString();
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            OpenForm form2 = new OpenForm();
            form2.ShowDialog();
            this.Close();
        }
    }
}
