using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Newproject
{
    public partial class Form2 : Form
    {
        MySqlConnection conn = new MySqlConnection("Server=localhost;Database=PersonelDB;Uid=root;Pwd=root;");
        MySqlCommand cmd;
        MySqlDataAdapter adapter;
        DataTable dt;
        public Form2(DataGridView data)
        {
            InitializeComponent();
        }
        private void PlaniGoster(string[,] plan, string[] nobetYerleri, string[] gunler)
        {
            dataGridView2.Columns.Clear();
            dataGridView2.Rows.Clear();

            dataGridView2.Columns.Add("Yerler", "Nöbet Yerleri");
            foreach (string gun in gunler)
            {
                dataGridView2.Columns.Add(gun, gun);
            }

            for (int yer = 0; yer < nobetYerleri.Length; yer++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView2);

                row.Cells[0].Value = nobetYerleri[yer];

                for (int gun = 0; gun < gunler.Length; gun++)
                {
                    row.Cells[gun + 1].Value = plan[yer, gun];
                }

                dataGridView2.Rows.Add(row);
            }
        }

        private void btnPlanolust_Click(object sender, EventArgs e)
        {
            try
            {
                string[] nobetYerleri = { "Kat1", "Kat2", "Kat3", "Kat4", "Kat5", "Kat6", "Kat7", "Kat8", "Kat9", "Kat10" };
                string[] gunler = { "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi", "Pazar" };

                DataTable personeller = new DataTable();
                conn.Open();
                adapter = new MySqlDataAdapter("SELECT Ad, Soyad FROM Personeller", conn);
                adapter.Fill(personeller);
                conn.Close();

                if (personeller.Rows.Count < nobetYerleri.Length * gunler.Length)
                {
                    MessageBox.Show("Yeterli personel yok. Daha fazla personel ekleyin.");
                    return;
                }

                Random rand = new Random();
                var kalanPersoneller = personeller.Rows.Cast<DataRow>().ToList();
                string[,] plan = new string[nobetYerleri.Length, gunler.Length];

                for (int gun = 0; gun < gunler.Length; gun++)
                {
                    for (int yer = 0; yer < nobetYerleri.Length; yer++)
                    {
                        int rastgeleIndex = rand.Next(kalanPersoneller.Count);
                        DataRow personel = kalanPersoneller[rastgeleIndex];
                        plan[yer, gun] = $"{personel["Ad"]} {personel["Soyad"]}";
                        kalanPersoneller.RemoveAt(rastgeleIndex);
                    }
                }
                PlaniGoster(plan, nobetYerleri, gunler);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close();
            }
        }

        private void btnPdfOlustur_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "PDF dosyası|*.pdf";
                    saveFileDialog.Title = "PDF dosyasını kaydet";
                    saveFileDialog.FileName = "nobet_plani.pdf";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create))
                        {
                            Document doc = new Document(PageSize.A4.Rotate());
                            PdfWriter.GetInstance(doc, fs);
                            doc.Open();

                            Paragraph baslik = new Paragraph("Nöbet Plani", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 18, iTextSharp.text.Font.BOLD));
                            baslik.Alignment = Element.ALIGN_CENTER;
                            doc.Add(baslik);

                            doc.Add(new Paragraph("\n"));

                            PdfPTable table = new PdfPTable(dataGridView2.Columns.Count);
                            table.WidthPercentage = 100;

                            foreach (DataGridViewColumn column in dataGridView2.Columns)
                            {
                                PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText));
                                cell.BackgroundColor = new BaseColor(224, 116, 99);
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                table.AddCell(cell);
                            }

                            foreach (DataGridViewRow row in dataGridView2.Rows)
                            {
                                foreach (DataGridViewCell gridCell in row.Cells)
                                {
                                    string cellValue = gridCell.Value?.ToString() ?? string.Empty;
                                    PdfPCell cell = new PdfPCell(new Phrase(cellValue));
                                    cell.BackgroundColor = new BaseColor(99, 224, 180);
                                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                                    table.AddCell(cell);
                                }
                            }

                            doc.Add(table);

                            doc.Close();
                            MessageBox.Show("PDF başarıyla oluşturuldu!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}