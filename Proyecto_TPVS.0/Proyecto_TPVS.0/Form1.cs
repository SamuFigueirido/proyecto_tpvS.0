using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DesktopControl;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections;

namespace Proyecto_TPVS._0
{
    public partial class FormIniciarSesion : Form
    {
        SqlConnection connection;
        string connectionString;
        KunLibertad_DesktopControl desktopControl;
        public FormIniciarSesion()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["Proyecto_TPVS._0.Properties.Settings.DataBaseConnectionString"].ConnectionString;
        }

        private void FormIniciarSesion_Load(object sender, EventArgs e)
        {
            ajustarPaneles();
            connection = new SqlConnection(connectionString);
            connection.Open();
            pantallaCompleta(this);
        }

        public void pantallaCompleta(Form f)
        {
            desktopControl = new KunLibertad_DesktopControl();
            desktopControl.TaskBar(true);
            f.Size = Screen.PrimaryScreen.WorkingArea.Size;
            f.Location = Screen.PrimaryScreen.WorkingArea.Location;
            f.WindowState = FormWindowState.Maximized;
            f.TopMost = true;
            f.FormBorderStyle = FormBorderStyle.None;
        }

        private void FormIniciarSesion_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("¿Seguro que deseas salir?", "Salir", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
            else
            {
                if(connection != null)
                {
                    connection.Close();
                }
                desktopControl.TaskBar(false);
            }
        }

        private void FormIniciarSesion_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin)
            {
                desktopControl.TaskBar(false);
            }
        }

        private void FormIniciarSesion_Click(object sender, EventArgs e)
        {
            desktopControl.TaskBar(true);
        }

        private void lblSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lblSalir_MouseEnter(object sender, EventArgs e)
        {
            lblSalir.Image = Properties.Resources.apagar_seleccionado;
        }

        private void lblSalir_MouseLeave(object sender, EventArgs e)
        {
            lblSalir.Image = Properties.Resources.apagar;
        }

        private void lbl_MouseLeave(object sender, EventArgs e)
        {
            ((Label)sender).BackColor = Color.Empty;
            ((Label)sender).ForeColor = Color.Black;
        }

        private void lbl_MouseEnter(object sender, EventArgs e)
        {
            ((Label)sender).BackColor = Color.Black;
            ((Label)sender).ForeColor = Color.White;
        }

        private void lblRegistrarse_Click(object sender, EventArgs e)
        {
            cambioDePanel(panelRegistrarUsuario, panelIniciarSesion);
        }

        private void lblIniciarSesion_Click(object sender, EventArgs e)
        {
            cambioDePanel(panelMenu, panelIniciarSesion);
        }

        private void cambioDePanel(Panel panelVisible, Panel panelNoVisible)
        {
            panelVisible.Visible = true;
            panelNoVisible.Visible = false;
        }

        private void ajustarPaneles()
        {
            foreach (Control control in this.Controls)
            {
                if (control is Panel)
                {
                    control.Dock = DockStyle.Fill;
                }
            }
        }

        private void labelAtras_MouseEnter(object sender, EventArgs e)
        {
            lblAtras.Image = Properties.Resources.atras_seleccionado;
        }

        private void labelAtras_MouseLeave(object sender, EventArgs e)
        {
            lblAtras.Image = Properties.Resources.atras;
        }

        private void lblAtras_Click(object sender, EventArgs e)
        {
            cambioDePanel(panelIniciarSesion, panelRegistrarUsuario);
        }

        private void lblRegistrarUsuario_Click(object sender, EventArgs e)
        {
            //TODO comprobación si no existe un usuario con el mismo nombre
            cambioDePanel(panelMenu, panelRegistrarUsuario);
        }
    }
}
