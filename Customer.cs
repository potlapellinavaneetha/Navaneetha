using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Reflection;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Real_Estate_tool
{
    public partial class Customer : Form

    {
        private string? _custId;
        private string Connstring = "Data Source=PROBYTE\\SQLEXPRESS;Initial Catalog=RealEstateTool;Integrated Security=True;Encrypt=False";

        public Customer()
        {
            InitializeComponent();
          

           // Cmbx_custname.Items.Add("Golden Customer");
            Cmbx_custname.Items.Add("New Customer");

            Btn_addcustomer.Enabled = true;
            Btn_update.Enabled = false;
            Btn_delete.Enabled = false;
            Logger.Log("Customer form loaded for new entry.");
        }


        public Customer(string custId, string name, string mobile, string email, string address, string customerType)
        {
            InitializeComponent();
           

            Cmbx_custname.Items.Add("Golden Customer");
            Cmbx_custname.Items.Add("New Customer");

            _custId = custId;
            Txt_name.Text = name;
            Txt_mobilenumber.Text = mobile;
            Txt_email.Text = email;
            Txt_address.Text = address;

            Cmbx_custname.SelectedItem = customerType;

            Btn_addcustomer.Enabled = false;
            Btn_update.Enabled = true;
            Btn_delete.Enabled = true;
            Logger.Log($"Customer form loaded for edit: CustID={custId}, Name={name}");
        }

        private void Btn_addcustomer_Click(object sender, EventArgs e)
        {
            string Name = Txt_name.Text.Trim();
            string MobileNumber = Txt_mobilenumber.Text.Trim();
            string Address = Txt_address.Text.Trim();
            string email = Txt_email.Text.Trim();
         
            
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(MobileNumber))
            {
                MessageBox.Show("Name and Mobile Number are required.");
                return;
            }
            if (Txt_mobilenumber.Text.Length != 10 || !Txt_mobilenumber.Text.All(char.IsDigit))
            {
                MessageBox.Show("Mobile number must be exactly 10 digits.");
                return;
            }
           

            try
            {
                using (SqlConnection con = new SqlConnection(Connstring))
                {
                    string query = "INSERT INTO AddCustomers (Name, MobileNumber, Email, Address,CustomerType) VALUES (@Name, @MobileNumber, @Email, @Address,@CustomerType)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {

                        cmd.Parameters.AddWithValue("@Name", Name);
                        cmd.Parameters.AddWithValue("@MobileNumber", MobileNumber);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Address", Address);
                        cmd.Parameters.AddWithValue("@CustomerType", Cmbx_custname.SelectedItem?.ToString());


                        con.Open();
                        int rows = cmd.ExecuteNonQuery();
                        con.Close();


                        if (rows > 0)
                        {
                            
                          
                            MessageBox.Show("Customer added successfully!");
                            ClearForm();
                        }
                        else
                        {
                            
                            MessageBox.Show("Failed to add customer.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error adding customer: " + ex.Message);
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void ClearForm()
        {
            Txt_name.Text = "";
            Txt_email.Text = "";
            Txt_mobilenumber.Text = "";
            Txt_address.Text = "";
        }





        private void Btn_edit_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Connstring))
                {
                    conn.Open();

                    string query = "UPDATE AddCustomers SET Name = @Name, MobileNumber = @Mobile, Email = @Email, Address = @Address,CustomerType=@CustomerType WHERE CustID = @CustID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CustID", _custId);
                        cmd.Parameters.AddWithValue("@Name", Txt_name.Text.Trim());
                        cmd.Parameters.AddWithValue("@Mobile", Txt_mobilenumber.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", Txt_email.Text.Trim());
                        cmd.Parameters.AddWithValue("@Address", Txt_address.Text.Trim());
                        cmd.Parameters.AddWithValue("@CustomerType", Cmbx_custname.SelectedItem?.ToString());


                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Logger.Log($"Customer updated: CustID={_custId}, Name={Txt_name.Text.Trim()}");
                            MessageBox.Show("Customer updated successfully.");
                        }
                           
                        else
                            MessageBox.Show("No records updated.");
                    }
                }

                this.Close(); // Optional: close the form after update
            }
            catch (Exception ex)
            {
                Logger.Log("Error updating customer: " + ex.Message);
                MessageBox.Show("Error updating customer: " + ex.Message);
            }
        }






        private void Btn_delete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this customer?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(Connstring)) // Use your existing Connstring variable
                {
                    try
                    {
                        conn.Open();
                        string query = "DELETE FROM AddCustomers WHERE CustID = @id";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", _custId);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                Logger.Log($"Customer deleted: CustID={_custId}");
                                MessageBox.Show("Customer deleted successfully!");
                                this.DialogResult = DialogResult.OK;
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("No customer found to delete.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("Error deleting customer: " + ex.Message);
                        MessageBox.Show("Error deleting customer: " + ex.Message);
                    }
                }
            }
        }



        private void Txt_name_TextChanged(object sender, EventArgs e)
        {

        }

        private void Txt_mobilenumber_TextChanged(object sender, EventArgs e)
        {

        }

        private void Txt_address_TextChanged(object sender, EventArgs e)
        {

        }

        private void Txt_email_TextChanged(object sender, EventArgs e)
        {

        }

        private void Cmbx_custname_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
