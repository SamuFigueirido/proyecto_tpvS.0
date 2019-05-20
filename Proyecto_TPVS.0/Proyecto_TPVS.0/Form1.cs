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
using System.Data;
using System.Data.OleDb;

namespace Proyecto_TPVS._0
{
    public partial class FormIniciarSesion : Form
    {
        SqlConnection connection;
        string connectionString;
        //KunLibertad_DesktopControl desktopControl;
        bool flag = true;
        public FormIniciarSesion()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
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
            //desktopControl = new KunLibertad_DesktopControl();
            //desktopControl.TaskBar(true);
            f.Size = Screen.PrimaryScreen.WorkingArea.Size;
            f.Location = Screen.PrimaryScreen.WorkingArea.Location;
            f.WindowState = FormWindowState.Maximized;
            //f.TopMost = true;
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
                if (connection != null)
                {
                    connection.Close();
                }
                //desktopControl.TaskBar(false);
            }
        }

        private void FormIniciarSesion_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin)
            {
                //desktopControl.TaskBar(false);
            }
        }

        private void FormIniciarSesion_Click(object sender, EventArgs e)
        {
            //desktopControl.TaskBar(true);
        }

        private void lblSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lblSalir_MouseEnter(object sender, EventArgs e)
        {
            ((Label)sender).Image = Properties.Resources.apagar_seleccionado;
        }

        private void lblSalir_MouseLeave(object sender, EventArgs e)
        {
            ((Label)sender).Image = Properties.Resources.apagar;
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
            string userName = txtUsuario.Text.Trim();
            string userPassword = txtContraseña.Text.Trim();
            if (userName == "" || userPassword == "")
            {
                MessageBox.Show("Usuario o contraseña incorrectos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string consultName = "";
                string consultPassword = "";
                SqlCommand query = new SqlCommand("select * from empleados where nombre = '" + userName + "'", connection);
                SqlDataReader dr = query.ExecuteReader();
                if (dr.Read())
                {
                    consultName = Convert.ToString(dr["nombre"]).Trim();
                    consultPassword = decrypt(Convert.ToString(dr["contraseña"]).Trim());
                }
                dr.Close();
                if (userName == consultName && userPassword == consultPassword)
                {
                    cambioDePanel(panelMenu, panelIniciarSesion);
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static string encrypt(string password)
        {
            string result = string.Empty;
            byte[] encryted = Encoding.Unicode.GetBytes(password);
            return Convert.ToBase64String(encryted);
        }

        public static string decrypt(string password)
        {
            string result = string.Empty;
            byte[] decryted = Convert.FromBase64String(password);
            return Encoding.Unicode.GetString(decryted);
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
            ((Label)sender).Image = Properties.Resources.atras_seleccionado;
        }

        private void labelAtras_MouseLeave(object sender, EventArgs e)
        {
            ((Label)sender).Image = Properties.Resources.atras;
        }

        private void lblAtras_Click(object sender, EventArgs e)
        {
            cambioDePanel(panelIniciarSesion, panelRegistrarUsuario);
        }

        private void lblRegistrarUsuario_Click(object sender, EventArgs e)
        {
            if (txtContraseñaRegistro.Text.Trim() == "" || txtConfirmarContraseña.Text.Trim() == "" || txtUsuarioRegistro.Text.Trim() == "")
            {
                MessageBox.Show("Completa todos los campos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string newName = txtUsuarioRegistro.Text.Trim();
                string consultName = "";
                SqlCommand query = new SqlCommand("select * from empleados where nombre = '" + newName + "'", connection);
                SqlDataReader dr = query.ExecuteReader();
                if (dr.Read())
                {
                    consultName = Convert.ToString(dr["nombre"]).Trim();
                }
                dr.Close();
                if (txtContraseñaRegistro.Text.Trim() == txtConfirmarContraseña.Text.Trim())
                {
                    if (consultName != newName)
                    {
                        string password = txtContraseñaRegistro.Text.Trim();
                        int id = 1;
                        do
                        {
                            try
                            {
                                query = new SqlCommand("insert into empleados (id, nombre, contraseña) values (" + id + ", '" + newName + "', '" + encrypt(password) + "')", connection);
                                int res = query.ExecuteNonQuery();
                                if (res > 0)
                                {
                                    MessageBox.Show("Nuevo usuario creado con éxito", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    cambioDePanel(panelMenu, panelRegistrarUsuario);
                                    return;
                                }
                            }
                            catch (SqlException)
                            {
                                id++;
                            }
                        } while (true);
                    }
                    else
                    {
                        MessageBox.Show("El nombre de usuario ya está en uso", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtUsuarioRegistro.Text = "";
                    }
                }
                else
                {
                    MessageBox.Show("Las contraseñas no coinciden", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtContraseñaRegistro.Text = "";
                    txtConfirmarContraseña.Text = "";
                }
            }
        }

        private void lblOpcionesSalir_Click(object sender, EventArgs e)
        {
            cambioDePanel(panelMenu, panelSeleccionado((Label)sender));
        }

        public Panel panelSeleccionado(Label lbl)
        {
            Panel panel;
            string option = lbl.Tag.ToString();
            if (option == "comedor")
            {
                panel = panelComedor;
            }
            else if (option == "almacen")
            {
                panel = panelAlmacen;
            }
            else if (option == "facturas")
            {
                panel = panelFacturas;
            }
            else if (option == "reservas")
            {
                panel = panelReservas;
            }
            else
            {
                panel = panelConfiguracion;
            }
            return panel;
        }

        private void lblConfiguracion_MouseEnter(object sender, EventArgs e)
        {
            ((Label)sender).Image = Properties.Resources.ajustes_seleccionado;
        }

        private void lblConfiguracion_MouseLeave(object sender, EventArgs e)
        {
            ((Label)sender).Image = Properties.Resources.ajustes;
        }

        private void lblComedor_Click(object sender, EventArgs e)
        {
            panelMenu.Visible = false;
            panelComedor.Visible = true;
        }

        private void lblAlmacen_Click(object sender, EventArgs e)
        {
            panelMenu.Visible = false;
            panelAlmacen.Visible = true;
        }

        private void lblFacturas_Click(object sender, EventArgs e)
        {
            panelMenu.Visible = false;
            panelFacturas.Visible = true;
        }

        private void lblReservas_Click(object sender, EventArgs e)
        {
            panelMenu.Visible = false;
            panelReservas.Visible = true;
        }

        private void lblConfiguracion_Click(object sender, EventArgs e)
        {
            panelMenu.Visible = false;
            panelConfiguracion.Visible = true;
        }

        private void txtCantMesas_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                e.Handled = true;
            }
        }

        private void btnAceptarCantMesas_Click(object sender, EventArgs e)
        {
            if (txtCantMesas.Text.Trim() == "" || Convert.ToInt32(txtCantMesas.Text) > 0 && Convert.ToInt32(txtCantMesas.Text) <= 20)
            {

            }
            else
            {
                MessageBox.Show("Introduce un número del 1-20", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
