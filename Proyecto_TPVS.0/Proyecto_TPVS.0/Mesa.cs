using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_TPVS._0
{
    class Mesa
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
        List<string> productos;
        public List<string> Productos
        {
            set
            {
                productos = value;
            }
            get
            {
                return productos;
            }
        }
    }
}
