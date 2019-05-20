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
        Hashtable bebidasHash;
        Hashtable tapasHash;
        List<string> facturasList;

        public void abrirConexion()
        {
            try
            {
                conn.Open();
                Console.WriteLine("Conexión abierta");
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
                Console.WriteLine("Conexión cerrada");
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
                            [precio] float NOT NULL
                            )";
            string tapas = @"CREATE TABLE IF NOT EXISTS
                            [tapas](
                            [id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [nombre] VARCHAR(50) NULL,
                            [cantidad] VARCHAR(50) NULL,
                            [precio] float NOT NULL
                            )";
            string facturas = @"CREATE TABLE IF NOT EXISTS
                            [facturas](
                            [id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [platos] VARCHAR(50) NULL,
                            [total] float NOT NULL
                            )";
            string reservas = @"CREATE TABLE IF NOT EXISTS
                            [reservas](
                            [id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [nombre] VARCHAR(50) NULL,
                            [fecha] datetimeoffset NULL
                            )";
            //string insertAdmin = "INSERT INTO [empleados] (nombre, contraseña) values('admin', '" + encryptDecrypt.encrypt("0000") + "')";
            abrirConexion();
            executeQuery(empleados);
            executeQuery(bebidas);
            executeQuery(tapas);
            executeQuery(facturas);
            executeQuery(reservas);
            //executeQuery(insertAdmin);
            cerrarConexion();
        }

        public void executeQuery(string query)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
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
                while(reader.Read())
                {
                    empleadosList.Add(Convert.ToString(reader["nombre"]).Trim());
                }
            }
            cerrarConexion();
            return empleadosList;
        }

        public Hashtable bebidas()
        {
            abrirConexion();
            bebidasHash = new Hashtable();
            string query = "select * from [bebidas]";
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = query;
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    bebidasHash.Add(Convert.ToString(reader["nombre"]).Trim(), Convert.ToDouble(reader["precio"]));
                }
            }
            cerrarConexion();
            return bebidasHash;
        }

        public Hashtable tapas()
        {
            abrirConexion();
            tapasHash = new Hashtable();
            string query = "select * from [tapas]";
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = query;
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tapasHash.Add(Convert.ToString(reader["nombre"]).Trim(), Convert.ToDouble(reader["precio"]));
                }
            }
            cerrarConexion();
            return tapasHash;
        }

        public List<string> facturas()
        {
            abrirConexion();
            facturasList = new List<string>();
            string query = "select * from [facturas]";
            Factura factura;
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = query;
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    factura = new Factura();
                    factura.Id = Convert.ToInt32(reader["id"]);
                    factura.Platos = Convert.ToString(reader["platos"]).Trim();
                    factura.Total = Convert.ToDouble(reader["total"]);
                    facturasList.Add(factura.ToString());
                }
            }
            cerrarConexion();
            return facturasList;
        }
    }
}
