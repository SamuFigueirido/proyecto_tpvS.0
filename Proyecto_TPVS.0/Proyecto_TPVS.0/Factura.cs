using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_TPVS._0
{
    class Factura
    {
        private string nombre;
        public string Nombre
        {
            set
            {
                nombre = value;
            }
            get
            {
                return nombre;
            }
        }
        private string platos;
        public string Platos
        {
            set
            {
                platos = value;
            }
            get
            {
                return platos;
            }
        }
        private double total;
        public double Total
        {
            set
            {
                total = value;
            }
            get
            {
                return total;
            }
        }

        public override string ToString()
        {
            string[] lines = Platos.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            return "\n\r"+Nombre+"\nPlatos:\n" + lines.ToString() + "\n\r\nTOTAL: " + total;
        }
    }
}
