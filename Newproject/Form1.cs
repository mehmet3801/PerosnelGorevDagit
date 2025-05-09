using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;

namespace Newproject
{
    public partial class Form1 : Form
    {
        MySqlConnection conn = new MySqlConnection("Server=localhost;Database=PersonelDB;Uid=root;Pwd=root;");
        MySqlCommand cmd;
        MySqlDataAdapter adapter;
        DataTable dt;

        public Form1()
        {
            InitializeComponent();
        }

        void VeriGetir()
        {
            try
            {
                dt = new DataTable();
                conn.Open();
                adapter = new MySqlDataAdapter("SELECT * FROM Personeller", conn);
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close();
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int selectedId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);
                    string sql = "DELETE FROM Personeller WHERE Id=@id";
                    cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", selectedId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    VeriGetir();
                    MessageBox.Show("Kayıt başarıyla silindi.");
                }
                else
                {
                    MessageBox.Show("Lütfen silmek için bir kayıt seçin.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close();
            }
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int selectedId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);
                    string sql = "UPDATE Personeller SET Ad=@ad, Soyad=@soyad, Telefon=@telefon, Sehir=@sehir WHERE Id=@id";
                    cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", selectedId);
                    cmd.Parameters.AddWithValue("@ad", txtAd.Text);
                    cmd.Parameters.AddWithValue("@soyad", txtSoyad.Text);
                    cmd.Parameters.AddWithValue("@telefon", txtTel.Text);
                    cmd.Parameters.AddWithValue("@sehir", txtSEHİR.Text);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    VeriGetir();
                    MessageBox.Show("Kayıt başarıyla güncellendi.");

                    dataGridView1.Columns[0].Width = 50; // 0. sütunun genişliğini 100 piksel yapar
                    dataGridView1.Columns[1].Width = 150; // 1. sütunun genişliğini 150 piksel yapar
                }
                else
                {
                    MessageBox.Show("Lütfen güncellemek için bir kayıt seçin.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            VeriGetir();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtAd.Text = dataGridView1.Rows[e.RowIndex].Cells["Ad"].Value.ToString();
                txtSoyad.Text = dataGridView1.Rows[e.RowIndex].Cells["Soyad"].Value.ToString();
                txtTel.Text = dataGridView1.Rows[e.RowIndex].Cells["Telefon"].Value.ToString();
                txtSEHİR.Text = dataGridView1.Rows[e.RowIndex].Cells["Sehir"].Value.ToString();
            }
        }

        private void btnTemizle_Click(object sender, EventArgs e)
        {
            txtAd.Clear();
            txtSoyad.Clear();
            txtSEHİR.Clear();
            txtTel.Clear();

        }



        private void btnEkle_click(object sender, EventArgs e)
        {
            try
            {
                string sorgu = "INSERT INTO Personeller (Ad, Soyad, Telefon, Sehir) VALUES (@ad, @soyad, @telefon, @sehir)";
                cmd = new MySqlCommand(sorgu, conn);
                cmd.Parameters.AddWithValue("@ad", txtAd.Text);
                cmd.Parameters.AddWithValue("@soyad", txtSoyad.Text);
                cmd.Parameters.AddWithValue("@telefon", txtTel.Text);
                cmd.Parameters.AddWithValue("@sehir", txtSEHİR.Text);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                VeriGetir();
                MessageBox.Show("Kayıt başarıyla eklendi.");
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close();
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            VeriGetir();
            DataGridViewAyarla();

        }

        void DataGridViewAyarla()
        {
            dataGridView1.Columns[0].Width = 36;
            dataGridView1.Columns[1].Width = 80;
            dataGridView1.Columns[2].Width = 80;
            dataGridView1.Columns[3].Width = 76;
            dataGridView1.Columns[4].Width = 80;

            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 yeniForm = new Form2(dataGridView1);
            yeniForm.Show();
        }
    }
}
