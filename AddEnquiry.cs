using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Real_Estate_tool
{
    public partial class AddEnquiry : Form
    {
        private string Connstring = "Data Source=PROBYTE\\SQLEXPRESS;Initial Catalog=RealEstateTool;Integrated Security=True;Encrypt=False";
        private string _enquiryid;
        public AddEnquiry()
        {
            InitializeComponent();

            LoadEnquiryPurposeItems();
            Logger.Log("AddEnquiry form loaded for new entry.");

        }
        private void LoadEnquiryPurposeItems()
        {
            Cmbx_enquirypurpose.Items.Clear();
            Cmbx_enquirypurpose.Items.Add("Investment");
            Cmbx_enquirypurpose.Items.Add("Personal Use");
            Cmbx_enquirypurpose.Items.Add("Commercial");
            Cmbx_enquirypurpose.Items.Add("Other");
        }

        public AddEnquiry(string Enquiryid,string EnquiryName,string MobileNumber,string Address,string planToPurchase,string PurposeOfEnquiry,string Status) 
        {
           InitializeComponent();
            LoadEnquiryPurposeItems();
            _enquiryid = Enquiryid;
            Txt_enqiryname.Text = EnquiryName;
            Txt_mobileno.Text = MobileNumber;
            Txt_address.Text = Address;
            Txt_plantopurchase.Text = planToPurchase;
            Txt_status.Text = Status;

            int index = Cmbx_enquirypurpose.FindStringExact(PurposeOfEnquiry);
            if (index >= 0)
                Cmbx_enquirypurpose.SelectedIndex = index;
            else
                Cmbx_enquirypurpose.SelectedIndex = -1;
            Logger.Log($"AddEnquiry form loaded for editing: EnquiryID={_enquiryid}");
        }


        private void Txt_enqiryname_TextChanged(object sender, EventArgs e)
        {

        }

        private void Txt_mobileno_TextChanged(object sender, EventArgs e)
        {

        }

        private void Cmbx_enquirypurpose_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Btn_update_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Connstring))
                {
                    conn.Open();

                    string query = "UPDATE AddEnquiry SET CustomerName = @CustomerName, MobileNumber = @MobileNumber, Status = @Status, Address = @Address,PlanToPurchase=@PlanToPurchase,PurposeOfEnquiry=@PurposeOfEnquiry WHERE EnquiryID = @EnquiryID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EnquiryID", _enquiryid);
                        cmd.Parameters.AddWithValue("@CustomerName", Txt_enqiryname.Text.Trim());
                        cmd.Parameters.AddWithValue("@MobileNumber", Txt_mobileno.Text.Trim());
                        cmd.Parameters.AddWithValue("@Status", Txt_status.Text.Trim());
                        cmd.Parameters.AddWithValue("@Address", Txt_address.Text.Trim());
                        cmd.Parameters.AddWithValue("@PlanToPurchase", Txt_plantopurchase.Text.Trim());
                        cmd.Parameters.AddWithValue("@PurposeOfEnquiry", Cmbx_enquirypurpose.SelectedItem?.ToString());


                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Logger.Log($"Enquiry updated: EnquiryID={_enquiryid}, Name={Txt_enqiryname.Text.Trim()}");
                            MessageBox.Show("Enquiry updated successfully.");

                        }
                            
                        else
                            MessageBox.Show("No records updated.");
                    }
                }

                this.Close(); // Optional: close the form after update
            }
            catch (Exception ex)
            {
                Logger.Log("Error updating enquiry: " + ex.Message);
                MessageBox.Show("Error updating enquiry: " + ex.Message);
            }
        }

        private void Btn_deleteenquiry_Click(object sender, EventArgs e)
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
                        string query = "DELETE FROM AddEnquiry WHERE EnquiryID = @id";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", _enquiryid);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                Logger.Log($"Enquiry deleted: EnquiryID={_enquiryid}");
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
                        Logger.Log("Error deleting enquiry: " + ex.Message);
                        MessageBox.Show("Error deleting customer: " + ex.Message);
                    }
                }
            }
        }


        private void Btn_addenquiry_Click(object sender, EventArgs e)
        {
            string Name = Txt_enqiryname.Text.Trim();
            string MobileNumber = Txt_mobileno.Text.Trim();
            string Address = Txt_address.Text.Trim();
            string PlanToPurchase = Txt_plantopurchase.Text.Trim();
            string Status = Txt_status.Text.Trim();
            string PurposeOfEnquiry = Cmbx_enquirypurpose.SelectedItem != null ? Cmbx_enquirypurpose.SelectedItem.ToString() : "";


            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(MobileNumber))
            {
                MessageBox.Show("Name and Mobile Number are required.");
                return;
            }
            try
            {
                using (SqlConnection con = new SqlConnection(Connstring))
                {
                    string query = "INSERT INTO AddEnquiry (CustomerName, MobileNumber, Status, Address,PlanToPurchase,PurposeOfEnquiry) VALUES (@CustomerName, @MobileNumber, @Status, @Address,@PlanToPurchase,@PurposeOfEnquiry)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {

                        cmd.Parameters.AddWithValue("@CustomerName",Name);
                        cmd.Parameters.AddWithValue("@MobileNumber", MobileNumber);
                        cmd.Parameters.AddWithValue("@Status", Status);
                        cmd.Parameters.AddWithValue("@Address", Address);
                        cmd.Parameters.AddWithValue("@PlanToPurchase", PlanToPurchase);
                        cmd.Parameters.AddWithValue("@PurposeOfEnquiry",PurposeOfEnquiry );


                        con.Open();
                        int rows = cmd.ExecuteNonQuery();
                        con.Close();


                        if (rows > 0)
                        {
                            Logger.Log($"Enquiry added: Name={Name}, Mobile={MobileNumber}, Purpose={PurposeOfEnquiry}");
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
                Logger.Log("Error adding enquiry: " + ex.Message);
                MessageBox.Show("Error: " + ex.Message);
            }
        }
             private void ClearForm()
             {   
                Txt_enqiryname.Text = "";
                Txt_mobileno.Text = "";
                Txt_status.Text = "";
                Txt_address.Text = "";
                Txt_plantopurchase.Text = "";
            Cmbx_enquirypurpose.SelectedIndex = -1;



        }


        private void Txt_address_TextChanged(object sender, EventArgs e)
        {

        }

        private void Txt_plantopurchase_TextChanged(object sender, EventArgs e)
        {

        }

        private void Txt_status_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
