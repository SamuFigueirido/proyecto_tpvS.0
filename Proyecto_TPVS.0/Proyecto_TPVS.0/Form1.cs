﻿using System;
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
using System.Reflection;

namespace Proyecto_TPVS._0
{
    public partial class FormIniciarSesion : Form
    {
        //KunLibertad_DesktopControl desktopControl;
        ConnectionSQL connectionSQL;
        Pass encryptDecrypt = new Pass();
        List<string> datosNombres;
        List<Label> mesas;
        Label mesa;
        Panel panel;
        bool flag = true;
        List<Panel> panelesMesas;

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
            if (txtCantMesas.Text.Trim() == "" || Convert.ToInt32(txtCantMesas.Text) > 0 && Convert.ToInt32(txtCantMesas.Text) <= 25)
            {
                for (int i = panelComedor.Controls.Count - 1; i >= 0; i--)
                {
                    if (panelComedor.Controls[i] is Label && panelComedor.Controls[i].Tag != null && panelComedor.Controls[i].Tag.ToString().Contains("Mesa"))
                    {
                        panelComedor.Controls.Remove(panelComedor.Controls[i]);
                    }
                }
                flag = true;

                int x = 150, y = 110;
                int numMesas = Convert.ToInt32(txtCantMesas.Text.Trim());
                mesas = new List<Label>();
                panelesMesas = new List<Panel>(numMesas);
                int cont = 0;
                for (int i = 0; i < numMesas; i++)
                {
                    cont++;
                    mesa = new Label();
                    mesa.Image = Properties.Resources.mesa2p;
                    mesa.Width = 162;
                    mesa.Height = 162;
                    mesa.Cursor = Cursors.Hand;
                    mesa.Text = "";
                    mesa.Tag = "Mesa " + cont;
                    mesa.Location = new Point(x, y);
                    mesa.Click += new EventHandler(mesa_Click);
                    panelComedor.Controls.Add(mesa);
                    mesas.Add(mesa);
                    if (cont % 5 == 0)
                    {
                        y += 175;
                        x = 150;
                    }
                    else
                    {
                        x += 350;
                    }
                }
                txtCantMesas.Text = "";
            }
            else
            {
                MessageBox.Show("Introduce un número del 1-25", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void mesa_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < panelesMesas.Count; i++)
            {
                if (((Label)sender).Tag.ToString().Trim() == panelesMesas[i].Tag.ToString().Trim())
                {
                    panel = panelesMesas[i];
                    panel.Controls.Add(panelMesa);
                    panelMesa.Dock = DockStyle.Fill;
                    cambioDePanel(panel, panelComedor);
                    Console.WriteLine("Panel abierto: " + panel.Tag);
                    return;
                }
                else
                {
                    flag = true;
                }
            }

            if (flag)
            {
                int num = Convert.ToInt32(((Label)sender).Tag.ToString().Substring(5).Trim());
                panel = new Panel();
                panel.Tag = ((Label)sender).Tag;
                panel.Controls.Add(panelMesa);
                panelMesa.Dock = DockStyle.Fill;
                panelMesa.Dock = DockStyle.Fill;
                this.Controls.Add(panel);
                panel.Dock = DockStyle.Fill;
                panel.BringToFront();
                panelesMesas.Add(panel);
                Console.WriteLine("Panel creado: " + panel.Tag);
                flag = false;
            }
        }

        private void lblAtrasMesa_Click(object sender, EventArgs e)
        {
            cambioDePanel(panelComedor, panel);
        }

        private void lblCerrarSesion_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Seguro que deseas cerrar sesión?", "Salir", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.Cancel)
            {
                cerrarSesion();
            }
        }

        public void cerrarSesion()
        {
            txtUsuario.Text = "";
            txtContraseña.Text = "";
            txtCantMesas.Text = "";
            for (int i = mesas.Count - 1; i >= 0; i--)
            {
                mesas.RemoveAt(i);
            }
            for (int i = panelesMesas.Count - 1; i >= 0; i--)
            {
                panelesMesas.RemoveAt(i);
            }
            for (int i = panelComedor.Controls.Count - 1; i >= 0; i--)
            {
                if(panelComedor.Controls[i] is Label && ((Label)panelComedor.Controls[i]).Tag.ToString().Contains("Mesa"))
                {
                    panelComedor.Controls.Remove(panelComedor.Controls[i]);
                }
            }
            cambioDePanel(panelIniciarSesion, panelConfiguracion);
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
                    txtNombre.Text = "";
                    txtCantidad.Text = "";
                    txtPrecio.Text = "";
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Error al introducir los campos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblBebidasTapas_Click(object sender, EventArgs e)
        {
            for (int i = flowLayoutPanelDatos.Controls.Count-1; i >= 0; i--)
            {
                flowLayoutPanelDatos.Controls.RemoveAt(i);
            }
            Label lblOpcion;
            List<string> datos = connectionSQL.datosAlmacenNombres(((Label)sender).Tag.ToString());
            for (int i = 0; i < datos.Count; i++)
            {
                lblOpcion = new Label();
                lblOpcion.Text = datos[i];
                lblOpcion.BorderStyle = BorderStyle.FixedSingle;
                lblOpcion.Cursor = Cursors.Hand;
                lblOpcion.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                lblOpcion.Location = new Point(0, 0);
                lblOpcion.Tag = datos[i];
                lblOpcion.Size = new Size(241, 131);
                lblOpcion.TextAlign = ContentAlignment.MiddleCenter;
                lblOpcion.MouseEnter += new EventHandler(this.lbl_MouseEnter);
                lblOpcion.MouseLeave += new EventHandler(this.lbl_MouseLeave);
                //TODO evento click
                flowLayoutPanelDatos.Controls.Add(lblOpcion);
            }
        }
    }
}
