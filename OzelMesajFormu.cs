using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace _2.sınıf_2._dönem_projesi
{
    public partial class OzelMesajFormu : Form
    {
        // Form referansı: Form1 veya Form2 olabilir
        Form anaForm;

      
        public OzelMesajFormu(string mesaj, string tip, Form form)
        {
            InitializeComponent();
            lblMesaj.Text = mesaj;
        }


        private void OzelMesajFormu_Load(object sender, EventArgs e)
        {

        }

        private void OzelMesajFormu_Paint(object sender, PaintEventArgs e)
        {
            int kenarKalınlığı = 5; // px
            Color kenarRengi = Color.DarkRed; // istediğin renk

            using (Pen pen = new Pen(kenarRengi, kenarKalınlığı))
            {
                // Formun boyutunu kenar kalınlığı kadar küçülterek dikdörtgen çiz
                Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                e.Graphics.DrawRectangle(pen, rect);
            }
        }

        private void btnTamam_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
