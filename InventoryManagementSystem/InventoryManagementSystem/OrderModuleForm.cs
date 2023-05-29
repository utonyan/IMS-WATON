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

namespace InventoryManagementSystem
{
    public partial class OrderModuleForm : Form
    {
        //Sql Connection
        SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Hideru\\Documents\\dbIMS.mdf;Integrated Security=True;Connect Timeout=30");
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;
        int qty = 0;
        public OrderModuleForm()
        {
            //Loads shit up
            InitializeComponent();
            LoadCustomer();
            LoadProduct();
        }

        public void LoadCustomer()
        {
            //gets data to tbCustomer in the database and writes the table
            int i = 0;
            dgvCustomer.Rows.Clear();
            cm = new SqlCommand("SELECT cid,cname FROM tbCustomer WHERE CONCAT(cid,cname) LIKE '%"+txtSearchC.Text+"%'", con);
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dgvCustomer.Rows.Add(i, dr[0].ToString(), dr[1].ToString());
            }
            dr.Close();
            con.Close();
        }

        public void LoadProduct()
        {
            //gets data to tbProduct in the database and writes the table
            int i = 0;
            dgvProduct.Rows.Clear();
            cm = new SqlCommand("SELECT * FROM tbProduct WHERE CONCAT(pid, pname, pprice, pdescription, pcategory) LIKE '%" + txtSearchP.Text + "%'", con);
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dgvProduct.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString());
            }
            dr.Close();
            con.Close();
        }

        private void txtSearchC_TextChanged(object sender, EventArgs e)
        {
            //loads command for searchbar for customer
            LoadCustomer();
        }

        private void txtSearchP_TextChanged(object sender, EventArgs e)
        {
            //loads command for searchbar for product
            LoadProduct();
        }


        private void dgvCustomer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //cell selection for customer and adds data to textboxes
            txtcid.Text = dgvCustomer.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtcname.Text = dgvCustomer.Rows[e.RowIndex].Cells[2].Value.ToString();
        }
        private void dgvProduct_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //Cell selection for product and adds data to textboxes
            txtpid.Text = dgvProduct.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtpname.Text = dgvProduct.Rows[e.RowIndex].Cells[2].Value.ToString();
            txtprice.Text = dgvProduct.Rows[e.RowIndex].Cells[4].Value.ToString();
            //if left enabled shit bugs lmaooo
            UDQty.Enabled = true;
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            GetQty();
            //This limits the stock
            if (Convert.ToInt16(UDQty.Value) > qty)
            {
                MessageBox.Show("Instock quantity is not enought!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                UDQty.Value = UDQty.Value - 1;
                return;
            }
            if (Convert.ToInt16(UDQty.Value) > 0)
            {
                //Gets the Total amount of price
                int total = Convert.ToInt16(txtprice.Text) * Convert.ToInt16(UDQty.Value);
                txttotal.Text = total.ToString();
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                //when no customer is selected
                if (txtcid.Text == "")
                {
                    MessageBox.Show("Please select customer!!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                //when no product is selected
                if (txtpid.Text == "")
                {
                    MessageBox.Show("Please select product!!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                // this shit inserts the order in the sql database
                if (MessageBox.Show("Are you sure you want to insert this order?", "Saving Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cm = new SqlCommand("INSERT INTO tbOrder(odate, pid, cid, qty, price, total)VALUES(@odate, @pid, @cid, @qty, @price, @total)", con);
                    cm.Parameters.AddWithValue("@odate", dtOrder.Value);
                    cm.Parameters.AddWithValue("@pid", Convert.ToInt16(txtpid.Text));
                    cm.Parameters.AddWithValue("@cid", Convert.ToInt16(txtcid.Text));
                    cm.Parameters.AddWithValue("@qty", Convert.ToInt16(UDQty.Value));
                    cm.Parameters.AddWithValue("@price", Convert.ToInt16(txtprice.Text));
                    cm.Parameters.AddWithValue("@total", Convert.ToInt16(txttotal.Text));
                    con.Open();
                    cm.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("Order has been successfully saved.");
                    


                    //subtracts the product quantity by the order amount
                    cm = new SqlCommand("UPDATE tbProduct SET pqty = (pqty-@pqty) WHERE pid LIKE '" + txtpid.Text + "'", con);
                    cm.Parameters.AddWithValue("@pqty", Convert.ToInt16(UDQty.Text));

                    con.Open();
                    cm.ExecuteNonQuery();
                    con.Close();
                    Clear();
                    LoadProduct();

                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }

        }
        public void Clear()
        {
            //Clear function (yes it fucking is)
            txtcname.Clear();
            txtcid.Clear();

            txtpid.Clear();
            txtpname.Clear();

            txtprice.Clear();
            UDQty.Value = 0;
            txttotal.Clear();
            dtOrder.Value = DateTime.Now;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            //clear button itself
            Clear();
        }


        public void GetQty()
        {
            cm = new SqlCommand("SELECT pqty FROM tbProduct WHERE pid='" + txtpid.Text + "'", con);
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                qty = Convert.ToInt32(dr[0].ToString());
            }
            dr.Close();
            con.Close();
        }
    }
}
