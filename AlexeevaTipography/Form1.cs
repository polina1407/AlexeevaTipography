using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace AlexeevaTipography
{
    public partial class OpenForm : Form
    {
        public OpenForm()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void OpenForm_Load(object sender, EventArgs e)
        {
      
            StartGifAnimation();
        }
        private int currentFrame = 0; 
        private void StartGifAnimation()
        {

            Timer timer = new Timer();
            timer.Interval = 25;  
            timer.Tick += (s, e) =>
            {

                ImageAnimator.UpdateFrames(pictureBox5.Image);

                currentFrame++;

                if (currentFrame >= pictureBox5.Image.GetFrameCount(FrameDimension.Time))
                {
                    timer.Stop();
                    pictureBox5.Visible = false;  
                }
            };
            timer.Start();
        }

        private bool IsAnimationFinished()
        {

            return pictureBox5.Image.SelectActiveFrame(FrameDimension.Time, 0) == pictureBox5.Image.GetFrameCount(FrameDimension.Time) - 1;
        }
    
    private void pictureBox2_Click(object sender, EventArgs e)
        {
            LogIn form2 = new LogIn();
            form2.ShowDialog();
            this.Close();
        }



        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            SighUp form2 = new SighUp();
            form2.ShowDialog();
            this.Close();
        }
    }
    }

