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
    public partial class FormUser : Form
    {
        public FormUser()
        {
            InitializeComponent();
        }

        private void LoadDataUser()
        {
            // Memanggil method crud universal untuk menampilkan data user
            Koneksi.crud("SELECT * FROM users", dataGridView1);
        }

        private void FormUser_Load(object sender, EventArgs e)
        {
            LoadDataUser();
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Username dan Password tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = $"INSERT INTO users (username, password, nama_lengkap, role) " +
                           $"VALUES ('{txtUsername.Text}', '{txtPassword.Text}', '{txtNamaLengkap.Text}', '{cbRole.Text}')";

            Koneksi.crud(query);
            LoadDataUser();
            BersihkanForm();
        }

        private void btnUbah_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                MessageBox.Show("Pilih data user yang mau diubah dari tabel!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = $"UPDATE users SET password='{txtPassword.Text}', nama_lengkap='{txtNamaLengkap.Text}', role='{cbRole.Text}' " +
                           $"WHERE username='{txtUsername.Text}'";

            Koneksi.crud(query);
            LoadDataUser();
            BersihkanForm();
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                MessageBox.Show("Pilih user yang mau dihapus!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dr = MessageBox.Show("Apakah yakin ingin menghapus user ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                string query = $"DELETE FROM users WHERE username='{txtUsername.Text}'";
                Koneksi.crud(query);
                LoadDataUser();
                BersihkanForm();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtUsername.Text = row.Cells["username"].Value.ToString();
                txtPassword.Text = row.Cells["password"].Value.ToString();
                txtNamaLengkap.Text = row.Cells["nama_lengkap"].Value.ToString();
                cbRole.Text = row.Cells["role"].Value.ToString();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            BersihkanForm();
        }

        private void BersihkanForm()
        {
            txtUsername.Clear();
            txtPassword.Clear();
            txtNamaLengkap.Clear();
            cbRole.SelectedIndex = -1;
            txtUsername.Focus();
        }
    }
}