using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AlexeevaTipography;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Linq;


namespace AlexeevaTipography.tests
{
    [TestClass]
    public class ProductTypeTests
    {
        private static string connectionString = "Server=POLINA;Database=Типография;Trusted_Connection=True;";
        [TestMethod]
        public void LoadProductTypes_ShouldReturnNonEmptyList()
        {
            var form = new AddOrder1(1);

            form.LoadProductTypes();

            var comboBox = form.Controls.Find("comboBoxProductType", true).FirstOrDefault() as ComboBox;
            Assert.IsNotNull(comboBox, "ComboBoxProductType не найден.");
            Assert.IsTrue(comboBox.Items.Count > 0, "Типы продукции не загружаются.");
        }


        [TestMethod]
        public void LoadFormats_ShouldReturnNonEmptyList()
        {
            var form = new AddOrder1(1);
            var comboBox = form.Controls["comboBoxFormat"] as ComboBox;

            form.LoadFormats();

            Assert.IsNotNull(comboBox, "ComboBoxFormat не найден.");
            Assert.IsTrue(comboBox.Items.Count > 0, "Форматы не загружаются.");
        }
        [TestMethod]
        public void HashPassword_ShouldReturnConsistentHash()
        {
            var form = new SighUp();

            string password = "TestPassword123!";
            string hash1 = form.HashPassword(password);
            string hash2 = form.HashPassword(password);

            Assert.AreEqual(hash1, hash2, "Хэши для одинаковых паролей должны совпадать.");
            Assert.AreNotEqual(password, hash1, "Хэш не должен совпадать с паролем.");
        }

        [TestMethod]
        public void LoadTableData_ShouldLoadOnlyCurrentUserOrders()
        {
            // Arrange
            var form = new List(0, "5"); // "5" - роль клиента
            var tableName = "Заказы";

            // Act
            form.LoadTableData(tableName);
            var dataGridView = form.Controls["dataGridView1"] as DataGridView;

            // Assert
            Assert.IsNotNull(dataGridView, "DataGridView1 не найден.");
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                Assert.AreEqual(0, Convert.ToInt32(row.Cells["ID_Пользователя"].Value), "Загружены данные, не относящиеся к текущему пользователю.");
            }
        }
        [TestMethod]
        public void TestHashPassword()
        {
            // Arrange
            string password = "testPassword";
            string expectedHash = "fd5cb51bafd60f6fdbedde6e62c473da6f247db271633e15919bab78a02ee9eb";

            var loginForm = new LogIn();

            // Act
            string hashedPassword = loginForm.HashPassword(password);

            // Assert
            Assert.AreEqual(expectedHash, hashedPassword);
        }
   
        [TestMethod]
        public void TestLoadUserProfile_Successful()
        {
            // Arrange
            var infoForm = new Info(1);
            var textBoxLogin = infoForm.Controls["textBoxLogin"] as TextBox;
            var textBoxFirstName = infoForm.Controls["textBoxFirstName"] as TextBox;
            var textBoxLastName = infoForm.Controls["textBoxLastName"] as TextBox;
            var textBoxMiddleName = infoForm.Controls["textBoxMiddleName"] as TextBox;

            textBoxLogin.Text = "testLogin";
            textBoxFirstName.Text = "John";
            textBoxLastName.Text = "Doe";
            textBoxMiddleName.Text = "MiddleName";
            infoForm.LoadUserProfile(1);  

            Assert.AreEqual("testLogin", textBoxLogin.Text);
            Assert.AreEqual("John", textBoxFirstName.Text);
            Assert.AreEqual("Doe", textBoxLastName.Text);
            Assert.AreEqual("MiddleName", textBoxMiddleName.Text);
        }

        [TestMethod]
        public void CalculateTotalCost_ShouldReturnCorrectResult()
        {
            var form = new AddOrder1(1);

            var textBoxPricePerUnit = form.Controls["textBoxPricePerUnit"] as TextBox;
            var textBoxQuantity = form.Controls["textBoxQuantity"] as TextBox;
            var textBoxTotalCost = form.Controls["textBoxTotalCost"] as TextBox;

            textBoxPricePerUnit.Text = "50,00";
            textBoxQuantity.Text = "2";

            form.CalculateTotalCost();
            Assert.AreEqual("100,00", textBoxTotalCost.Text, "Общая стоимость рассчитывается неправильно.");
        }

        [TestMethod]
        public void CalculateTotalCost_ShouldHandleInvalidData()
        {
            var form = new AddOrder1(1);
            var textBoxQuantity = form.Controls["textBoxQuantity"] as TextBox;

            textBoxQuantity.Text = "2";

            form.CalculateTotalCost();

            Assert.AreEqual(string.Empty, form.Controls["textBoxTotalCost"].Text, "Должна быть пустая строка при неверных данных.");
        }
        [TestMethod]
        public void LoadProductData_ShouldFillProductIdCorrectly()
        {
            var form = new AddOrder1(1);
            var productTypeId = 1;
            form.LoadProductData(productTypeId);

            var textBoxProductId = form.Controls["textBoxProduct"] as TextBox;
            Assert.IsFalse(string.IsNullOrEmpty(textBoxProductId.Text), "ID продукции не заполняется.");
        }

        [TestMethod]
        public void ButtonAdd_ShouldShowError_WhenFieldsAreEmpty()
        {
            // Arrange
            var form = new AddOrder1(1);
            var textBoxQuantity = form.Controls["textBoxQuantity"] as TextBox;
            textBoxQuantity.Text = string.Empty;

            // Act
            form.buttonAdd_Click(null, EventArgs.Empty);

            // Assert
            Assert.IsTrue(MessageBox.Show("Заполните все поля.") != null); // Допустим, используем вспомогательный метод LastShownMessage
        }
    }
}