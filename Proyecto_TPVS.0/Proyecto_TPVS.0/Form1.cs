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
using System.Reflection;
using System.IO;

namespace Proyecto_TPVS._0
{
    public partial class FormIniciarSesion : Form
    {
        string format = "{0,-15}{1,-20}{2,-10:#.00}";
        ConnectionSQL connectionSQL;
        Pass encryptDecrypt = new Pass();
        List<string> datosNombres;
        List<Label> mesas;
        Label mesa;
        Panel panel;
        bool flag = true;
        List<Panel> panelesMesas;
        string tagMesaAux = "";
        string tabla = "";

        double total = 0;
        Factura factura;

        Hashtable comensales = new Hashtable();

        //PARA GESTIONAR LA NOTA DE CADA MESA
        Hashtable mesasHash = new Hashtable(); //CONJUNTO DE MESAS (KEY: NOMBRE DE LA MESA, VALUE: OBJETO MESA)

        int cantProd = 0;

        public FormIniciarSesion()
        {
            InitializeComponent();
        }

        private void FormIniciarSesion_Load(object sender, EventArgs e)
        {
            try
            {
                ajustarPaneles();
                connectionSQL = new ConnectionSQL();
                connectionSQL.createBD();
                flowLayoutPanelDatos.Height = this.Height * 4 / 10;
                pantallaCompleta(this);
                getFacuraFromDB(listBoxFacturas);
                getReservaFromDB(listBoxReservas);
            }
            catch (FileNotFoundException)
            {
                SQLiteConnection.CreateFile("database.db");
                connectionSQL.createBD();
            }
        }

        public void pantallaCompleta(Form f)
        {
            f.Size = Screen.PrimaryScreen.WorkingArea.Size;
            f.Location = Screen.PrimaryScreen.WorkingArea.Location;
            f.WindowState = FormWindowState.Maximized;
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
                saveReserva(listBoxReservas);
                cerrarSesion();
                connectionSQL.cerrarConexion();
            }
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

        private void numConfirm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                e.Handled = true;
            }
        }

        private void btnAceptarCantMesas_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCantMesas.Text.Trim() != "" && Convert.ToInt32(txtCantMesas.Text) > 0 && Convert.ToInt32(txtCantMesas.Text) <= 25)
                {
                    for (int i = panelComedor.Controls.Count - 1; i >= 0; i--)
                    {
                        if (panelComedor.Controls[i] is Label && panelComedor.Controls[i].Tag != null && panelComedor.Controls[i].Tag.ToString().Contains("Mesa"))
                        {
                            panelComedor.Controls.Remove(panelComedor.Controls[i]);
                        }
                    }
                    flag = true;
                    int x = this.Width * 1 / 10;
                    int y = this.Height * 3 / 2 / 10;
                    int numMesas = Convert.ToInt32(txtCantMesas.Text.Trim());
                    mesas = new List<Label>();
                    panelesMesas = new List<Panel>(numMesas);
                    int cont = 0;
                    for (int i = 0; i < numMesas; i++)
                    {
                        cont++;
                        mesa = new Label();
                        mesa.Image = Properties.Resources.mesa2p;
                        mesa.Width = 120;
                        mesa.Height = 120;
                        mesa.Cursor = Cursors.Hand;
                        mesa.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
                        mesa.Text = cont.ToString();
                        mesa.Tag = "Mesa " + cont;
                        mesa.TextAlign = ContentAlignment.MiddleCenter;
                        mesa.Location = new Point(x, y);
                        mesa.Click += new EventHandler(mesa_Click);
                        panelComedor.Controls.Add(mesa);
                        mesas.Add(mesa);

                        //CREACION DE LOS OBJETOS MESA  
                        crearObjetosMesa(mesa.Tag.ToString().Trim(), total);

                        if (cont % 5 == 0)
                        {
                            y += this.Height * 3 / 2 / 10;
                            x = this.Width * 1 / 10;
                        }
                        else
                        {
                            x += this.Width * 7 / 2 / 2 / 10;
                        }
                    }
                    txtCantMesas.Text = "";
                }
                else
                {
                    MessageBox.Show("Introduce un número del 1-25", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (FormatException)
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
                    txtCantPersonas.Text = "Comensales: " + comensales[((Label)sender).Tag.ToString().Trim()];
                    flag = false;
                    break;
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
                txtCantPersonas.Text = "Comensales: ";
            }
            Console.WriteLine("Click: " + ((Label)sender).Tag.ToString());
            tagMesaAux = ((Label)sender).Tag.ToString().Trim();
            lblMesa.Text = tagMesaAux;

            //ABRIR EL OBJETO MESA Y RECORRERLO
            añadirMesaAlListBox(listBoxNota, tagMesaAux);
            Console.WriteLine("MESA_CLICK");
            recorrerHashtable();
        }

        private void lblAtrasMesa_Click(object sender, EventArgs e)
        {
            //AÑADIR DATOS AL OBJETO MESA
            añadirDatosAlObjetoMesa(listBoxNota, tagMesaAux);
            Console.WriteLine("LBLATRASMESA_CLICK");
            recorrerHashtable();

            cambioDePanel(panelComedor, panel);
            for (int i = listBoxNota.Items.Count - 1; i >= 0; i--)
            {
                listBoxNota.Items.RemoveAt(i);
            }
            txtTotal.Text = "";
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
            try
            {
                for (int i = mesas.Count - 1; i >= 0; i--)
                {
                    mesas.RemoveAt(i);
                }
            }
            catch (NullReferenceException)
            {

            }
            try
            {
                for (int i = panelesMesas.Count - 1; i >= 0; i--)
                {
                    panelesMesas.RemoveAt(i);
                }
            }
            catch (NullReferenceException)
            {

            }
            try
            {
                for (int i = panelComedor.Controls.Count - 1; i >= 0; i--)
                {
                    if (panelComedor.Controls[i] is Label && ((Label)panelComedor.Controls[i]).Tag.ToString().Contains("Mesa"))
                    {
                        panelComedor.Controls.Remove(panelComedor.Controls[i]);
                    }
                }
            }
            catch (NullReferenceException)
            {

            }
            comensales.Clear();
            saveFactura(listBoxFacturas);
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
                    MessageBox.Show("Completa todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (Convert.ToInt32(txtCantidad.Text.Trim()) == 0)
                    {
                        MessageBox.Show("La cantidad no puede ser cero.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        if (Convert.ToInt32(txtCantidad.Text.Trim()) < 0 || Convert.ToDecimal(txtPrecio.Text.Trim()) < 0)
                        {
                            MessageBox.Show("Los valores no pueden ser negativos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            if (txtNombre.Text.Trim().Length > 20 || txtCantidad.Text.Trim().Length > 5 || txtPrecio.Text.Trim().Length > 6)
                            {
                                MessageBox.Show("Alguno de los valores introducidos\nes demasiado grande.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                try
                                {
                                    connectionSQL.insertDatos(listBoxTablas.SelectedValue.ToString(), txtNombre.Text.Trim(), Convert.ToInt32(txtCantidad.Text.Trim()), Convert.ToDouble(txtPrecio.Text));
                                    listBoxDatos.DataSource = connectionSQL.datosAlmacen(listBoxTablas.SelectedValue.ToString());
                                    txtNombre.Text = "";
                                    txtCantidad.Text = "";
                                    txtPrecio.Text = "";
                                }
                                catch (OverflowException)
                                {
                                    MessageBox.Show("Error al introducir los campos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Error al introducir los campos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblBebidasTapas_Click(object sender, EventArgs e)
        {
            for (int i = flowLayoutPanelDatos.Controls.Count - 1; i >= 0; i--)
            {
                flowLayoutPanelDatos.Controls.RemoveAt(i);
            }
            Label lblOpcion;
            List<string> datos = connectionSQL.datosAlmacenNombres(((Label)sender).Tag.ToString());
            tabla = ((Label)sender).Tag.ToString();
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
                lblOpcion.Click += new EventHandler(this.lblDatos_Click);
                flowLayoutPanelDatos.Controls.Add(lblOpcion);
            }
        }

        private void lblDatos_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCalc.Text.Trim() != "")
                {
                    if (txtCalc.Text.Trim() == "0")
                    {
                        txtCalc.Text = "1";
                    }
                    cantProd = Convert.ToInt32(txtCalc.Text.Trim());

                    string nombreProducto = connectionSQL.getNombreProducto(tabla, ((Label)sender).Tag.ToString());
                    double precioProducto = connectionSQL.getPrecioProducto(tabla, ((Label)sender).Tag.ToString());
                    precioProducto *= cantProd;
                    total += precioProducto;
                    Console.WriteLine("-----TAG MESA: " + tagMesaAux);
                    Console.WriteLine("-----DATO: " + nombreProducto + "-----PRECIO: " + precioProducto);

                    listBoxNota.Items.Add(String.Format(format, cantProd, nombreProducto, precioProducto + "€"));
                    txtTotal.Text = total.ToString();
                    txtCalc.Text = "0";
                }
            }catch (OverflowException)
            {
                MessageBox.Show("Valor demasiado grande.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAceptarComensales_Click(object sender, EventArgs e)
        {
            if (txtCantComensales.Text.Trim() != "" && Convert.ToInt32(txtCantComensales.Text.Trim()) > 0)
            {
                int cant = Convert.ToInt32(txtCantComensales.Text.Trim());
                for (int i = 0; i < panelComedor.Controls.Count; i++)
                {
                    if (panelComedor.Controls[i] is Label && ((Label)panelComedor.Controls[i]).Tag.ToString().Trim() == tagMesaAux)
                    {
                        Image image = Properties.Resources.mesa2p;
                        if (cant <= 2)
                        {
                            image = Properties.Resources.mesa2p;
                        }
                        else if (cant <= 4)
                        {
                            image = Properties.Resources.mesa4p;
                        }
                        else if (cant <= 6)
                        {
                            image = Properties.Resources.mesa6p;
                        }
                        else
                        {
                            image = Properties.Resources.mesa8p;
                        }
                        ((Label)panelComedor.Controls[i]).Image = image;

                        try
                        {
                            comensales.Add(((Label)panelComedor.Controls[i]).Tag.ToString().Trim(), cant);
                        }
                        catch (ArgumentException)
                        {
                            comensales[((Label)panelComedor.Controls[i]).Tag.ToString().Trim()] = cant;
                        }
                        Console.WriteLine("hashtable.value = " + comensales[((Label)panelComedor.Controls[i]).Tag.ToString().Trim()]);
                        Console.WriteLine("hashtable.key = " + ((Label)panelComedor.Controls[i]).Tag.ToString().Trim());
                    }
                }
                txtCantPersonas.Text = "Comensales: " + cant;
            }
            else
            {
                MessageBox.Show("Cantidad errónea.\nIntroduce de nuevo un valor.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            txtCantComensales.Text = "";
        }

        private void btnAñadirFecha_Click(object sender, EventArgs e)
        {
            string nombreReserva = txtNombreReserva.Text.Trim();
            if (nombreReserva == "")
            {
                MessageBox.Show("Nombre introducido no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                int hora = Convert.ToInt32(txtHoraReserva.Text.Trim());
                if (hora >= 24)
                {
                    hora = 0;
                }
                int minutos = Convert.ToInt32(txtMinutosReserva.Text.Trim());
                if (minutos >= 60)
                {
                    minutos = 0;
                }
                string horaReserva = String.Format("{0,0:D2}:{1,0:D2}", hora, minutos);
                string fechaReserva = dateTPReserva.Value.Day + "/" + dateTPReserva.Value.Month + "/" + dateTPReserva.Value.Year;
                Console.WriteLine("Nombre: " + nombreReserva + "\nFecha: " + fechaReserva + "\nHora: " + horaReserva);

                listBoxReservas.Items.Add(String.Format("{0, -20}{1, 9}{2, 15}", nombreReserva, fechaReserva, horaReserva));
                txtHoraReserva.Text = "";
                txtMinutosReserva.Text = "";
                txtNombreReserva.Text = "";
            }
        }

        private void btnEliminarFecha_Click(object sender, EventArgs e)
        {
            for (int i = listBoxReservas.SelectedItems.Count - 1; i >= 0; i--)
            {
                listBoxReservas.Items.Remove(listBoxReservas.SelectedItems[i]);
            }
        }

        private void saveReserva(ListBox list)
        {
            connectionSQL.vaciarTabla("reservas");
            for (int i = 0; i < list.Items.Count; i++)
            {
                connectionSQL.saveReserva(list.Items[i].ToString());
            }
        }

        private void getReservaFromDB(ListBox list)
        {
            for (int i = 0; i < connectionSQL.getReservas().Count; i++)
            {
                list.Items.Add(connectionSQL.getReservas()[i]);
            }
        }

        private void btnBorrarFactura_Click(object sender, EventArgs e)
        {
            for (int i = listBoxFacturas.SelectedItems.Count - 1; i >= 0; i--)
            {
                listBoxFacturas.Items.Remove(listBoxFacturas.SelectedItems[i]);
            }
        }

        private void lblFactura_Click(object sender, EventArgs e)
        {
            string platos = "";
            if (listBoxNota.Items.Count > 0)
            {
                for (int i = 0; i < listBoxNota.Items.Count; i++)
                {
                    platos += listBoxNota.Items[i] + "\r\n";
                }
                Console.WriteLine(platos);
                Console.WriteLine(tagMesaAux);
                factura = new Factura();
                factura.Nombre = tagMesaAux;
                factura.Platos = platos;
                factura.Total = total;
                listBoxFacturas.Items.Add(factura);
                MessageBox.Show(factura.ToString2() + "\n(Factura añadida a facturas)", "Factura", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            for (int i = listBoxNota.Items.Count - 1; i >= 0; i--)
            {
                listBoxNota.Items.RemoveAt(i);
            }
            txtTotal.Text = "";

            //REINICIAR OBJETO MESA UNA VEZ COBRADO
            reiniciarObjetoMesa(tagMesaAux);
        }

        private void saveFactura(ListBox list)
        {
            connectionSQL.vaciarTabla("facturas");
            for (int i = 0; i < list.Items.Count; i++)
            {
                Factura f = ((Factura)(list.Items[i]));
                connectionSQL.saveFactura(f.Nombre, f.Platos, f.Total);
            }
        }

        private void getFacuraFromDB(ListBox list)
        {
            for (int i = 0; i < connectionSQL.getFacturas().Count; i++)
            {
                list.Items.Add(connectionSQL.getFacturas()[i]);
            }
        }

        private void lblBorrar_Click(object sender, EventArgs e)
        {
            for (int i = listBoxNota.SelectedItems.Count - 1; i >= 0; i--)
            {
                string precio = listBoxNota.SelectedItems[i].ToString().Substring(listBoxNota.SelectedItems[i].ToString().Length - 15).Trim();
                precio = precio.Replace("€", "").Trim();
                double precioARestar = Convert.ToDouble(precio);
                listBoxNota.Items.Remove(listBoxNota.SelectedItems[i]);
                Console.WriteLine("PRECIOOOOOOOOOOOOOO:" + precioARestar);
                total -= precioARestar;
            }
            txtTotal.Text = total.ToString();
        }

        private void listBoxFacturas_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                txtFactura.Text = ((Factura)listBoxFacturas.SelectedItem).ToString2();
            }
            catch (NullReferenceException)
            {
                txtFactura.Text = "";
            }
        }

        private void crearObjetosMesa(string nombre, double precioTotal)
        {
            Mesa mesa = new Mesa();
            mesa.Nombre = nombre;
            mesa.Productos = new List<string>();
            mesa.PrecioTotal = precioTotal;
            try
            {
                mesasHash.Add(nombre, mesa);
            }
            catch (ArgumentException)
            {
                mesasHash[nombre] = mesa;
            }
        }

        public void añadirMesaAlListBox(ListBox list, string nombre)
        {
            for (int i = list.Items.Count - 1; i >= 0; i--)
            {
                list.Items.RemoveAt(i);
            }
            Mesa mesa = (Mesa)mesasHash[nombre];
            for (int i = 0; i < mesa.Productos.Count; i++)
            {
                list.Items.Add(mesa.Productos[i].ToString());
            }
            total = mesa.PrecioTotal;
        }

        public void añadirDatosAlObjetoMesa(ListBox list, string nombre)
        {
            Mesa mesa = (Mesa)mesasHash[nombre];
            for (int i = mesa.Productos.Count - 1; i >= 0; i--)
            {
                mesa.Productos.RemoveAt(i);
            }
            for (int i = 0; i < list.Items.Count; i++)
            {
                mesa.Productos.Add(list.Items[i].ToString());
            }
            mesa.PrecioTotal = total;
        }

        public void reiniciarObjetoMesa(string nombre)
        {
            Mesa mesa = (Mesa)mesasHash[nombre];
            mesa.Productos.Clear();
            mesa.PrecioTotal = 0;
            mesa.Nombre = nombre;
        }

        private void recorrerHashtable()
        {
            foreach (DictionaryEntry item in mesasHash)
            {
                Console.WriteLine("----" + item.Key);
                //Console.WriteLine("----"+item.Value);
                Mesa mesa = (Mesa)item.Value;
                for (int i = 0; i < mesa.Productos.Count; i++)
                {
                    Console.WriteLine(mesa.Productos[i].ToString());
                }
            }
        }

        private void lblBorrarTxt_Click(object sender, EventArgs e)
        {
            txtCalc.Text = "0";
        }

        private void lblCalc_Click(object sender, EventArgs e)
        {
            if (txtCalc.Text.Length < 3)
            {
                if (txtCalc.Text == "0")
                {
                    txtCalc.Text = ((Label)sender).Tag.ToString();
                }
                else
                {
                    txtCalc.Text += ((Label)sender).Tag.ToString();
                }
            }
        }
    }
}
