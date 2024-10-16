using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ContactManagerT
{
    public partial class Form1 : Form
    {
        private List<Contact> contacts = new List<Contact>();
        private string connectionString = "Server=localhost;Database=personal_contacts;Uid=root;Pwd=123456;";

        public Form1()
        {
            InitializeComponent();
            LoadContacts();
        }

        private void LoadContacts()
        {
            contactListBox.Items.Clear(); // 清空当前列表
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = new MySqlCommand("SELECT name, address, phone FROM contacts", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // 创建联系人对象并添加到列表
                        var contact = new Contact(reader["name"].ToString(), reader["address"].ToString(), reader["phone"].ToString());
                        contacts.Add(contact);
                        contactListBox.Items.Add(contact); // 添加联系人对象到列表
                    }
                }
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            var name = nameTextBox.Text;
            var address = addressTextBox.Text;
            var phone = phoneTextBox.Text;

            if (!string.IsNullOrWhiteSpace(name))
            {
                var contact = new Contact(name, address, phone);
                contacts.Add(contact);
                contactListBox.Items.Add(contact); // 直接添加 Contact 对象
                SaveContact(contact);
                ClearInputFields(); // 清空输入框
            }
        }

        private void SaveContact(Contact contact)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = new MySqlCommand("INSERT INTO contacts (name, address, phone) VALUES (@name, @address, @phone)", connection);
                command.Parameters.AddWithValue("@name", contact.Name);
                command.Parameters.AddWithValue("@address", contact.Address);
                command.Parameters.AddWithValue("@phone", contact.Phone);
                command.ExecuteNonQuery();
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (contactListBox.SelectedItem != null)
            {
                var selectedContact = contactListBox.SelectedItem.ToString();
                contacts.RemoveAll(c => c.Name == selectedContact);
                contactListBox.Items.Remove(selectedContact);
                DeleteContact(selectedContact);
                ClearInputFields(); // 清空输入框
                LoadContacts();
            }
        }

        private void DeleteContact(string name)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = new MySqlCommand("DELETE FROM contacts WHERE name = @name", connection);
                command.Parameters.AddWithValue("@name", name);
                command.ExecuteNonQuery();
            }
        }

        private void contactListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (contactListBox.SelectedItem != null)
            {
                var selectedContact = contactListBox.SelectedItem.ToString();
                var contact = contacts.Find(c => c.Name == selectedContact);
                if (contact != null)
                {
                    nameTextBox.Text = contact.Name;
                    addressTextBox.Text = contact.Address;
                    phoneTextBox.Text = contact.Phone;
                }
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            if (contactListBox.SelectedItem != null)
            {
                var selectedContact = contactListBox.SelectedItem.ToString();
                var contact = contacts.Find(c => c.Name == selectedContact);

                if (contact != null)
                {
                    contact.Name = nameTextBox.Text;
                    contact.Address = addressTextBox.Text;
                    contact.Phone = phoneTextBox.Text;
                    UpdateContact(contact);
                    LoadContacts(); // 重新加载联系人
                    ClearInputFields(); // 清空输入框
                }
            }
        }

        private void UpdateContact(Contact contact)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = new MySqlCommand("UPDATE contacts SET address = @address, phone = @phone WHERE name = @name", connection);
                command.Parameters.AddWithValue("@name", contact.Name);
                command.Parameters.AddWithValue("@address", contact.Address);
                command.Parameters.AddWithValue("@phone", contact.Phone);
                command.ExecuteNonQuery();
            }
        }

        private void ClearInputFields()
        {
            nameTextBox.Clear();
            addressTextBox.Clear();
            phoneTextBox.Clear();
        }

        // 查询功能
        private void searchButton_Click(object sender, EventArgs e)
        {
            var nameToSearch = nameTextBox.Text;

            if (!string.IsNullOrWhiteSpace(nameToSearch))
            {
                var contact = contacts.Find(c => c.Name.Equals(nameToSearch, StringComparison.OrdinalIgnoreCase));
                if (contact != null)
                {
                    addressTextBox.Text = contact.Address;
                    phoneTextBox.Text = contact.Phone;
                }
                else
                {
                    MessageBox.Show("联系人未找到。");
                    ClearInputFields(); // 清空输入框
                }
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            ClearInputFields(); // 清空输入框
        }
    }
}
