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
using System.Data.SQLite;

namespace Proyecto_TPVS._0
{
    public partial class FormIniciarSesion : Form
    {
        //KunLibertad_DesktopControl desktopControl;
        ConnectionSQL connectionSQL;
        Pass encryptDecrypt = new Pass();
        List<string> datosNombres;
        public FormIniciarSesion()
        {
            InitializeComponent();
        }

        private void FormIniciarSesion_Load(object sender, EventArgs e)
        {
            ajustarPaneles();
            connectionSQL = new ConnectionSQL();
            connectionSQL.createBD();
            foreach (string user in connectionSQL.empleados())
            {
                Console.WriteLine(user);
            }
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
                connectionSQL.cerrarConexion();
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
                if (connectionSQL.loginEmpleado(userName, userPassword))
                {
                    cambioDePanel(panelMenu, panelIniciarSesion);
                }
            }
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

        private void lblAtras_MouseEnter(object sender, EventArgs e)
        {
            ((Label)sender).Image = Properties.Resources.atras_seleccionado;
        }

        private void lblAtras_MouseLeave(object sender, EventArgs e)
        {
            ((Label)sender).Image = Properties.Resources.atras;
        }

        private void lblAtras_Click(object sender, EventArgs e)
        {
            cambioDePanel(panelIniciarSesion, panelRegistrarUsuario);
            txtUsuario.Text = "";
            txtContraseña.Text = "";
        }

        private void lblRegistrarUsuario_Click(object sender, EventArgs e)
        {
            if (txtContraseñaRegistro.Text.Trim() == "" || txtConfirmarContraseña.Text.Trim() == "" || txtUsuarioRegistro.Text.Trim() == "")
            {
                MessageBox.Show("Completa todos los campos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (txtContraseñaRegistro.Text.Trim() == txtConfirmarContraseña.Text.Trim())
            {
                string newName = txtUsuarioRegistro.Text.Trim();
                string password = txtContraseñaRegistro.Text.Trim();
                if (connectionSQL.addEmpleado(newName, password))
                {
                    cambioDePanel(panelMenu, panelRegistrarUsuario);
                }
            }
            else
            {
                MessageBox.Show("Las contraseñas no coinciden", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtContraseñaRegistro.Text = "";
                txtConfirmarContraseña.Text = "";
            }
        }

        private void lblOpcionesAtras_Click(object sender, EventArgs e)
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
            listBoxTablas.DataSource = connectionSQL.almacen();
            foreach (string tabla in connectionSQL.almacen())
            {
                Console.WriteLine(tabla);
            }
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

        private void lblCerrarSesion_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("¿Seguro que deseas cerrar sesión?", "Salir", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.Cancel)
            {
                txtUsuario.Text = "";
                txtContraseña.Text = "";
                cambioDePanel(panelIniciarSesion, panelConfiguracion);
            }
        }

        private void lblBorrarUsuario_Click(object sender, EventArgs e)
        {
            cambioDePanel(panelBorrarUsuario, panelConfiguracion);
            listBoxUsuarios.DataSource = connectionSQL.empleados();
        }

        private void lblAtrasBorrarUsuario_Click(object sender, EventArgs e)
        {
            cambioDePanel(panelConfiguracion, panelBorrarUsuario);
        }

        private void btnBorrarUsuario_Click(object sender, EventArgs e)
        {
            string nombre = listBoxUsuarios.SelectedValue.ToString();
            string query = "delete from [empleados] where nombre='" + nombre + "'";
            Console.WriteLine(query);
            connectionSQL.abrirConexion();
            if (listBoxUsuarios.Items.Count == 1)
            {
                if (MessageBox.Show("¿Seguro que deseas eliminar el último usuario?\nVolverás a la pantalla de registro", "Eliminar usuario", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.Cancel)
                {
                    connectionSQL.executeQuery(query);
                    cambioDePanel(panelRegistrarUsuario, panelBorrarUsuario);
                }
            }
            else
            {
                connectionSQL.executeQuery(query);
            }
            connectionSQL.cerrarConexion();
            listBoxUsuarios.DataSource = connectionSQL.empleados();
        }

        private void listBoxTablas_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBoxDatos.Visible = true;
            listBoxDatos.DataSource = connectionSQL.datosAlmacen(listBoxTablas.SelectedValue.ToString());
            lblTablaSeleccionada.Text = "Tabla seleccionada: " + listBoxTablas.SelectedValue.ToString();
            datosNombres = connectionSQL.datosAlmacenNombres(listBoxTablas.SelectedValue.ToString());
            foreach (string nombres in datosNombres)
            {
                Console.WriteLine(nombres);
            }
        }

        private void btnEliminarDato_Click(object sender, EventArgs e)
        {
            try
            {
                string tabla = listBoxTablas.SelectedValue.ToString();
                int pos = listBoxDatos.SelectedIndex;
                string nombre = connectionSQL.datosAlmacenNombres(tabla)[pos];
                Console.WriteLine(tabla + " " + pos + " " + nombre);
                connectionSQL.deleteDatos(tabla, nombre);
                listBoxDatos.DataSource = connectionSQL.datosAlmacen(listBoxTablas.SelectedValue.ToString());
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("No hay más datos para eliminar", "Tabla vacía", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnInsertarDato_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtNombre.Text.Trim() == "" || txtCantidad.Text.Trim() == "" || txtPrecio.Text.Trim() == "")
                {
                    MessageBox.Show("Completa todos los campos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    connectionSQL.insertDatos(listBoxTablas.SelectedValue.ToString(), txtNombre.Text.Trim(), Convert.ToInt32(txtCantidad.Text.Trim()), Convert.ToDouble(txtPrecio.Text));
                    listBoxDatos.DataSource = connectionSQL.datosAlmacen(listBoxTablas.SelectedValue.ToString());
                    txtNombre.Text= "";
                    txtCantidad.Text = "";
                    txtPrecio.Text= "";
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Error al introducir los campos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
