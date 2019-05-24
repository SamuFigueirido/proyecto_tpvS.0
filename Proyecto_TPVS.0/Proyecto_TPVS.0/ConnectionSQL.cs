using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_TPVS._0
{
    class ConnectionSQL
    {
        Pass encryptDecrypt = new Pass();
        SQLiteConnection conn = new SQLiteConnection("Data Source=database.db");
        List<string> empleadosList;
        List<Factura> facturasList;
        List<string> tablasList;
        List<string> tablasAlmacenList;
        List<string> datosAlmacenList;
        List<string> datosNombres;
        List<string> reservas;

        public void abrirConexion()
        {
            cerrarConexion();
            try
            {
                conn.Open();
                //Console.WriteLine("Conexión abierta");
            }
            catch (SqlException)
            {
                Console.WriteLine("Fallo al abrir la conexión");
            }
        }

        public void cerrarConexion()
        {
            if (conn != null)
            {
                conn.Close();
                //Console.WriteLine("Conexión cerrada");
            }
        }

        public void createBD()
        {
            if (!System.IO.File.Exists("database.db"))
            {
                SQLiteConnection.CreateFile("database.db");
            }

            string empleados = @"CREATE TABLE IF NOT EXISTS
                            [empleados](
                            [id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [nombre] VARCHAR(50) NOT NULL,
                            [contraseña] VARCHAR(50)
                            )";
            string bebidas = @"CREATE TABLE IF NOT EXISTS
                            [bebidas](
                            [id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [nombre] VARCHAR(50) NULL,
                            [cantidad] VARCHAR(50) NULL,
                            [precio] DECIMAL NOT NULL
                            )";
            string tapas = @"CREATE TABLE IF NOT EXISTS
                            [tapas](
                            [id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [nombre] VARCHAR(50) NULL,
                            [cantidad] VARCHAR(50) NULL,
                            [precio] DECIMAL NOT NULL
                            )";
            string facturas = @"CREATE TABLE IF NOT EXISTS
                            [facturas](
                            [id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [nombre] VARCHAR(50) NOT NULL,
                            [platos] VARCHAR(123456789) NULL,
                            [total] float NOT NULL
                            )";
            string reservas = @"CREATE TABLE IF NOT EXISTS
                            [reservas](
                            [id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [reserva] VARCHAR(50) NOT NULL
                            )";
            abrirConexion();
            executeQuery(empleados);
            executeQuery(bebidas);
            executeQuery(tapas);
            executeQuery(facturas);
            executeQuery(reservas);
            //executeQuery("DROP TABLE [facturas]");
            //executeQuery("DROP TABLE [reservas]");
            //executeQuery("DROP TABLE [tapas]");
            //executeQuery("DROP TABLE [bebidas]");
            //executeQuery("INSERT INTO [empleados] (nombre, contraseña) values('admin', '" + encryptDecrypt.encrypt("0000") + "')");
            //executeQuery("INSERT INTO [bebidas] (nombre, cantidad, precio) values('Fanta', 20, 2.5)");
            //executeQuery("INSERT INTO [tapas] (nombre, cantidad, precio) values('Calamares', 10, 8.5)");
            //executeQuery("INSERT INTO [bebidas] (nombre, cantidad, precio) values('CocaCola', 20, 2.5)");
            //executeQuery("INSERT INTO [tapas] (nombre, cantidad, precio) values('Croquetas', 10, 6)");
            //executeQuery("INSERT INTO [bebidas] (nombre, cantidad, precio) values('Nestea', 20, 2)");
            //executeQuery("INSERT INTO [tapas] (nombre, cantidad, precio) values('Tortilla', 10, 7)");
            //executeQuery("INSERT INTO [bebidas] (nombre, cantidad, precio) values('Aquarius', 20, 2)");
            //executeQuery("INSERT INTO [tapas] (nombre, cantidad, precio) values('Callos', 10, 6.5)");
            //executeQuery("DELETE FROM [empleados]");
            //executeQuery("DELETE FROM [bebidas]");
            //executeQuery("DELETE FROM [tapas]");
            cerrarConexion();
        }

        public void executeQuery(string query)
        {
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }
            }catch (SQLiteException e)
            {
                Console.WriteLine("Error: "+ e.ErrorCode);
            }
        }

        public bool addEmpleado(string newName, string password)
        {
            abrirConexion();
            string consultName = "";
            string querySelect = "select * from [empleados] where nombre = '" + newName + "'";
            string queryInsert = "";
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = querySelect;
                SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    consultName = Convert.ToString(reader["nombre"]).Trim();
                }
                reader.Close();
                if (consultName != newName)
                {
                    try
                    {
                        queryInsert = "insert into [empleados] (nombre, contraseña) values ('" + newName + "', '" + encryptDecrypt.encrypt(password) + "')";
                        cmd.CommandText = queryInsert;
                        int res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            MessageBox.Show("Nuevo usuario creado con éxito", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return true;
                        }
                    }
                    catch (SqlException)
                    {
                        Console.WriteLine("Error");
                    }
                }
                else
                {
                    MessageBox.Show("El nombre de usuario ya está en uso", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                cerrarConexion();
                return false;
            }
        }

        public bool loginEmpleado(string userName, string userPassword)
        {
            abrirConexion();
            string consultName = "";
            string consultPassword = "";
            string query = "select * from [empleados] where nombre = '" + userName + "'";
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = query;
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        consultName = Convert.ToString(reader["nombre"]).Trim();
                        consultPassword = encryptDecrypt.decrypt(Convert.ToString(reader["contraseña"]).Trim());
                    }
                }
                if (userName == consultName && userPassword == consultPassword)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            cerrarConexion();
            return false;
        }

        public List<string> empleados()
        {
            abrirConexion();
            empleadosList = new List<string>();
            string query = "select * from [empleados]";
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = query;
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    empleadosList.Add(Convert.ToString(reader["nombre"]).Trim());
                }
            }
            cerrarConexion();
            return empleadosList;
        }

        public List<string> tablas()
        {
            tablasList = new List<string>();
            abrirConexion();
            string query = "SELECT * FROM sqlite_master WHERE type='table'";
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = query;
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tablasList.Add(Convert.ToString(reader["name"]).Trim());
                }
            }
            cerrarConexion();
            return tablasList;
        }

        public List<string> almacen()
        {
            tablasAlmacenList = new List<string>();
            abrirConexion();
            string query = "SELECT * FROM sqlite_master WHERE type='table' and name='tapas'";
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = query;
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tablasAlmacenList.Add(Convert.ToString(reader["name"]).Trim());
                }
                reader.Close();
                query = "SELECT * FROM sqlite_master WHERE type='table' and name='bebidas'";
                cmd.CommandText = query;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tablasAlmacenList.Add(Convert.ToString(reader["name"]).Trim());
                }
            }
            cerrarConexion();
            return tablasAlmacenList;
        }

        public List<string> datosAlmacen(string tabla)
        {
            abrirConexion();
            datosAlmacenList = new List<string>();
            string query = "select * from [" + tabla + "]";
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = query;
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    datosAlmacenList.Add(string.Format("{0, -20}{1, -20}{2, -20}", Convert.ToString(reader["nombre"]).Trim(), Convert.ToString(reader["precio"]).Trim() + "€", Convert.ToString(reader["cantidad"]).Trim() + "Uds."));
                }
            }
            cerrarConexion();
            return datosAlmacenList;
        }

        public List<string> datosAlmacenNombres(string tabla)
        {
            abrirConexion();
            datosNombres = new List<string>();
            string query = "select * from [" + tabla + "]";
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = query;
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    datosNombres.Add(Convert.ToString(reader["nombre"]).Trim());
                }
            }
            cerrarConexion();
            return datosNombres;
        }

        public void insertDatos(string tabla, string nombre, int cantidad, double precio)
        {
            abrirConexion();
            executeQuery("INSERT INTO [" + tabla + "] (nombre, cantidad, precio) values('" + nombre + "', " + cantidad + ", '" + precio + "')");
            cerrarConexion();
        }

        public void deleteDatos(string tabla, string nombre)
        {
            abrirConexion();
            executeQuery("DELETE FROM [" + tabla + "] where nombre='" + nombre + "'");
            cerrarConexion();
        }

        public string getNombreProducto(string tabla, string nombre)
        {
            string nombreProducto = "";
            abrirConexion();
            string query = "select * from [" + tabla + "] where nombre='"+nombre+"'";
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = query;
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    nombreProducto = Convert.ToString(reader["nombre"]).Trim();
                }
            }
            cerrarConexion();
            return nombreProducto;
        }

        public double getPrecioProducto(string tabla, string nombre)
        {
            double precio = 0;
            abrirConexion();
            string query = "select * from [" + tabla + "] where nombre='" + nombre + "'";
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = query;
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    precio = Convert.ToDouble(reader["precio"]);
                }
            }
            cerrarConexion();
            return precio;
        }

        public void saveReserva(string reserva)
        {
            abrirConexion();
            executeQuery("INSERT INTO [reservas] (reserva) values ('" + reserva + "')");
            cerrarConexion();
        }

        public List<string> getReservas()
        {
            reservas = new List<string>();
            abrirConexion();
            string query = "select * from [reservas]";
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = query;
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    reservas.Add(Convert.ToString(reader["reserva"]).Trim());
                }
            }
            cerrarConexion();
            return reservas;
        }

        public void vaciarTabla(string tabla)
        {
            abrirConexion();
            executeQuery("DELETE FROM " + tabla);
            cerrarConexion();
        }

        public void saveFactura(string nombre, string platos, double total)
        {
            abrirConexion();
            executeQuery("INSERT INTO [facturas] (nombre, platos, total) values ('" + nombre + "', '"+platos+"',"+total+" )");
            cerrarConexion();
        }

        public List<Factura> getFacturas()
        {
            abrirConexion();
            facturasList = new List<Factura>();
            string query = "select * from [facturas]";
            Factura factura;
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = query;
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    factura = new Factura();
                    factura.Nombre = Convert.ToString(reader["nombre"]).Trim();
                    factura.Platos = Convert.ToString(reader["platos"]).Trim();
                    factura.Total = Convert.ToDouble(reader["total"]);
                    facturasList.Add(factura);
                }
            }
            cerrarConexion();
            return facturasList;
        }
    }
}
