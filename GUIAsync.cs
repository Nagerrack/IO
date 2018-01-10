using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gui_Async
{
    public partial class Form1 : Form
    {
        //Counter zaimplementowany w celu przetestowania responsywnosci GUI podczas ladowania
        private static int Counter = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void clickButton_Click(object sender, EventArgs e)
        {
            Counter++;
            clickButton.Text = Counter.ToString();
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Files | *.jpg; *.jpeg; *.png";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Task loadTask = CustomLoadAsync(ofd.FileName, pictureBox1);
            }
        }

        private static async Task CustomLoadAsync(string fileName, PictureBox p)
        {
            await Task.Run(
                () =>
                {
                    //Sztuczne wydluzenie ladowania
                    Thread.Sleep(5000);
                    p.Image = new Bitmap(fileName);
                });
        }
    }
}