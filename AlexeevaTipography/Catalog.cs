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
    public partial class Catalog : Form
    {
        private static string connectionString = "Server=POLINA;Database=Типография;Trusted_Connection=True;";
        public Catalog()
        {
            InitializeComponent();
            InitializeComboBox();
        }

        private void InitializeComboBox()
        {
            comboBoxTables.Items.Add("Продукция");
            comboBoxTables.Items.Add("ТипПродукции");
            comboBoxTables.Items.Add("Формат");

            comboBoxTables.SelectedIndex = 0;  
            LoadTableData(comboBoxTables.SelectedItem.ToString());  
        }

        private void comboBoxTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTable = comboBoxTables.SelectedItem.ToString();
            LoadTableData(selectedTable);
        }

        private void LoadTableData(string tableName)
        {
            string query = $"SELECT * FROM {tableName}";  

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();

                try
                {
                    conn.Open();
                    dataAdapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable; 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
                }
            }
        }


private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
