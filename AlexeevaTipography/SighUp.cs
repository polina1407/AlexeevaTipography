using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using static AlexeevaTipography.Info;

namespace AlexeevaTipography
{
    public partial class SighUp : Form
    {
        private static string connectionString = "Server=POLINA;Database=Типография;Trusted_Connection=True;";

        public SighUp()
        {
            InitializeComponent();
        }

        public void RegisterButton_Click(object sender, EventArgs e)
        {
            string login = textBoxLogin.Text.Trim();
            string password = textBoxPassword.Text.Trim();
            string firstName = textBoxFirstName.Text.Trim();
            string lastName = textBoxLastName.Text.Trim();
            string middleName = textBoxMiddleName.Text.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                MessageBox.Show("Все поля должны быть заполнены.");
                return;
            }

            if (IsLoginExists(login)) // Проверка на уникальность логина
            {
                MessageBox.Show("Логин уже существует.");
                return;
            }

            int roleId = 5;
            string hashedPassword = HashPassword(password);
            string roleCheckQuery = "SELECT COUNT(*) FROM Роли WHERE ID = @RoleId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand roleCheckCmd = new SqlCommand(roleCheckQuery, conn);
                roleCheckCmd.Parameters.AddWithValue("@RoleId", roleId);

                conn.Open();

                int roleExists = (int)roleCheckCmd.ExecuteScalar();
                if (roleExists == 0)
                {
                    MessageBox.Show("Роль с таким ID не существует в базе данных.");
                    return;
                }

                // Исправление: запрос теперь с OUTPUT для получения ID
                string query = "INSERT INTO Пользователи (Логин, Хэш_Пароль, Имя, Фамилия, ID_Роли, Отчество) " +
                               "OUTPUT INSERTED.ID " +
                               "VALUES (@Login, @Password, @FirstName, @LastName, @RoleId, @MiddleName)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Login", login);
                cmd.Parameters.AddWithValue("@Password", hashedPassword);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@RoleId", roleId);
                cmd.Parameters.AddWithValue("@MiddleName", middleName);
                try
                {
                    // Получаем ID только что добавленного пользователя
                    int userId = (int)cmd.ExecuteScalar();

                    MessageBox.Show("Пользователь успешно зарегистрирован.");
                    UserSession.UserRole = roleId.ToString();
                    UserSession.UserId = userId;
                    OpenForm form2 = new OpenForm();
                    form2.ShowDialog();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при регистрации: " + ex.Message);
                }
            }
        }

        public bool IsLoginExists(string login)
        {
            string checkLoginQuery = "SELECT COUNT(*) FROM Пользователи WHERE Логин = @Login";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(checkLoginQuery, conn);
                cmd.Parameters.AddWithValue("@Login", login);

                try
                {
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0; // Если количество записей больше 0, значит логин уже существует
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при проверке логина: " + ex.Message);
                    return true;
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
