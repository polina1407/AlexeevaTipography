using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AlexeevaTipography
{
    public partial class AddOrder1 : Form
    {
        private static string connectionString = "Server=POLINA;Database=Типография;Trusted_Connection=True;";
        private int currentUserId;

        public AddOrder1(int userId)
        {
            InitializeComponent();
            currentUserId = userId;

            // Загружаем типы продукции и форматы
            LoadProductTypes();
            LoadFormats();
        }

        public void LoadProductTypes()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT ID, Название FROM ТипПродукции";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        comboBoxProductType.Items.Add(new
                        {
                            ID = reader["ID"],
                            Название = reader["Название"].ToString()
                        });
                    }

                    comboBoxProductType.DisplayMember = "Название";
                    comboBoxProductType.ValueMember = "ID";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке типов продукции: " + ex.Message);
                }
            }
        }

        public void LoadFormats()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT ID, Название FROM Формат";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        comboBoxFormat.Items.Add(new
                        {
                            ID = reader["ID"],
                            Название = reader["Название"].ToString()
                        });
                    }

                    comboBoxFormat.DisplayMember = "Название";
                    comboBoxFormat.ValueMember = "ID";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке форматов: " + ex.Message);
                }
            }
        }

        public void LoadProductData(int productTypeId, int? formatId = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT TOP 1 ID, Цена FROM Продукция WHERE ID = @ProductTypeId";
                if (formatId != null)
                {
                    query += " AND ID_Формата = @FormatId";
                }

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductTypeId", productTypeId);
                if (formatId != null)
                {
                    cmd.Parameters.AddWithValue("@FormatId", formatId);
                }

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Заполняем текстовые поля ID продукции и цены
                        textBoxProduct.Text = reader["ID"].ToString();
                        textBoxPricePerUnit.Text = Convert.ToDecimal(reader["Цена"]).ToString("F2");
                    }
                    else
                    {
                        textBoxProduct.Clear();
                        textBoxPricePerUnit.Clear();
                    }

                    CalculateTotalCost();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке данных продукции: " + ex.Message);
                }
            }
        }

        public void comboBoxProductType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxProductType.SelectedItem != null)
            {
                dynamic selectedType = comboBoxProductType.SelectedItem;
                int productTypeId = selectedType.ID;

                // Если формат выбран, передаем его в метод
                if (comboBoxFormat.SelectedItem != null)
                {
                    dynamic selectedFormat = comboBoxFormat.SelectedItem;
                    int formatId = selectedFormat.ID;
                    LoadProductData(productTypeId, formatId);
                }
                else
                {
                    // Если формат не выбран, загружаем только по типу
                    LoadProductData(productTypeId);
                }
            }
        }

        public void comboBoxFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxProductType.SelectedItem != null)
            {
                dynamic selectedType = comboBoxProductType.SelectedItem;
                int productTypeId = selectedType.ID;

                dynamic selectedFormat = comboBoxFormat.SelectedItem;
                int formatId = selectedFormat.ID;

                // Загружаем продукцию по типу и формату
                LoadProductData(productTypeId, formatId);
            }
        }

        public void textBoxQuantity_TextChanged(object sender, EventArgs e)
        {
            CalculateTotalCost();
        }

        public void CalculateTotalCost()
        {
            if (decimal.TryParse(textBoxPricePerUnit.Text, out decimal pricePerUnit) &&
                int.TryParse(textBoxQuantity.Text, out int quantity))
            {
                textBoxTotalCost.Text = (pricePerUnit * quantity).ToString("F2");
            }
            else
            {
                textBoxTotalCost.Text = string.Empty;
            }
        }

        public void buttonAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxProduct.Text) || string.IsNullOrWhiteSpace(textBoxQuantity.Text))
            {
                MessageBox.Show("Заполните все поля.");
                return;
            }

            int productId = int.Parse(textBoxProduct.Text);
            int quantity = int.Parse(textBoxQuantity.Text);
            decimal pricePerUnit = decimal.Parse(textBoxPricePerUnit.Text);
            decimal totalCost = decimal.Parse(textBoxTotalCost.Text);
            int statusId = 1; // "Новый" статус

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Заказы (ID_Пользователя, ID_Продукции, Тираж, ЦенаЗаЕденицу, ОбщаяСтоимость, ID_Статуса) " +
                               "VALUES (@UserId, @ProductId, @Quantity, @PricePerUnit, @TotalCost, @StatusId)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", currentUserId);
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@PricePerUnit", pricePerUnit);
                cmd.Parameters.AddWithValue("@TotalCost", totalCost);
                cmd.Parameters.AddWithValue("@StatusId", statusId);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Заказ успешно добавлен.");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении заказа: " + ex.Message);
                }
            }
        }
        public bool IsOrderInDatabase(int userId, int productId, int quantity, decimal pricePerUnit, decimal totalCost)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Заказы WHERE ID_Пользователя = @UserId AND ID_Продукции = @ProductId AND " +
                               "Тираж = @Quantity AND ЦенаЗаЕденицу = @PricePerUnit AND ОбщаяСтоимость = @TotalCost";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@PricePerUnit", pricePerUnit);
                cmd.Parameters.AddWithValue("@TotalCost", totalCost);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public void CleanTestOrders()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Заказы WHERE ID_Пользователя = 1 AND ID_Продукции = 1 AND Тираж = 5 " +
                               "AND ЦенаЗаЕденицу = 10000 AND ОбщаяСтоимость = 500.00";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
