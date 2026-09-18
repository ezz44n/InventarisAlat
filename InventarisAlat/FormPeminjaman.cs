using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace InventarisAlat
{
    public partial class FormPeminjaman : Form
    {
        private string selectedID = "";

        public FormPeminjaman()
        {
            InitializeComponent();
        }

        private void FormPeminjaman_Load(object sender, EventArgs e)
        {
            TampilData();
            LoadDataAlat();
            InitStatus();
        }

        // 1. Tampil Data Peminjaman ke DataGridView (JOIN dengan tabel Alat untuk ambil nama_alat)
        private void TampilData()
        {
            string query = @"SELECT p.id, p.alat_id, a.nama_alat, p.nama_peminjam, p.jumlah_pinjam, 
                                    p.tanggal_pinjam, p.tanggal_kembali_rencana, p.status 
                             FROM peminjaman p 
                             JOIN alat a ON p.alat_id = a.id";
            Koneksi.crud(query, dgvPeminjaman);
        }

        // 2. Load Data Alat ke ComboBox
        private void LoadDataAlat()
        {
            string query = "SELECT id, nama_alat FROM alat";
            using (MySqlConnection conn = Koneksi.GetConnection())
            {
                try
                {
                    conn.Open();
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmbAlat.DataSource = dt;
                    cmbAlat.DisplayMember = "nama_alat";
                    cmbAlat.ValueMember = "id";
                    cmbAlat.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memuat data alat: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 3. Inisialisasi Pilihan Status
        private void InitStatus()
        {
            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("Dipinjam");
            cmbStatus.Items.Add("Kembali");
            cmbStatus.SelectedIndex = 0;
        }

        // 4. Tombol SIMPAN
        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (cmbAlat.SelectedValue == null || string.IsNullOrWhiteSpace(txtNamaPeminjaman.Text) || string.IsNullOrWhiteSpace(txtJumlah.Text))
            {
                MessageBox.Show("Semua kolom inputan wajib diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string alatId = cmbAlat.SelectedValue.ToString();
            int jumlahPinjam = Convert.ToInt32(txtJumlah.Text);
            int jumlahKembali = Convert.ToInt32(txtJumlah.Text);
            string tglPinjam = dtpTanggalPinjam.Value.ToString("yyyy-MM-dd");
            string tglKembaliRencana = dtpTanggalKembali.Value.ToString("yyyy-MM-dd");

            using (MySqlConnection conn = Koneksi.GetConnection())
            {
                try
                {
                    conn.Open();

                    // STEP A: Cek ketersediaan stok alat di tabel alat
                    string queryCekStok = $"SELECT jumlah FROM alat WHERE id = {alatId}";
                    MySqlCommand cmdCek = new MySqlCommand(queryCekStok, conn);
                    int stokTersedia = Convert.ToInt32(cmdCek.ExecuteScalar());

                    // Validasi batas jumlah peminjaman
                    if (jumlahPinjam > stokTersedia)
                    {
                        MessageBox.Show($"Stok alat tidak mencukupi! Stok tersedia saat ini: {stokTersedia}", "Peringatan Stok", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // STEP B: Insert data transaksi ke tabel peminjaman
                    string queryInsert = $@"INSERT INTO peminjaman (alat_id, nama_peminjam, jumlah_pinjam, tanggal_pinjam, tanggal_kembali_rencana, status) 
                                           VALUES ({alatId}, '{txtNamaPeminjaman.Text}', {jumlahPinjam}, '{tglPinjam}', '{tglKembaliRencana}', '{cmbStatus.SelectedItem}')";

                    MySqlCommand cmdInsert = new MySqlCommand(queryInsert, conn);
                    cmdInsert.ExecuteNonQuery();

                    // STEP B: Insert data transaksi ke tabel peminjaman
                    string queryins = $@"INSERT INTO peminjaman (alat_id, nama_peminjam, jumlah_pinjam, tanggal_pinjam, tanggal_kembali_rencana, status) 
                                           VALUES ({alatId}, '{txtNamaPeminjaman.Text}', {jumlahKembali}, '{tglPinjam}', '{tglKembaliRencana}', '{cmbStatus.SelectedItem}')";

                    MySqlCommand cmdins = new MySqlCommand(queryInsert, conn);
                    cmdInsert.ExecuteNonQuery();

                    // STEP C: Kurangi stok alat jika statusnya 'Dipinjam'
                    if (cmbStatus.SelectedItem.ToString() == "Dipinjam")
                    {
                        string queryUpdateStok = $"UPDATE alat SET jumlah = jumlah - {jumlahPinjam} WHERE id = {alatId}";
                        MySqlCommand cmdUpdate = new MySqlCommand(queryUpdateStok, conn);
                        cmdUpdate.ExecuteNonQuery();
                    }

                    if (cmbStatus.SelectedItem.ToString() == "Kembali")
                    {
                        string queryUpdateStok = $"UPDATE alat SET jumlah = jumlah + {jumlahKembali} WHERE id = {alatId}";
                        MySqlCommand cmdUpdate = new MySqlCommand(queryUpdateStok, conn);
                        cmdUpdate.ExecuteNonQuery();
                    }

                    MessageBox.Show("Transaksi peminjaman berhasil", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ResetForm();
                    TampilData();
                    LoadDataAlat(); // Refresh pilihan alat untuk memperbarui stok
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Terjadi Kesalahan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 5. Tombol UPDATE
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedID))
            {
                MessageBox.Show("Pilih data yang ingin diubah dari tabel terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string alatId = cmbAlat.SelectedValue.ToString();
            string tglPinjam = dtpTanggalPinjam.Value.ToString("yyyy-MM-dd");
            string tglKembaliRencana = dtpTanggalKembali.Value.ToString("yyyy-MM-dd");

            string query = $@"UPDATE peminjaman 
                             SET alat_id={alatId}, nama_peminjam='{txtNamaPeminjaman.Text}', jumlah_pinjam={txtJumlah.Text}, 
                                 tanggal_pinjam='{tglPinjam}', tanggal_kembali_rencana='{tglKembaliRencana}', status='{cmbStatus.SelectedItem}' 
                             WHERE id={selectedID}";

            Koneksi.crud(query);
            ResetForm();
            TampilData();
        }

        // 6. Tombol HAPUS
        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedID))
            {
                MessageBox.Show("Pilih data yang ingin dihapus dari tabel terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Apakah Anda yakin ingin menghapus data transaksi ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string query = $"DELETE FROM peminjaman WHERE id={selectedID}";
                Koneksi.crud(query);
                ResetForm();
                TampilData();
            }
        }

        // 7. Tombol RESET
        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            selectedID = "";
            if (cmbAlat.Items.Count > 0) cmbAlat.SelectedIndex = -1;
            txtNamaPeminjaman.Clear();
            dtpTanggalPinjam.Value = DateTime.Now;
            dtpTanggalKembali.Value = DateTime.Now;
            txtJumlah.Clear();
            cmbStatus.SelectedIndex = 0;
        }

        // 8. Event CellClick DataGridView
        private void dgvPeminjaman_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvPeminjaman.Rows[e.RowIndex];
                selectedID = row.Cells["id"].Value.ToString();
                cmbAlat.SelectedValue = row.Cells["alat_id"].Value;
                txtNamaPeminjaman.Text = row.Cells["nama_peminjam"].Value.ToString();
                txtJumlah.Text = row.Cells["jumlah_pinjam"].Value.ToString();
                dtpTanggalPinjam.Value = Convert.ToDateTime(row.Cells["tanggal_pinjam"].Value);
                dtpTanggalKembali.Value = Convert.ToDateTime(row.Cells["tanggal_kembali_rencana"].Value);
                cmbStatus.SelectedItem = row.Cells["status"].Value.ToString();
            }
        }
    }
}