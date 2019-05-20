using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_TPVS._0
{
    class Factura
    {
        private int id;
        public int Id
        {
            set
            {
                id = value;
            }
            get
            {
                return id;
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
            return "\nFactura nº "+Id+"\nPlatos:\n" + lines.ToString() + "\n\r\nTOTAL: " + total;
        }
    }
}
