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
using Emgu.CV.CvEnum;
using System.Configuration;
using System.Data.SqlClient;

namespace ORV_StreznikApp
{
    public partial class Form1 : Form
    {
        // database
        string connectionString;
        SqlConnection connection;
        SqlDataAdapter adapter;
        SqlCommand command;
        DataTable dTable;

        // image variables
        Image<Bgr, byte> My_ImageRecieve;

        int[] vrednosti_pixlov = new int[256];
        int[] vrednost_pixlov_uniformni = new int[59];
        Image<Gray, byte> imgSiva;
        Image<Gray, byte> lbn;



        public Form1()
        {
            InitializeComponent();
            // initialize connectionString to Database
            connectionString = ConfigurationManager.ConnectionStrings["ORV_StreznikApp.Properties.Settings.ORV_StreznikDatabaseConnectionString"].ConnectionString;
            connectMe();
        }
        /*
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        */
        private void button1_Click(object sender, EventArgs e)
        {
            // initialize Form2 to upload
            Form2 newF = new Form2();
            if(newF.ShowDialog() == DialogResult.OK)
            {
                // GET IMAGE
                My_ImageRecieve = new Image<Bgr, byte>(newF.MyImageSG.ToBitmap());
                pictureBox1.Image = My_ImageRecieve.ToBitmap();
                
            }
        }

        private void connectMe()
        {
            using (connection = new SqlConnection(connectionString))
            using (adapter = new SqlDataAdapter("SELECT * FROM ImageT", connection))
            {
                dTable = new DataTable();
                adapter.Fill(dTable);

                listBox1.DisplayMember = "imageDescriptives";
                listBox1.ValueMember = "Id";
                listBox1.DataSource = dTable;
            }
        }

        private void insertMe()
        {
            // initialize Database: query,
            string query = "INSERT INTO ImageT VALUES(@imgDesc)";
            // SAVE IMAGE DESCRIPTIVES TO DATABASE
            /*sectionStart*/
            using (connection = new SqlConnection(connectionString))
            using (command = new SqlCommand(query, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@imgDesc", IntArrayToString(vrednosti_pixlov));
                command.ExecuteNonQuery();
                connectMe();
            }
            /*sectionEnd*/
            /*TODO*/
        }

        public string IntArrayToString(int[] ints)
        {
            return string.Join(",", ints.Select(x => x.ToString()).ToArray());
        }// end of IntArrayToString

        private void LBPbasic()
        {
            int i;
            int j;
            int x;
            int y;
            int[,] maska = new int[3, 3];

            int[,] maska_primerjava = new int[3, 3];

            //int[] vrednosti_pixlov = new int[256];

            string bin_vrednost;
            int bin_v_int;

            imgSiva = My_ImageRecieve.Convert<Gray, Byte>();

            Image<Gray, byte> lbn = new Image<Gray, byte>(imgSiva.Width, imgSiva.Height);
            //gremo čez vse piksle slike
            for (i = 0; i < imgSiva.Height; i++)
            {
                for (j = 0; j < imgSiva.Width; j++)
                {

                    //dodajanje vrednosti v masko
                    for (x = 0; x < 3; x++)
                    {
                        for (y = 0; y < 3; y++)
                        {
                            //maska[0,0]
                            if (x == 0 && y == 0 && (i - 1 < 0 || j - 1 < 0))
                            {
                                maska[x, y] = 0;
                            }
                            else if (x == 0 && y == 0)
                            {
                                maska[x, y] = (int)imgSiva.Data[i - 1, j - 1, 0];
                            }
                            //maska[0,1]
                            if (x == 0 && y == 1 && i - 1 < 0)
                            {
                                maska[x, y] = 0;
                            }
                            else if (x == 0 && y == 1)
                            {

                                maska[x, y] = (int)imgSiva.Data[i - 1, j, 0];
                            }
                            //maska[0,2]
                            if (x == 0 && y == 2 && (i - 1 < 0 || j + 1 >= imgSiva.Width))
                            {
                                maska[x, y] = 0;
                            }
                            else if (x == 0 && y == 2)
                            {
                                maska[x, y] = (int)imgSiva.Data[i - 1, j + 1, 0];
                            }
                            //maska[1,0]
                            if (x == 1 && y == 0 && j - 1 < 0)
                            {
                                maska[x, y] = 0;
                            }
                            else if (x == 1 && y == 0)
                            {
                                maska[x, y] = (int)imgSiva.Data[i, j - 1, 0];
                            }
                            //maska[1,1]
                            if (x == 1 && y == 1)
                            {
                                maska[x, y] = (int)imgSiva.Data[i, j, 0];
                            }
                            //maska[1,2]
                            if (x == 1 && y == 2 && j + 1 >= imgSiva.Width)
                            {
                                maska[x, y] = 0;
                            }
                            else if (x == 1 && y == 2)
                            {
                                maska[x, y] = (int)imgSiva.Data[i, j + 1, 0];
                            }
                            //maska[2,0]
                            if (x == 2 && y == 0 && (i + 1 >= imgSiva.Height || j - 1 < 0))
                            {
                                maska[x, y] = 0;
                            }
                            else if (x == 2 && y == 0)
                            {
                                maska[x, y] = (int)imgSiva.Data[i + 1, j - 1, 0];
                            }
                            //maska[2,1]
                            if (x == 2 && y == 1 && i + 1 >= imgSiva.Height)
                            {
                                maska[x, y] = 0;
                            }
                            else if (x == 2 && y == 1)
                            {
                                maska[x, y] = (int)imgSiva.Data[i + 1, j, 0];
                            }

                            //maska[2,2]
                            if (x == 2 && y == 2 && (i + 1 >= imgSiva.Height || j + 1 >= imgSiva.Width))
                            {
                                maska[x, y] = 0;
                            }
                            else if (x == 2 && y == 2)
                            {
                                maska[x, y] = (int)imgSiva.Data[i + 1, j + 1, 0];
                            }

                        }
                    }

                    // primerjanje vrednosti v maski
                    for (x = 0; x < 3; x++)
                    {
                        for (y = 0; y < 3; y++)
                        {
                            //če je vrednost soseda manjša 
                            if (maska[x, y] <= maska[1, 1])
                            {
                                maska_primerjava[x, y] = 1;
                            }
                            //če je vrednost soseda večja ali enaka 
                            else if (maska[x, y] > maska[1, 1])
                            {
                                maska_primerjava[x, y] = 0;
                            }

                        }
                    }
                    //pretvorba v binarno stivilo
                    //zacnemo z sosedom na poziciji maska[0,0] in nadaljujemo v smeri urinega kazalca po maski
                    bin_vrednost = maska_primerjava[0, 0].ToString() + maska_primerjava[0, 1].ToString() + maska_primerjava[0, 2].ToString() + maska_primerjava[1, 2].ToString() + maska_primerjava[2, 2].ToString() + maska_primerjava[2, 1].ToString() + maska_primerjava[2, 0].ToString() + maska_primerjava[1, 0].ToString();

                    //polje pojavitve vrednosti
                    bin_v_int = Convert.ToInt32(bin_vrednost, 2);
                    //vnesemo klko krat se kera vrednost pojavi
                    vrednosti_pixlov[bin_v_int]++;
                    //nastavimo vrednost piksla v novi sliki
                    lbn.Data[i, j, 0] = (byte)bin_v_int;

                }
            }
            pictureBox2.Image = lbn.ToBitmap();
        }// end of LBPbasic

        private void LBPuniform()
        {
            int i;
            int j;
            int x;
            int y;
            int[,] maska = new int[3, 3];

            int[,] maska_primerjava = new int[3, 3];

            //int[] vrednosti_pixlov = new int[256];

            string bin_vrednost;
            int bin_v_int;

            imgSiva = My_ImageRecieve.Convert<Gray, Byte>();

            Image<Gray, byte> lbn = new Image<Gray, byte>(imgSiva.Width, imgSiva.Height);
            //gremo čez vse piksle slike
            for (i = 0; i < imgSiva.Height; i++)
            {
                for (j = 0; j < imgSiva.Width; j++)
                {

                    //dodajanje vrednosti v masko
                    for (x = 0; x < 3; x++)
                    {
                        for (y = 0; y < 3; y++)
                        {
                            //maska[0,0]
                            if (x == 0 && y == 0 && (i - 1 < 0 || j - 1 < 0))
                            {
                                maska[x, y] = 0;
                            }
                            else if (x == 0 && y == 0)
                            {
                                maska[x, y] = (int)imgSiva.Data[i - 1, j - 1, 0];
                            }
                            //maska[0,1]
                            if (x == 0 && y == 1 && i - 1 < 0)
                            {
                                maska[x, y] = 0;
                            }
                            else if (x == 0 && y == 1)
                            {

                                maska[x, y] = (int)imgSiva.Data[i - 1, j, 0];
                            }
                            //maska[0,2]
                            if (x == 0 && y == 2 && (i - 1 < 0 || j + 1 >= imgSiva.Width))
                            {
                                maska[x, y] = 0;
                            }
                            else if (x == 0 && y == 2)
                            {
                                maska[x, y] = (int)imgSiva.Data[i - 1, j + 1, 0];
                            }
                            //maska[1,0]
                            if (x == 1 && y == 0 && j - 1 < 0)
                            {
                                maska[x, y] = 0;
                            }
                            else if (x == 1 && y == 0)
                            {
                                maska[x, y] = (int)imgSiva.Data[i, j - 1, 0];
                            }
                            //maska[1,1]
                            if (x == 1 && y == 1)
                            {
                                maska[x, y] = (int)imgSiva.Data[i, j, 0];
                            }
                            //maska[1,2]
                            if (x == 1 && y == 2 && j + 1 >= imgSiva.Width)
                            {
                                maska[x, y] = 0;
                            }
                            else if (x == 1 && y == 2)
                            {
                                maska[x, y] = (int)imgSiva.Data[i, j + 1, 0];
                            }
                            //maska[2,0]
                            if (x == 2 && y == 0 && (i + 1 >= imgSiva.Height || j - 1 < 0))
                            {
                                maska[x, y] = 0;
                            }
                            else if (x == 2 && y == 0)
                            {
                                maska[x, y] = (int)imgSiva.Data[i + 1, j - 1, 0];
                            }
                            //maska[2,1]
                            if (x == 2 && y == 1 && i + 1 >= imgSiva.Height)
                            {
                                maska[x, y] = 0;
                            }
                            else if (x == 2 && y == 1)
                            {
                                maska[x, y] = (int)imgSiva.Data[i + 1, j, 0];
                            }

                            //maska[2,2]
                            if (x == 2 && y == 2 && (i + 1 >= imgSiva.Height || j + 1 >= imgSiva.Width))
                            {
                                maska[x, y] = 0;
                            }
                            else if (x == 2 && y == 2)
                            {
                                maska[x, y] = (int)imgSiva.Data[i + 1, j + 1, 0];
                            }

                        }
                    }

                    // primerjanje vrednosti v maski
                    for (x = 0; x < 3; x++)
                    {
                        for (y = 0; y < 3; y++)
                        {
                            //če je vrednost soseda manjša 
                            if (maska[x, y] <= maska[1, 1])
                            {
                                maska_primerjava[x, y] = 1;
                            }
                            //če je vrednost soseda večja ali enaka 
                            else if (maska[x, y] > maska[1, 1])
                            {
                                maska_primerjava[x, y] = 0;
                            }

                        }
                    }
                    //pretvorba v binarno stivilo
                    //zacnemo z sosedom na poziciji maska[0,0] in nadaljujemo v smeri urinega kazalca po maski
                    bin_vrednost = maska_primerjava[0, 0].ToString() + maska_primerjava[0, 1].ToString() + maska_primerjava[0, 2].ToString() + maska_primerjava[1, 2].ToString() + maska_primerjava[2, 2].ToString() + maska_primerjava[2, 1].ToString() + maska_primerjava[2, 0].ToString() + maska_primerjava[1, 0].ToString();

                    //polje pojavitve vrednosti
                    bin_v_int = Convert.ToInt32(bin_vrednost, 2);
                    int[] uniform_lookup = new int[256]{
                             0, 1, 2, 3, 4, 58, 5, 6, 7, 58, 58, 58, 8, 58, 9, 10,
                            11, 58, 58, 58, 58, 58, 58, 58, 12, 58, 58, 58, 13, 58, 14, 15,
                            16, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58,
                            17, 58, 58, 58, 58, 58, 58, 58, 18, 58, 58, 58, 19, 58, 20, 21,
                            22, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58,
                            58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58,
                            23, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58,
                            24, 58, 58, 58, 58, 58, 58, 58, 25, 58, 58, 58, 26, 58, 27, 28,
                            29, 30, 58, 31, 58, 58, 58, 32, 58, 58, 58, 58, 58, 58, 58, 33,
                            58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 34,
                            58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58,
                            58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 35,
                            36, 37, 58, 38, 58, 58, 58, 39, 58, 58, 58, 58, 58, 58, 58, 40,
                            58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 58, 41,
                            42, 43, 58, 44, 58, 58, 58, 45, 58, 58, 58, 58, 58, 58, 58, 46,
                            47, 48, 58, 49, 58, 58, 58, 50, 51, 52, 58, 53, 54, 55, 56, 57
                         };
                    vrednost_pixlov_uniformni[uniform_lookup[bin_v_int]]++;
                    lbn.Data[i, j, 0] = (byte)uniform_lookup[bin_v_int];
                }
            }
            pictureBox3.Image = lbn.ToBitmap();
        }// end of LBPuniform

        private void LBPtransition()
        {
            int i;
            int j;
            int x;
            int y;
            int[,] maska = new int[3, 3];

            int[,] maska_primerjava = new int[3, 3];

            //int[] vrednosti_pixlov = new int[256];

            string bin_vrednost;
            int bin_v_int;

            imgSiva = My_ImageRecieve.Convert<Gray, Byte>();

            lbn = new Image<Gray, byte>(imgSiva.Width, imgSiva.Height);
            //gremo čez vse piksle slike
            for (i = 0; i < imgSiva.Height; i++)
            {
                for (j = 0; j < imgSiva.Width; j++)
                {

                    //dodajanje vrednosti v masko
                    for (x = 0; x < 3; x++)
                    {
                        for (y = 0; y < 3; y++)
                        {
                            //maska[0,0]
                            if (x == 0 && y == 0 && (i - 1 < 0 || j - 1 < 0))
                            {
                                maska[x, y] = 0;
                            }
                            else if (x == 0 && y == 0)
                            {
                                maska[x, y] = (int)imgSiva.Data[i - 1, j - 1, 0];
                            }
                            //maska[0,1]
                            if (x == 0 && y == 1 && i - 1 < 0)
                            {
                                maska[x, y] = 0;
                            }
                            else if (x == 0 && y == 1)
                            {

                                maska[x, y] = (int)imgSiva.Data[i - 1, j, 0];
                            }
                            //maska[0,2]
                            if (x == 0 && y == 2 && (i - 1 < 0 || j + 1 >= imgSiva.Width))
                            {
                                maska[x, y] = 0;
                            }
                            else if (x == 0 && y == 2)
                            {
                                maska[x, y] = (int)imgSiva.Data[i - 1, j + 1, 0];
                            }
                            //maska[1,0]
                            if (x == 1 && y == 0 && j - 1 < 0)
                            {
                                maska[x, y] = 0;
                            }
                            else if (x == 1 && y == 0)
                            {
                                maska[x, y] = (int)imgSiva.Data[i, j - 1, 0];
                            }
                            //maska[1,1]
                            if (x == 1 && y == 1)
                            {
                                maska[x, y] = (int)imgSiva.Data[i, j, 0];
                            }
                            //maska[1,2]
                            if (x == 1 && y == 2 && j + 1 >= imgSiva.Width)
                            {
                                maska[x, y] = 0;
                            }
                            else if (x == 1 && y == 2)
                            {
                                maska[x, y] = (int)imgSiva.Data[i, j + 1, 0];
                            }
                            //maska[2,0]
                            if (x == 2 && y == 0 && (i + 1 >= imgSiva.Height || j - 1 < 0))
                            {
                                maska[x, y] = 0;
                            }
                            else if (x == 2 && y == 0)
                            {
                                maska[x, y] = (int)imgSiva.Data[i + 1, j - 1, 0];
                            }
                            //maska[2,1]
                            if (x == 2 && y == 1 && i + 1 >= imgSiva.Height)
                            {
                                maska[x, y] = 0;
                            }
                            else if (x == 2 && y == 1)
                            {
                                maska[x, y] = (int)imgSiva.Data[i + 1, j, 0];
                            }

                            //maska[2,2]
                            if (x == 2 && y == 2 && (i + 1 >= imgSiva.Height || j + 1 >= imgSiva.Width))
                            {
                                maska[x, y] = 0;
                            }
                            else if (x == 2 && y == 2)
                            {
                                maska[x, y] = (int)imgSiva.Data[i + 1, j + 1, 0];
                            }

                        }
                    }

                    // primerjanje vrednosti v maski
                    /**            transition            **/
                    int h;
                    int v;
                    //uppermost horizontal 00|01|02
                    h = 0;
                    for (v = 0; v < 2; v++)//do dva ker preveri prvega z drugim in drugega z tretjim in konec
                    {
                        if (maska[h, v] > maska[h, v + 1])
                        {
                            maska_primerjava[h, v] = 1;
                        }
                        else
                            maska_primerjava[h, v] = 0;
                    }
                    h = 0;
                    v = 0;
                    //rightmost vertical 02|12|22 continuing from |02
                    v = 2;
                    for (h = 0; h < 2; h++)//do dva ker preveri prvega z drugim in drugega z tretjim in konec
                    {
                        if (maska[h, v] > maska[h + 1, v])
                        {
                            maska_primerjava[h, v] = 1;
                        }
                        else
                            maska_primerjava[h, v] = 0;
                    }
                    h = 0;
                    v = 0;
                    //bottom horizontal 20|21|22 in reverse order continuing from |22
                    h = 2;
                    for (v = 2; v > 0; v--)//do dva ker preveri prvega z drugim in drugega z tretjim in konec
                    {
                        if (maska[h, v] > maska[h, v - 1])
                        {
                            maska_primerjava[h, v] = 1;
                        }
                        else
                            maska_primerjava[h, v] = 0;
                    }
                    h = 0;
                    v = 0;
                    //leftmost vertical 00|10|20 continuing from 20| again in reverse from bottom up
                    v = 0;
                    for (h = 2; h > 0; h--)//do dva ker preveri prvega z drugim in drugega z tretjim in konec
                    {
                        if (maska[h, v] > maska[h - 1, v])
                        {
                            maska_primerjava[h, v] = 1;
                        }
                        else
                            maska_primerjava[h, v] = 0;
                    }
                    h = 0;
                    v = 0;
                    /**          transition end          **/
                    //pretvorba v binarno stivilo
                    //zacnemo z sosedom na poziciji maska[0,0] in nadaljujemo v smeri urinega kazalca po maski
                    bin_vrednost = maska_primerjava[0, 0].ToString() + maska_primerjava[0, 1].ToString() + maska_primerjava[0, 2].ToString() + maska_primerjava[1, 2].ToString() + maska_primerjava[2, 2].ToString() + maska_primerjava[2, 1].ToString() + maska_primerjava[2, 0].ToString() + maska_primerjava[1, 0].ToString();

                    //polje pojavitve vrednosti
                    bin_v_int = Convert.ToInt32(bin_vrednost, 2);
                    //vnesemo klko krat se kera vrednost pojavi
                    vrednosti_pixlov[bin_v_int]++;
                    //nastavimo vrednost piksla v novi sliki
                    lbn.Data[i, j, 0] = (byte)bin_v_int;

                }
            }
            pictureBox4.Image = lbn.ToBitmap();
        }// end of LBPtransition

        /** Histogram of Oriented Gradients **/
        struct binType
        {
            public float[] beans;
        };

        float[] bins = new float[9];
        int binMatX = 0;
        int binMatY = 0;
        int currentbinMatY = 0;

        private void HOG()
        {
            Image<Gray, byte> slika = My_ImageRecieve.Convert<Gray, byte>();
            Image<Gray, float> photoX = slika.Sobel(1, 0, 1);
            Image<Gray, float> photoY = slika.Sobel(0, 1, 1);

            Image<Gray, float> magnitude = new Image<Gray, float>(slika.Size);  //magnitude spremenljivka za sliko
            Image<Gray, float> direction = new Image<Gray, float>(slika.Size); //spremenljivka za kote 

            var binsMat = new float[5000,10000][];
            binType[,] beansMat = new binType[5000,10000];
            List< List<float[]> > biniMat2d = new List< List< float[] > >();
            List<float[]> biniMat1d = new List<float[]>();

            CvInvoke.CartToPolar(photoX, photoY, magnitude, direction, true);

            //pretvorba iz 0-360 v 0-180
            for (int i = 0; i < direction.Height; i++)
            {
                for(int j = 0; j < direction.Width; j++)
                {
                    int temp = (int)direction.Data[i, j, 0];
                    temp = temp - 360;
                    if (temp < 0)
                        temp = temp * -1;
                    direction.Data[i, j, 0] = (byte)temp;
                }
            }
            //konec pretvorbe

            pictureBox5.Image = direction.ToBitmap();

            int[,] celica = new int[8, 8];


            for (int x = 0; x < direction.Height-8; x=x+8)
            {
                if (direction.Height - x >= 1)
                {
                    currentbinMatY = binMatY;
                    Console.WriteLine(binMatY);
                    binMatY = 0;
                    for (int y = 0; y < direction.Width-8; y=y+8)
                    {
                        if (direction.Width - y >= 1)
                        {

                            for (int i = 0; i < 8; i++)
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    bins = new float[9];
                                    celica[i, j] = (int)direction.Data[x+i, y+j, 0];

                                    if (celica[i,j] == 0 || celica[i,j] == 180)//0 ali 180 je enako //POLNI BIN
                                    {
                                        bins[0] = (float)magnitude.Data[x + i, y + j, 0];
                                    }
                                    else if (celica[i, j] > 0 && celica[i, j] < 20)//RAZDELITEV MED SOSEDE
                                    {
                                        int tmpDir = celica[i,j];
                                        float subLbin = tmpDir - 0;
                                        float part = subLbin / 20;
                                        float part2 = 1 - part;

                                        float getMag = (float)magnitude.Data[x + i, y + j, 0];
                                        if (celica[i,j] < 10)
                                        {
                                            bins[0] = getMag * part2;
                                            bins[1] = getMag * part;
                                        }
                                        else if (celica[i,j] > 10)
                                        {
                                            bins[0] = getMag * part;
                                            bins[1] = getMag * part2;
                                        }
                                        else
                                        {
                                            bins[0] = getMag * part;
                                            bins[1] = getMag * part2;
                                        }

                                    }
                                    else if (celica[i,j] == 20)//POLNI BIN
                                    {
                                        bins[1] = (float)magnitude.Data[x + i, y + j, 0];
                                    }
                                    else if (celica[i, j] > 20 && celica[i, j] < 40)//RAZDELITEV MED SOSEDE
                                    {
                                        int tmpDir = celica[i, j];
                                        float subLbin = tmpDir - 20;
                                        float part = subLbin / 20;
                                        float part2 = 1 - part;

                                        float getMag = (float)magnitude.Data[x + i, y + j, 0];
                                        if (celica[i, j] < 30)
                                        {
                                            bins[1] = getMag * part2;
                                            bins[2] = getMag * part;
                                        }
                                        else if (celica[i, j] > 30)
                                        {
                                            bins[1] = getMag * part;
                                            bins[2] = getMag * part2;
                                        }
                                        else
                                        {
                                            bins[1] = getMag * part;
                                            bins[2] = getMag * part2;
                                        }
                                    }
                                    else if (celica[i,j] == 40)//POLNI BIN
                                    {
                                        bins[2] = (float)magnitude.Data[x + i, y + j, 0];
                                    }
                                    else if (celica[i, j] > 40 && celica[i, j] < 60)//RAZDELITEV MED SOSEDE
                                    {
                                        int tmpDir = celica[i, j];
                                        float subLbin = tmpDir - 40;
                                        float part = subLbin / 20;
                                        float part2 = 1 - part;

                                        float getMag = (float)magnitude.Data[x + i, y + j, 0];
                                        if (celica[i, j] < 50)
                                        {
                                            bins[2] = getMag * part2;
                                            bins[3] = getMag * part;
                                        }
                                        else if (celica[i, j] > 50)
                                        {
                                            bins[2] = getMag * part;
                                            bins[3] = getMag * part2;
                                        }
                                        else
                                        {
                                            bins[2] = getMag * part;
                                            bins[3] = getMag * part2;
                                        }
                                    }
                                    else if (celica[i,j] == 60)//POLNI BIN
                                    {
                                        bins[3] = (float)magnitude.Data[x + i, y + j, 0];
                                    }
                                    else if (celica[i, j] > 60 && celica[i, j] < 80)//RAZDELITEV MED SOSEDE
                                    {
                                        int tmpDir = celica[i, j];
                                        float subLbin = tmpDir - 60;
                                        float part = subLbin / 20;
                                        float part2 = 1 - part;

                                        float getMag = (float)magnitude.Data[x + i, y + j, 0];
                                        if (celica[i, j] < 70)
                                        {
                                            bins[3] = getMag * part2;
                                            bins[4] = getMag * part;
                                        }
                                        else if (celica[i, j] > 70)
                                        {
                                            bins[3] = getMag * part;
                                            bins[4] = getMag * part2;
                                        }
                                        else
                                        {
                                            bins[3] = getMag * part;
                                            bins[4] = getMag * part2;
                                        }
                                    }
                                    else if (celica[i,j] == 80)//POLNI BIN
                                    {
                                        bins[4] = (float)magnitude.Data[x + i, y + j, 0];
                                    }
                                    else if (celica[i, j] > 80 && celica[i, j] < 100)//RAZDELITEV MED SOSEDE
                                    {
                                        int tmpDir = celica[i, j];
                                        float subLbin = tmpDir - 80;
                                        float part = subLbin / 20;
                                        float part2 = 1 - part;

                                        float getMag = (float)magnitude.Data[x + i, y + j, 0];
                                        if (celica[i, j] < 90)
                                        {
                                            bins[4] = getMag * part2;
                                            bins[5] = getMag * part;
                                        }
                                        else if (celica[i, j] > 90)
                                        {
                                            bins[4] = getMag * part;
                                            bins[5] = getMag * part2;
                                        }
                                        else
                                        {
                                            bins[4] = getMag * part;
                                            bins[5] = getMag * part2;
                                        }
                                    }
                                    else if (celica[i,j] == 100)//POLNI BIN
                                    {
                                        bins[5] = (float)magnitude.Data[x + i, y + j, 0];
                                    }
                                    else if (celica[i, j] > 100 && celica[i, j] < 120)//RAZDELITEV MED SOSEDE
                                    {
                                        int tmpDir = celica[i, j];
                                        float subLbin = tmpDir - 100;
                                        float part = subLbin / 20;
                                        float part2 = 1 - part;

                                        float getMag = (float)magnitude.Data[x + i, y + j, 0];
                                        Console.WriteLine(magnitude.Data[x + i, y + j, 0]);
                                        if (celica[i, j] < 110)
                                        {
                                            bins[5] = getMag * part2;
                                            bins[6] = getMag * part;
                                        }
                                        else if (celica[i, j] > 110)
                                        {
                                            bins[5] = getMag * part;
                                            bins[6] = getMag * part2;
                                        }
                                        else
                                        {
                                            bins[5] = getMag * part;
                                            bins[6] = getMag * part2;
                                        }
                                    }
                                    else if (celica[i,j] == 120)//POLNI BIN
                                    {
                                        bins[6] = (float)magnitude.Data[x + i, y + j, 0];
                                    }
                                    else if (celica[i, j] > 120 && celica[i, j] < 140)//RAZDELITEV MED SOSEDE
                                    {
                                        int tmpDir = celica[i, j];
                                        float subLbin = tmpDir - 120;
                                        float part = subLbin / 20;
                                        float part2 = 1 - part;

                                        float getMag = (float)magnitude.Data[x + i, y + j, 0];
                                        if (celica[i, j] < 130)
                                        {
                                            bins[6] = getMag * part2;
                                            bins[7] = getMag * part;
                                        }
                                        else if (celica[i, j] > 130)
                                        {
                                            bins[6] = getMag * part;
                                            bins[7] = getMag * part2;
                                        }
                                        else
                                        {
                                            bins[6] = getMag * part;
                                            bins[7] = getMag * part2;
                                        }
                                    }
                                    else if (celica[i,j] == 140)//POLNI BIN
                                    {
                                        bins[7] = (float)magnitude.Data[x + i, y + j, 0];
                                    }
                                    else if (celica[i, j] > 140 && celica[i, j] < 160)//RAZDELITEV MED SOSEDE
                                    {
                                        int tmpDir = celica[i, j];
                                        float subLbin = tmpDir - 140;
                                        float part = subLbin / 20;
                                        float part2 = 1 - part;

                                        float getMag = (float)magnitude.Data[x + i, y + j, 0];
                                        if (celica[i, j] < 150)
                                        {
                                            bins[7] = getMag * part2;
                                            bins[8] = getMag * part;
                                        }
                                        else if (celica[i, j] > 150)
                                        {
                                            bins[7] = getMag * part;
                                            bins[8] = getMag * part2;
                                        }
                                        else
                                        {
                                            bins[7] = getMag * part;
                                            bins[8] = getMag * part2;
                                        }
                                    }
                                    else if (celica[i,j] == 160)//POLNI BIN
                                    {
                                        bins[8] = (float)magnitude.Data[x + i, y + j, 0];
                                    }
                                    else if (celica[i, j] > 160 && celica[i, j] < 180)//RAZDELITEV MED SOSEDE
                                    {
                                        int tmpDir = celica[i, j];
                                        float subLbin = tmpDir - 160;
                                        float part = subLbin / 20;
                                        float part2 = 1 - part;

                                        float getMag = (float)magnitude.Data[x + i, y + j, 0];
                                        if (celica[i, j] < 170)
                                        {
                                            bins[8] = getMag * part2;
                                            bins[0] = getMag * part;
                                        }
                                        else if (celica[i, j] > 170)
                                        {
                                            bins[8] = getMag * part;
                                            bins[0] = getMag * part2;
                                        }
                                        else
                                        {
                                            bins[8] = getMag * part;
                                            bins[0] = getMag * part2;
                                        }
                                    }
                                }//j celica
                            }//i celica
                            biniMat1d.Add(bins);//list
                            beansMat[binMatX, binMatY].beans = bins;//probamo z strukturo
                            binsMat[binMatX, binMatY] = bins;//2d array of arrays
                            //Console.Write(beansMat[binMatX, binMatY].beans);
                            binMatY++;//++ y coordinate of 2d array of arrays
                        }//znotraj ifa za lego [i,j]WIDTH
                    }//direction width
                    biniMat2d.Add(biniMat1d);//list
                    binMatX++;//++ x coordinate of 2d array of arrays
                }//znotraj ifa za lego [i,j]HEIGHT
            }//direction height
            Console.WriteLine("________");
            float[] temp1 = new float[9];
            for (int i = 0; i < binMatX - 1; i++)//-1 ker bomo z x + x+1 vedno pogledali z predzadnjim tudi zadnjega v x in y smeri
            {
                for (int j = 0; j < binMatY - 1; j++)//-||-
                {
                    Console.WriteLine("[" + i + "," + j + "]  = ");
                    //Console.WriteLine(beansMat[i, j].beans);
                    temp1 = beansMat[i, j].beans;
                    for (int k = 0; k < 9; k++)
                    {
                        Console.Write(temp1[k]);
                    }
                    Console.WriteLine(".");
                }//end of Y
            }//end of X
            Console.WriteLine("______________");
            //prepare cells for storage of bins for each cell and 1 blockBins that will concatenate 4 cells into 1 blockbin(4bins->1bin)
            float[] cell1 = new float[9];
            float[] cell2 = new float[9];
            float[] cell3 = new float[9];
            float[] cell4 = new float[9];
            float[] blockBins = new float[37];
            int blockBinC = 0;//counter
            float[] concatBlockBinsNormalized = new float[binMatX * binMatY * 36];
            int concatBlockBinsNormalizedC = 0;//counter
            //prepare a SortedDictionary TODO
            Dictionary<float, int> dictionary = new Dictionary<float, int>();
            for (int i = 0; i < binMatX-1; i++)//-1 ker bomo z x + x+1 vedno pogledali z predzadnjim tudi zadnjega v x in y smeri
            {
                for(int j = 0; j < binMatY-1; j++)//-||-
                {
                    //cell1 = binsMat[i, j];
                    cell1 = beansMat[i, j].beans;
                    for (int l = 0; l < 9; l++)
                    {
                        Console.Write(cell1[l]);
                    }//console wrrite test
                    Console.WriteLine(".");
                    for (int l = 0; l < 9; l++)
                    {
                        blockBins[blockBinC] = cell1[l];
                        blockBinC++;
                    }//go through floats in each cell and copy them to blockBins
                    //cell2 = binsMat[i, j+1];
                    cell2 = beansMat[i, j+1].beans;
                    for (int l = 0; l < 9; l++)
                    {
                        blockBins[blockBinC] = cell2[l];
                        blockBinC++;
                    }//go through floats in each cell and copy them to blockBins
                    //cell3 = binsMat[i+1, j];
                    cell3 = beansMat[i+1, j].beans;
                    for (int l = 0; l < 9; l++)
                    {
                        blockBins[blockBinC] = cell3[l];
                        blockBinC++;
                    }//go through floats in each cell and copy them to blockBins
                    //cell4 = binsMat[i+1, j+1];
                    cell4 = beansMat[i+1, j+1].beans;
                    for (int l = 0; l < 9; l++)
                    {
                        blockBins[blockBinC] = cell4[l];
                        blockBinC++;
                    }//go through floats in each cell and copy them to blockBins
                    
                    //normalizacija blockBins:
                    float[] blockBinsNormalized = new float[37];
                    float suma = 0;
                    for (int k = 0; k < 37; k++)//racunanje skale
                    {
                        suma = suma + (blockBins[k]* blockBins[k]);
                    }
                    float scale = (float)Math.Sqrt(suma);//skala
                    for (int k = 0; k < 37; k++)//normalizacija blockBins*scale -> blockBinsNormalized
                    {
                        blockBinsNormalized[k] = blockBins[k] * scale;
                    }
                    for(int k = 0; k < 37; k++)//zlivanje v koncni vector vseh vrednosti
                    {
                        concatBlockBinsNormalized[concatBlockBinsNormalizedC] = blockBinsNormalized[k];
                        concatBlockBinsNormalizedC++;
                    }
                    blockBinC = 0;
                }//end of Y
            }//end of X
            //izdelava slovarja
            for (int i = 0; i < concatBlockBinsNormalizedC; i++)//zlivanje v koncni vector vseh vrednosti
            {
                // See whether it contains this string.
                if (!dictionary.ContainsKey(concatBlockBinsNormalized[i]))
                {
                    dictionary.Add(concatBlockBinsNormalized[i], 1);
                }
                if (dictionary.ContainsKey(concatBlockBinsNormalized[i]))
                {
                    dictionary[concatBlockBinsNormalized[i]]++;
                }
            }
            foreach (KeyValuePair<float, int> pair in dictionary)
            {
                Console.WriteLine("{0}, {1}", pair.Key, pair.Value);
            }
        }

        private void button2_Click(object sender, EventArgs e)//LBPbasic BUTTON
        {
            LBPbasic();
            insertMe();
        }

        private void button3_Click(object sender, EventArgs e)//LBPuniform BUTTON
        {
            LBPuniform();
            insertMe();
        }

        private void button4_Click(object sender, EventArgs e)//LBPtransition BUTTON
        {
            LBPtransition();
            insertMe();
        }

        private void button5_Click(object sender, EventArgs e)//HOG BUTTON
        {
            HOG();
        }
    }

    /*PRIMER LIST OF LIST
     * 
        List<List<string>> myList = new List<List<string>>();
        myList.Add(new List<string> { "a", "b" });
        myList.Add(new List<string> { "c", "d", "e" });
        myList.Add(new List<string> { "qwerty", "asdf", "zxcv" });
        myList.Add(new List<string> { "a", "b" });

        // To iterate over it.
        foreach (List<string> subList in myList)
        {
            foreach (string item in subList)
            {
                Console.WriteLine(item);
            }
        }
     * */
    /*PRIMER 2d array of array oziroma primer array of 2d arrays(ravno obratno)
     * One more curly bracket set {} is required in the initial declaration:

        var waypoints = new int[4][,]   {
           new int[,] {{6}, {0}},
           new int[,] {{1}, {1}},
           new int[,] {{1}, {5}},
           new int[,] {{6}, {5}}
        };
     * 
     * 
     * 
     * 
     * 
     * */
}
