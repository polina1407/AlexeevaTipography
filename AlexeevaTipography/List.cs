using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using static AlexeevaTipography.Info;

namespace AlexeevaTipography
{
    public partial class List : Form
    {
        private int currentUserId;
        private string userRole;
        private string connectionString = "Server=POLINA;Database=Типография;Trusted_Connection=True;";

        public List(int userId, string role)
        {
            InitializeComponent();
            currentUserId = userId;
            userRole = role;
        }
        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int recordId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);
                string tableName = comboBoxTables.SelectedValue.ToString();
                Edit editForm = new Edit(tableName, recordId);
                editForm.Show();
            }
            else
            {
                MessageBox.Show("Выберите строку для редактирования.", "Ай-ай-ай", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public List(string role)
        {
            InitializeComponent();
            userRole = role;
        }
        public static int CurrentUserId { get; set; }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            string tableName = comboBoxTables.SelectedValue.ToString();
            Add form2 = new Add(tableName);
            form2.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int userId = UserSession.UserId;
            Info profileForm = new Info(userId);
            profileForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Catalog form2 = new Catalog();
            form2.Show();
        }
        private void List_Load(object sender, EventArgs e)
        {
            LoadTables();
            if (userRole == "5") 
            {
                buttonEdit.Visible = false;
            }
        }
        public void LoadTables()
        {
            if (string.IsNullOrEmpty(userRole))
            {
                MessageBox.Show("Роль пользователя не задана. Проверьте авторизацию.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(userRole, out int roleID))
            {
                MessageBox.Show("Некорректный идентификатор роли. Роль должна быть числом.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = @"
                SELECT TABLE_NAME 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_TYPE = 'BASE TABLE'
                AND TABLE_NAME IN (
                    SELECT TablesName 
                    FROM Разрешение 
                    WHERE ID = @ID
                )";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", roleID);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                try
                {
                    conn.Open();
                    da.Fill(dt);

                    comboBoxTables.DisplayMember = "TABLE_NAME";
                    comboBoxTables.ValueMember = "TABLE_NAME";
                    comboBoxTables.DataSource = dt;

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("У вас нет доступа ни к одной таблице.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке таблиц: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void comboBoxTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxTables.SelectedItem != null)
            {
                string tableName = comboBoxTables.SelectedValue.ToString();
                LoadTableData(tableName);
            }
        }

        public void LoadTableData(string tableName)
        {
            if (tableName == "Заказы" && userRole == "5")
            {
                string query = @"
            SELECT * 
            FROM Заказы 
            WHERE ID_Пользователя = @UserId";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    da.SelectCommand.Parameters.AddWithValue("@UserId", currentUserId); // Используем ID текущего пользователя
                    DataTable dt = new DataTable();

                    try
                    {
                        conn.Open();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при загрузке данных из таблицы '{tableName}': {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                string query = $"SELECT * FROM {tableName}";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();

                    try
                    {
                        conn.Open();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при загрузке данных из таблицы '{tableName}': {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        public void buttonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string tableName = comboBoxTables.SelectedValue.ToString();
                var primaryKeyValue = dataGridView1.SelectedRows[0].Cells["ID"].Value;

                if (primaryKeyValue != null)
                {
                    string deleteQuery = $"DELETE FROM {tableName} WHERE ID = @PrimaryKey";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        SqlCommand cmd = new SqlCommand(deleteQuery, conn);
                        cmd.Parameters.AddWithValue("@PrimaryKey", primaryKeyValue);

                        try
                        {
                            conn.Open();
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Строка успешно удалена!");
                            LoadTableData(tableName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка при удалении строки: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Не удалось получить значение первичного ключа для удаления.");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите строку для удаления.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int userId = UserSession.UserId; 
            AddOrder1 form2 = new AddOrder1(userId);
            form2.Show();
        }
    }
}