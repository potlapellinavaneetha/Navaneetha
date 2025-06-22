using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;

namespace Real_Estate_tool
{
    public partial class AddPlots : Form

    {
        private string? _plotId;
        private string Connstring = "Data Source=PROBYTE\\SQLEXPRESS;Initial Catalog=RealEstateTool;Integrated Security=True;Encrypt=False";
        public AddPlots()
        {
            InitializeComponent();
            LoadVentures();
            LoadPhaseAndStatusItems();



        }
        private void LoadPhaseAndStatusItems()
        {
            Cmbx_plotphase.Items.Clear();
            Cmbx_plotphase.Items.AddRange(new string[] { "EAST", "WEST", "NORTH", "SOUTH" });

            Cmbx_plotstatus.Items.Clear();
            Cmbx_plotstatus.Items.AddRange(new string[] { "SOLD OUT", "OPEN", "RE-SALE" });
        }

        public AddPlots(string ventureid, string plotnumber, string imagepath, string plotphase, string status, string plotsize, string plotid)
        {
            InitializeComponent();
            LoadVentures();
            LoadPhaseAndStatusItems();
            //LoadPlotNumber();
            if (int.TryParse(ventureid, out int vid))
            {
                Cmbx_ventID.SelectedValue = vid;
            }

            Txt_imagepath.Text = imagepath;
            Txt_plotsize.Text = plotsize;
            Txt_plotnumber.Text = plotnumber;
            _plotId = plotid;
            if (!string.IsNullOrEmpty(imagepath) && System.IO.File.Exists(imagepath))
            {
                Picturebox.Image = Image.FromFile(imagepath);
                Picturebox.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            Cmbx_plotstatus.SelectedItem = status;
            Cmbx_plotphase.SelectedItem = plotphase;

        }


        private void LoadVentures()
        {
            string connStr = "Data Source=PROBYTE\\SQLEXPRESS;Initial Catalog=RealEstateTool;Integrated Security=True;Encrypt=False";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT VentureID, Location, CAST(VentureID AS VARCHAR) + ' - ' + Location AS DisplayText FROM AddVenture";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                Cmbx_ventID.DataSource = dt;
                Cmbx_ventID.DisplayMember = "DisplayText"; // Shows "1 - Hyderabad"
                Cmbx_ventID.ValueMember = "VentureID";     // Internally stores just 1
            }
        }
        //private void LoadPlotNumber()
        //{
        //    string connStr = "Data Source=PROBYTE\\SQLEXPRESS;Initial Catalog=RealEstateTool;Integrated Security=True;Encrypt=False";
        //    using (SqlConnection conn = new SqlConnection(connStr))
        //    {
        //        string query = "SELECT  Plotnumber FROM AddPlots";
        //        SqlDataAdapter da = new SqlDataAdapter(query, conn);
        //        DataTable dt = new DataTable();
        //        da.Fill(dt);

        //        Cmbx_plotID.DataSource = dt;
        //        Cmbx_plotID.DisplayMember = "DisplayText"; // Shows "1 - Hyderabad"
        //        Cmbx_plotID.ValueMember = "Plotnumber";     // Internally stores just 1
        //    }
        //}

        private bool IsPlotNumberExists(string plotNumber, int ventureId)
        {
            string connString = "Data Source=PROBYTE\\SQLEXPRESS;Initial Catalog=RealEstateTool;Integrated Security=True;Encrypt=False";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT COUNT(*) FROM AddPlots WHERE PlotNumber = @PlotNumber AND VentureID = @VentureID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PlotNumber", plotNumber);
                    cmd.Parameters.AddWithValue("@VentureID", ventureId);

                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private void Btn_addplot_Click(object sender, EventArgs e)
        {
            int ventureId = Convert.ToInt32(Cmbx_ventID.SelectedValue);
            int plotphase = Convert.ToInt32(Cmbx_plotphase.SelectedIndex);
            int status = Convert.ToInt32(Cmbx_plotstatus.SelectedIndex);
            string plotSize = Txt_plotsize.Text;
            string plotnumber = Txt_plotnumber.Text;
            string Imagepath = Txt_imagepath.Text;
            

            if (IsPlotNumberExists(plotnumber, ventureId))
            {
                MessageBox.Show("This plot number already exists for the selected venture.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }



            // Insert into database
            string connString = "Data Source=PROBYTE\\SQLEXPRESS;Initial Catalog=RealEstateTool;Integrated Security=True;Encrypt=False";
            string query = "INSERT INTO AddPlots (VentureID, Plotnumber, Plotsize, Plotphase, Status, Imagesource) " +
                           "VALUES (@VentureID, @Plotnumber, @Plotsize, @Plotphase, @Status, @Imagesource)";

            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@VentureID", Cmbx_ventID.SelectedValue);
                cmd.Parameters.AddWithValue("@PlotNumber", plotnumber);
                cmd.Parameters.AddWithValue("@Plotsize", plotSize);
                cmd.Parameters.AddWithValue("@Plotphase", Cmbx_plotphase.SelectedItem?.ToString());
                cmd.Parameters.AddWithValue("@Status", Cmbx_plotstatus.SelectedItem?.ToString());
                cmd.Parameters.AddWithValue("@Imagesource", Imagepath);

                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Plot added successfully.");
            }


        }

        private void Btn_editplotdetails_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Connstring))
                {
                    conn.Open();

                    string query = "UPDATE  AddPlots SET VentureID= @VentureID, Plotnumber= @Plotnumber, Plotsize=@Plotsize, Plotphase=@Plotphase, Status=@Status, Imagesource=@Imagesource WHERE PlotID = @PlotID";



                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PlotID", _plotId);
                        cmd.Parameters.AddWithValue("@VentureID", Cmbx_ventID.SelectedValue);
                        cmd.Parameters.AddWithValue("@PlotNumber", Txt_plotnumber.Text.Trim());
                        cmd.Parameters.AddWithValue("@Plotsize", Txt_plotsize.Text.Trim());
                        cmd.Parameters.AddWithValue("@Plotphase", Cmbx_plotphase.SelectedItem?.ToString());
                        cmd.Parameters.AddWithValue("@Status", Cmbx_plotstatus.SelectedItem?.ToString());
                        cmd.Parameters.AddWithValue("@Imagesource", Txt_imagepath.Text.Trim());

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                            MessageBox.Show("Plots updated successfully.");
                        else
                            MessageBox.Show("No records updated.");
                    }

                }
                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating plots: " + ex.Message);
            }
        }








        private void Txt_plotphase_TextChanged(object sender, EventArgs e)
        {

        }

        private void Txt_status_TextChanged(object sender, EventArgs e)
        {

        }

        private void Picturebox_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files (*.jpg;*.png;*.jpeg)|*.jpg;*.png;*.jpeg";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Txt_imagepath.Text = ofd.FileName;
                    Picturebox.Image = Image.FromFile(ofd.FileName);
                    Picturebox.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
        }

        private void Txt_imagepath_TextChanged(object sender, EventArgs e)
        {

        }

        private void Cmbx_ventID_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Cmbx_plotID_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Txt_plotsize_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void Btn_browseimage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files (*.jpg;*.png;*.jpeg)|*.jpg;*.png;*.jpeg";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Txt_imagepath.Text = ofd.FileName;
                    Picturebox.Image = Image.FromFile(ofd.FileName);
                    Picturebox.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
        }

        private void Btn_deleteplot_Click(object sender, EventArgs e)
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
                        string query = "DELETE FROM AddPlots WHERE PlotID = @id";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", _plotId);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Plot deleted successfully!");
                                this.DialogResult = DialogResult.OK;
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("No plot found to delete.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting plot: " + ex.Message);
                    }
                }
            }
        }

        private void Txt_plotnumber_TextChanged(object sender, EventArgs e)
        {

        }

        private void Cmbx_plotphase_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Cmbx_plotstatus_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Txt_plotsize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
