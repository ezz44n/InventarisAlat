using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace InventarisAlat
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Validasi input kosong
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Username dan Password tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = Koneksi.GetConnection())
                {
                    conn.Open();
                    // Query untuk mengecek data user berdasarkan input
                    string query = "SELECT * FROM users WHERE username = @username AND password = @password";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Menggunakan parameter untuk keamanan dari SQL Injection
                        cmd.Parameters.AddWithValue("@username", txtUsername.Text);
                        cmd.Parameters.AddWithValue("@password", txtPassword.Text);

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            // Jika data ditemukan (baris > 0)
                            if (dt.Rows.Count > 0)
                            {
                                string namaLengkap = dt.Rows[0]["nama_lengkap"].ToString();
                                string role = dt.Rows[0]["role"].ToString();

                                MessageBox.Show($"Login Berhasil!\nSelamat datang, {namaLengkap} ({role})", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Buka Form Utama
                                MDIParent1 utama = new MDIParent1();
                                utama.Show();
                                this.Hide(); // Sembunyikan form login
                            }
                            else
                            {
                                MessageBox.Show("Username atau Password salah!", "Gagal Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtPassword.Clear();
                                txtPassword.Focus();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan koneksi: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}