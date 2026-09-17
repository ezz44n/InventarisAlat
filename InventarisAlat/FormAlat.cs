using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventarisAlat
{
    public partial class FormAlat : Form
    {
        private string idSelected = "";
        public FormAlat()
        {
            InitializeComponent();
        }

        // Method untuk merefresh/menampilkan data ke DataGridView
        private void LoadData()
        {
            // Memanggil method crud universal untuk SELECT
            Koneksi.crud("SELECT * FROM alat", dataGridView1);
        }

        private void FormAlat_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        // Tombol SIMPAN (Create)
        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtKode.Text) || string.IsNullOrEmpty(txtNama.Text))
            {
                MessageBox.Show("Kode dan Nama Alat tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Menggunakan string interpolation ($) sesuai gaya kamu
            string query = $"INSERT INTO alat (id, kode_alat, nama_alat, kategori, jumlah, kondisi, lokasi) " +
                           $"VALUES ('null','{txtKode.Text}', '{txtNama.Text}', '{txtKategori.Text}', '{txtJumlah.Text}', '{cbKondisi.Text}', '{txtLokasi.Text}')";

            Koneksi.crud(query);
            LoadData();
            BersihkanForm();
        }

        // Tombol UBAH (Update)
        private void btnUbah_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtKode.Text))
            {
                MessageBox.Show("Pilih data yang mau diubah terlebih dahulu dari tabel!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Menggunakan kode_alat sebagai acuan update
            string query = $"UPDATE alat SET nama_alat='{txtNama.Text}', kategori='{txtKategori.Text}', " +
                           $"jumlah='{txtJumlah.Text}', kondisi='{cbKondisi.Text}', lokasi='{txtLokasi.Text}' " +
                           $"WHERE kode_alat='{txtKode.Text}'";

            Koneksi.crud(query);
            LoadData();
            BersihkanForm();
        }

        // Tombol HAPUS (Delete)
        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtKode.Text))
            {
                MessageBox.Show("Pilih data yang mau dihapus!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dr = MessageBox.Show("Apakah yakin ingin menghapus data ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                string query = $"DELETE FROM alat WHERE kode_alat='{txtKode.Text}'";
                Koneksi.crud(query);
                LoadData();
                BersihkanForm();
            }
        }

        // Event saat baris di DataGridView diklik untuk otomatis masuk ke TextBox
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Simpan ID tersembunyi (sesuaikan nama kolom ID di database kamu, misal "id_alat")
                idSelected = row.Cells["id"].Value.ToString();
                txtKode.Text = row.Cells["kode_alat"].Value.ToString();
                txtNama.Text = row.Cells["nama_alat"].Value.ToString();
                txtKategori.Text = row.Cells["kategori"].Value.ToString();
                txtJumlah.Text = row.Cells["jumlah"].Value.ToString();
                cbKondisi.Text = row.Cells["kondisi"].Value.ToString();
                txtLokasi.Text = row.Cells["lokasi"].Value.ToString();
            }
        }

        // Tombol Reset / Bersihkan Form
        private void btnReset_Click(object sender, EventArgs e)
        {
            BersihkanForm();
        }

        private void BersihkanForm()
        {
            txtKode.Clear();
            txtNama.Clear();
            txtKategori.Clear();
            txtJumlah.Clear();
            cbKondisi.SelectedIndex = -1;
            txtLokasi.Clear();
            txtKode.Focus();
        }
    }
}