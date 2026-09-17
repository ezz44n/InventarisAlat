using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace InventarisAlat
{
    internal class Koneksi
    {
        // Perhatikan nama database diubah menjadi 'dbinventarisalat' sesuai file SQL
        private static string connectionString = "server=localhost;database=dbinventarisalat;uid=root;pwd=;";

        public static MySqlConnection GetConnection()
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }

        // Method CRUD universal yang sudah kita sepakati sebelumnya
        public static void crud(string query, DataGridView dgv = null)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();

                    if (query.Trim().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            if (dgv != null)
                            {
                                dgv.DataSource = dt;
                            }
                        }
                    }
                    else
                    {
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Berhasil mengeksekusi data!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kesalahan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}