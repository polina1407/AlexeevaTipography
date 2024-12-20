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
    public partial class Edit : Form
    {
        private string tableName;
        private int recordId;
        private string connectionString = "Server=POLINA;Database=Типография;Trusted_Connection=True;";
        public Edit(string tableName, int recordId)
        {
            InitializeComponent();
            this.tableName = tableName;
            this.recordId = recordId;
        }
        private void EditForm_Load(object sender, EventArgs e)
        {
            GenerateFormFields();
            LoadRecordData();
        }

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
                    int formWidth = this.ClientSize.Width; // Ширина формы

                    while (reader.Read())
                    {
                        string columnName = reader["COLUMN_NAME"].ToString();

                        // Пропускаем "ID"
                        if (columnName.Equals("ID", StringComparison.OrdinalIgnoreCase))
                            continue;

                        // Метка
                        Label label = new Label
                        {
                            Text = columnName,
                            Location = new Point(10, y), // Метки будут располагаться ближе к левому краю
                            Width = 100,
                            Font = new Font("Arial", 10), // Устанавливаем шрифт Arial размером 10
                            TextAlign = ContentAlignment.MiddleRight // Выравнивание текста справа
                        };
                        this.Controls.Add(label);

                        // Поле ввода
                        TextBox textBox = new TextBox
                        {
                            Name = "txt_" + columnName,
                            Location = new Point(120, y), 
                            Width = 200,
                            Font = new Font("Arial", 10) // Устанавливаем шрифт Arial размером 10
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



        private void LoadRecordData()
        {
            string query = $"SELECT * FROM {tableName} WHERE ID = @RecordID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RecordID", recordId);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        foreach (Control control in this.Controls)
                        {
                            if (control is TextBox textBox && textBox.Name.StartsWith("txt_"))
                            {
                                string columnName = textBox.Name.Replace("txt_", "");
                                textBox.Text = reader[columnName]?.ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке данных записи: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void SaveData()
        {
            string query = $"UPDATE {tableName} SET ";

            foreach (Control control in this.Controls)
            {
                if (control is TextBox textBox && textBox.Name.StartsWith("txt_"))
                {
                    string columnName = textBox.Name.Replace("txt_", "");
                    query += $"{columnName} = @{columnName}, ";
                }
            }

            query = query.TrimEnd(',', ' ') + " WHERE ID = @ID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);

                foreach (Control control in this.Controls)
                {
                    if (control is TextBox textBox && textBox.Name.StartsWith("txt_"))
                    {
                        string columnName = textBox.Name.Replace("txt_", "");
                        cmd.Parameters.AddWithValue($"@{columnName}", textBox.Text);
                    }
                }
                cmd.Parameters.AddWithValue("@ID", recordId);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Изменения успешно сохранены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadRecordData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении изменений: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
