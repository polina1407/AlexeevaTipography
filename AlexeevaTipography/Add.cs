using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlexeevaTipography
{
    public partial class Add : Form
    {
        private string tableName;
        private string connectionString = "Server=POLINA;Database=Типография;Trusted_Connection=True;";

        public Add(string tableName)
        {
            InitializeComponent();
            this.tableName = tableName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Add_Load(object sender, EventArgs e)
        {
            GenerateFormFields();
        }
        // Генерация динамических полей на основе структуры таблицы
        private void GenerateFormFields()
        {
            string query = $"SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TableName", tableName);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    int y = 10;
                    int formWidth = this.ClientSize.Width;

                    while (reader.Read())
                    {
                        string columnName = reader["COLUMN_NAME"].ToString();

                        // Пропускаем "ID" (предположительно автоинкремент)
                        if (columnName.Equals("ID", StringComparison.OrdinalIgnoreCase))
                            continue;

                        // Метка
                        Label label = new Label
                        {
                            Text = columnName,
                            Location = new Point(10, y),
                            Width = 100,
                            Font = new Font("Arial", 10),
                            TextAlign = ContentAlignment.MiddleRight
                        };
                        this.Controls.Add(label);

                        // Поле ввода
                        TextBox textBox = new TextBox
                        {
                            Name = "txt_" + columnName,
                            Location = new Point(120, y),
                            Width = 200,
                            Font = new Font("Arial", 10)
                        };
                        this.Controls.Add(textBox);

                        y += 30;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при создании формы: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Обработчик кнопки "Сохранить" для добавления новой записи
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (tableName != "Заказы")
            {
                AddRecord();
            }
            else
            {
                MessageBox.Show("Для добавления заказа используйте отдельную форму.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AddRecord()
        {
            string query = $"INSERT INTO {tableName} (";

            // Формируем список полей для INSERT
            foreach (Control control in this.Controls)
            {
                if (control is TextBox textBox && textBox.Name.StartsWith("txt_"))
                {
                    string columnName = textBox.Name.Replace("txt_", "");
                    query += $"{columnName}, ";
                }
            }

            // Убираем последнюю запятую и пробел
            query = query.TrimEnd(',', ' ') + ") VALUES (";

            // Формируем параметры для значений
            foreach (Control control in this.Controls)
            {
                if (control is TextBox textBox && textBox.Name.StartsWith("txt_"))
                {
                    string columnName = textBox.Name.Replace("txt_", "");
                    query += $"@{columnName}, ";
                }
            }

            // Убираем последнюю запятую и пробел
            query = query.TrimEnd(',', ' ') + ")";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);

                // Добавляем параметры для каждого текстового поля
                foreach (Control control in this.Controls)
                {
                    if (control is TextBox textBox && textBox.Name.StartsWith("txt_"))
                    {
                        string columnName = textBox.Name.Replace("txt_", "");
                        cmd.Parameters.AddWithValue($"@{columnName}", textBox.Text);
                    }
                }

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Запись успешно добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close(); // Закрываем форму после добавления записи
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении записи: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Обработчик кнопки "Отмена"
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}