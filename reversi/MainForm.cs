using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace reversi
{
    public partial class MainForm : Form
    {
        List<PictureBox> pictureBoxes = new List<PictureBox>();
        internal readonly int N = 8;
        internal readonly int a = 40;
        internal readonly int margin = 20;
        internal readonly Bitmap K = GetImageByName("K");
        internal readonly Bitmap F = GetImageByName("F");
        internal readonly String kivalasztando = "{0} > {1} | Reversi";
        internal String[] lines;
        public MainForm()
        {
            InitializeComponent();
            Feltolt();
            Beolvas();
        }

        private void Beolvas()
        {
            lines = File.ReadAllLines("allas.txt");

            int firstX = -1, firstY = -1, secondX = -1, secondY = -1;
            Color eredetiHatter = Color.White;
            for (int i = 0; i < pictureBoxes.Count; i++)
            {
                int x = i % N, y = i / N;
                pictureBoxes[i].BackgroundImage = lines[x][y] == 'K' ? K : (lines[x][y] == 'F' ? F : pictureBoxes[i].BackgroundImage);
                pictureBoxes[i].Click += delegate (object sender, EventArgs ex)
                {
                    if (firstX == -1 && firstY == -1)
                    {
                        if (lines[x][y] == 'K' || lines[x][y] == 'F')
                        {
                            firstX = x; firstY = y;
                            MessageBox.Show((i) + "\n" + (firstX * N + firstY));
                            Text = kivalasztando.Replace("{0}", lines[x][y] == 'K' ? "Kék" : lines[x][y] == 'F' ? "Fehér" : "").Replace(" {1}", "");
                            eredetiHatter = ((PictureBox)sender).BackColor;
                            //((PictureBox)sender).BackColor = Color.Aqua;
                            ((PictureBox)sender).BackColor = ((x + 1) + (y % 2)) % 2 == 0 ? Color.RoyalBlue : Color.Aqua;
                        }
                    }
                    else if (secondX == -1 && secondY == -1)
                    {
                        secondX = x; secondY = y;
                        Text = kivalasztando.Replace("{0}", lines[firstX][firstY] == 'K' ? "Kék" : lines[firstX][firstY] == 'F' ? "Fehér" : "").Replace("{1}", SzabalyosLepes(lines[x][y], x, y) ? "SZABÁLYOS" : "SZABÁLYTALAN");
                    }
                    else if (secondX > -1 && secondY > -1 && firstX > -1 && firstY > -1)
                    {
                        RepaintBackground();
                        firstX = -1; firstY = -1; secondX = -1; secondY = -1;
                    }
                };

            }
        }

        private void RepaintBackground()
        {
            for (int i = 0; i < pictureBoxes.Count; i++)
            {
                Text = "Reversi";
                int x = i % N, y = i / N;
                pictureBoxes[i].BackColor = ((x + 1) + (y % 2)) % 2 == 0 ? Color.Gray : Color.LightGray;
            }
        }

        private bool VanForditas(char jatekos, int sor, int oszlop, int iranySor, int iranyOszlop)
        {
            int aktSor = sor + iranySor, aktOszlop = oszlop + iranyOszlop;
            char ellenfel = jatekos == 'K' ? 'F' : 'K';
            bool nincsEllenfel = true;
            while (aktSor > 0 && aktSor < 8 && aktOszlop > 0 && aktOszlop < 8 && lines[aktSor][aktOszlop] == ellenfel)
            {
                aktSor = aktSor + iranySor;
                aktOszlop = aktOszlop + iranyOszlop;
                nincsEllenfel = false;
            }
            if (nincsEllenfel || aktSor < 0 || aktSor > 7 || aktOszlop < 0 || aktOszlop > 7 || lines[aktSor][aktOszlop] != jatekos) return false;

            return true;
        }

        private bool SzabalyosLepes(char jatekos, int sor, int oszlop)
        {
            return VanForditas(jatekos, sor, oszlop, -1, -1) ||
                VanForditas(jatekos, sor, oszlop, -1, 0) ||
                VanForditas(jatekos, sor, oszlop, -1, 1) ||
                VanForditas(jatekos, sor, oszlop, 0, -1) ||
                VanForditas(jatekos, sor, oszlop, 0, 1) ||
                VanForditas(jatekos, sor, oszlop, 1, -1) ||
                VanForditas(jatekos, sor, oszlop, 1, 0) ||
                VanForditas(jatekos, sor, oszlop, 1, 1);
        }
        private void Feltolt()
        {
            int elemekhossza = a * N + 2 * margin;
            this.Size = new System.Drawing.Size(15 + elemekhossza, 38 + elemekhossza);
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    PictureBox pb = new PictureBox();
                    int x = (i * a) + margin;
                    int y = (j * a) + margin;
                    pb.Location = new System.Drawing.Point(x, y);
                    pb.BackColor = ((i + 1) + (j % 2)) % 2 == 0 ? Color.Gray : Color.LightGray;
                    pb.Size = new System.Drawing.Size(a, a);
                    pb.BackgroundImageLayout = ImageLayout.Zoom;
                    pictureBoxes.Add(pb);
                }
            }
            this.Controls.AddRange(pictureBoxes.ToArray());
        }

        public static Bitmap GetImageByName(string imageName)
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            string resourceName = asm.GetName().Name + ".Properties.Resources";
            var rm = new System.Resources.ResourceManager(resourceName, asm);
            return (Bitmap)rm.GetObject(imageName);

        }
    }
}
