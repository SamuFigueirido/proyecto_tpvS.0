using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_TPVS._0
{
    class Pass
    {
        public string encrypt(string password)
        {
            string result = string.Empty;
            byte[] encryted = Encoding.Unicode.GetBytes(password);
            return Convert.ToBase64String(encryted);
        }

        public string decrypt(string password)
        {
            string result = string.Empty;
            byte[] decryted = Convert.FromBase64String(password);
            return Encoding.Unicode.GetString(decryted);
        }
    }
}
