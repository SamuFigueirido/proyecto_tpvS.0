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

namespace Proyecto_TPVS._0
{
    public partial class FormIniciarSesion : Form
    {
        KunLibertad_DesktopControl desktopControl;
        public FormIniciarSesion()
        {
            InitializeComponent();
        }

        private void FormIniciarSesion_Load(object sender, EventArgs e)
        {
            pantallaCompleta(this);
        }

        public void pantallaCompleta(Form f)
        {
            desktopControl = new KunLibertad_DesktopControl();
            desktopControl.TaskBar(true);
            f.Size = Screen.PrimaryScreen.WorkingArea.Size;
            f.Location = Screen.PrimaryScreen.WorkingArea.Location;
            f.WindowState = FormWindowState.Maximized;
            f.TopMost = true;
            f.FormBorderStyle = FormBorderStyle.None;
        }

        private void FormIniciarSesion_FormClosing(object sender, FormClosingEventArgs e)
        {
            desktopControl.TaskBar(false);
        }
    }
}
