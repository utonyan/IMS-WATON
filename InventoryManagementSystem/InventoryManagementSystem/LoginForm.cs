using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventoryManagementSystem
{

    public partial class LoginForm : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Hideru\Documents\dbIMS.mdf;Integrated Security=True;Connect Timeout=30");
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;
        public LoginForm()
        {
            InitializeComponent();
        }

        private void checkBoxPass_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxPass.Checked == false)
                txtPass.UseSystemPasswordChar = true;
            else
                txtPass.UseSystemPasswordChar = false;
        }

        private void lblClear_Click(object sender, EventArgs e)
        {
            txtUsername.Clear();
            txtPass.Clear();
        }

        private void pictureBox_Close_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Exit Application","Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) 
            { 
                Application.Exit();
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                cm = new SqlCommand("SELECT * FROM tbUser WHERE username=@username AND password=@password", con);
                cm.Parameters.AddWithValue("@username",txtUsername.Text);
                cm.Parameters.AddWithValue("@password", txtPass.Text);
                con.Open();
                dr = cm.ExecuteReader();
                dr.Read();
                if(dr.HasRows)
                {
                    MessageBox.Show("Welcome "+ dr["fullname"].ToString() + "! ", "ACCESS GRANTED", MessageBoxButtons.OK, MessageBoxIcon.Information );
                    MainForm main = new MainForm();
                    main.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid username or password!", "ACCESS DENIED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
public class RoundedTextBox : TextBox
{
    private const int borderRadius = 10; // Adjust this value to change the roundness

    protected override void OnPaint(PaintEventArgs e)
    {
        // Create a graphics path to define the rounded shape
        GraphicsPath path = new GraphicsPath();
        path.AddArc(0, 0, borderRadius, borderRadius, 180, 90);
        path.AddArc(Width - borderRadius, 0, borderRadius, borderRadius, 270, 90);
        path.AddArc(Width - borderRadius, Height - borderRadius, borderRadius, borderRadius, 0, 90);
        path.AddArc(0, Height - borderRadius, borderRadius, borderRadius, 90, 90);

        // Set the textbox region to the custom shape
        this.Region = new Region(path);

        // Call the base OnPaint method to draw the text
        base.OnPaint(e);

        RoundedTextBox roundedTextBox = new RoundedTextBox();
        roundedTextBox.Size = new Size(200, 30);
        roundedTextBox.Location = new Point(50, 50);
        this.Controls.Add(roundedTextBox);
    }

}