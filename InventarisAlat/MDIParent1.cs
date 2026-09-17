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
    public partial class MDIParent1 : Form
    {
        private int childFormNumber = 0;

        public MDIParent1()
        {
            InitializeComponent();
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            FormAlat childForm = new FormAlat();
            childForm.MdiParent = this;
            childForm.Text = "Window " + childFormNumber++;
            childForm.Show();
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void TutupFormAnakAktif()
        {
            foreach (Form child in this.MdiChildren)
            {
                child.Close();
            }
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void menuDataAlat_Click(object sender, EventArgs e)
        {
            TutupFormAnakAktif();
            FormAlat frmAlat = new FormAlat();
            frmAlat.MdiParent = this; 
            frmAlat.Show();
        }

        private void menuUser_Click(object sender, EventArgs e)
        {
            TutupFormAnakAktif();
            FormUser frmUser = new FormUser();
            frmUser.MdiParent = this;
            frmUser.Show();
        }

        private void menuPeminjaman_Click(object sender, EventArgs e)
        {
            TutupFormAnakAktif();
            FormPeminjaman frmPinjam = new FormPeminjaman();
            frmPinjam.MdiParent = this;
            frmPinjam.Show();
        }

        private void menuLogout_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Apakah Anda yakin ingin keluar?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                FormLogin login = new FormLogin();
                login.Show();
                this.Close();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            TutupFormAnakAktif();
            FormAlat frmAlat = new FormAlat();
            frmAlat.MdiParent = this;
            frmAlat.Show();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            TutupFormAnakAktif();
            FormPeminjaman frmPinjam = new FormPeminjaman();
            frmPinjam.MdiParent = this;
            frmPinjam.Show();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            TutupFormAnakAktif();
            DialogResult dr = MessageBox.Show("Apakah Anda yakin ingin keluar?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                FormLogin login = new FormLogin();
                login.Show();
                this.Close();
            }
        }

        private void MDIParent1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
