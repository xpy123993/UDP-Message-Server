using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;


namespace pet_v0
{

    delegate void UpdateInfo(String providerName, String text);

    


    public interface InfoWindow
    {
        void OnInfoUpdated(String providerName, String text);
    }

    public partial class Form1 : Form, InfoWindow
    {

        private Point mouseOff;
        private bool dragFlag = false;

        private int window_height = 0;  // the height of infowindow users acutally see, anime parameter

        private const int action_speed = 20;

        public const String textFile = "temp.rts";

        List<InfoProvider> providers = new List<InfoProvider>();


        public Form1()
        {
            InitializeComponent();
        }

        
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOff = new Point(-e.X, -e.Y);
                dragFlag = true;
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragFlag)
            {
                Point mouseSet = Control.MousePosition;
                mouseSet.Offset(mouseOff.X - pictureBox1.Left, mouseOff.Y - pictureBox1.Top); 
                Location = mouseSet;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (dragFlag)
            {
                dragFlag = false;
            }
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //textBox1.Visible = !textBox1.Visible;
            if (groupBox1.Visible)
            {
                fade_countdown = 1;
                anime_window_disappear();
            }
                
            else
                anime_window_appear();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SaveFile(textFile);
            foreach (InfoProvider provider in providers)
                provider.close();
            Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void initialize_provider()
        {
            InfoProvider provider = new UDPListener();
            provider.bindInfoWindow(this);
            providers.Add(provider);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Location = new Point(1920 - Width - 20, 1080 - Height - 40);
            window_height = groupBox1.Height;

            if (File.Exists(textFile))
            {
                //richTextBox1.LoadFile(textFile);
            }

            richTextBox1.Text = "";

            initialize_provider();
        }

        private void appand_text(String text, Color color, bool isBold, float fontSize)//normal: 14.25
        {
            int start_location = richTextBox1.Text.Length;
            richTextBox1.AppendText(text);
            richTextBox1.Select(start_location, text.Length);
            richTextBox1.SelectionColor = color;
            if(isBold)
                richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont.FontFamily, fontSize, FontStyle.Bold);
            else
                richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont.FontFamily, fontSize, FontStyle.Regular);

        }

        private void update_infoWindow(String providerName, String text)
        {
            if (richTextBox1.InvokeRequired)
            {
                while (!richTextBox1.IsHandleCreated)
                {
                    if (richTextBox1.Disposing || richTextBox1.IsDisposed)
                        return;
                }
                UpdateInfo u = new UpdateInfo(update_infoWindow);
                richTextBox1.Invoke(u, new object[] { providerName, text });
            }
            else
            {
                //groupBox1.Visible = true;
                String mk = providerName + " (" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + ") \r\n";
                anime_window_appear();
                appand_text(mk, Color.LightPink, false, 14.25f);
                mk = text + "\r\n\r\n";
                appand_text(mk, Color.LightGreen, false, 13.25f);
                
                
                after_update_info();
            }
        }

        void anime_window_appear()
        {
            if (!groupBox1.Visible && !timer1.Enabled)
            {
                groupBox1.Height = 10;
                groupBox1.Visible = true;

                timer2.Interval = 10;
                timer2.Start();
            }
        }

        void anime_window_disappear()
        {

            if(groupBox1.Visible && !timer2.Enabled)
            {
                timer1.Interval = 10;
                timer1.Start();
            }
            
        }

        void after_update_info()
        {
            start_count_down(10);
            //anime_window_disappear();

            richTextBox1.Select(richTextBox1.Text.Length - 1, 0);
            richTextBox1.ScrollToCaret();
        }

        void InfoWindow.OnInfoUpdated(string providerName, string text)
        {
            update_infoWindow(providerName, text);
        }

        private void do_fade()
        {
            if (groupBox1.Height <= 10)
            {
                timer1.Stop();
                groupBox1.Visible = false;
                groupBox1.Height = window_height;
            }
            else
            {
                groupBox1.Height -= action_speed;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!groupBox1.Visible)
                timer1.Stop();

            do_fade();
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (!groupBox1.Visible)
                timer2.Stop();

            if (groupBox1.Height < window_height)
                groupBox1.Height += action_speed;
            else if (groupBox1.Height >= window_height)
                timer2.Stop();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (InfoProvider provider in providers)
                provider.close();
        }

        int fade_countdown = 10;

        private void start_count_down(int second)
        {
            fade_countdown = second;
            timer3.Interval = 1000;
            timer3.Start();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            fade_countdown--;
            
            if(fade_countdown <= 0 || !groupBox1.Visible)
            {
                timer3.Stop();
                anime_window_disappear();
            }
        }
    }
}
