using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace ORV_StreznikApp
{
    public partial class Form2 : Form
    {
        OpenFileDialog Openfile;
        Image<Bgr, byte> My_Image;
        private Image<Bgr, byte> My_ImageSend;

        public Form2()
        {
            InitializeComponent();
        }

        public Image<Bgr, byte> MyImageSG
        {
            get { return My_ImageSend; }
            set { My_ImageSend = value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Openfile = new OpenFileDialog();
            if (Openfile.ShowDialog() == DialogResult.OK)
            {
                My_Image = new Image<Bgr, byte>(Openfile.FileName);
                pictureBox1.Image = My_Image.ToBitmap();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MyImageSG = new Image<Bgr, byte>(My_Image.ToBitmap());
        }
    }
}
