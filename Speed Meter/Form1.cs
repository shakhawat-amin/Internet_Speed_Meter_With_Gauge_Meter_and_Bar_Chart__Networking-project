using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using Add_And_Remove_Programme_To_Or_From_Startup;
using System.Collections;
using System.Runtime.InteropServices;

namespace Speed_Meter
{
    public partial class Form1 : Form
    {
        private NetworkInterface[] nicArr;
        private NetworkInterface activeConnection;
        string bytesent = "0";
        string byterecieved = "0";
        private bool isMaximized = false;
        private bool close = true;
        int x = 0;
        double [] chartUploadSpeed= new double [20];
        double[] chartDownloadSpeed = new double [20];

        //string startupKey = "speedmeter";

        private long[] byteRecieved;
        private long[] byteSent;
        private bool[] connection;

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        public Form1()
        {
            this.Location = new Point(400, 380);
            InitializeComponent();
            init();
            timer1.Start();

            //Startup.AddProgrammeToStartUp(startupKey);
           

        }

        private void init()
        {
            nicArr = NetworkInterface.GetAllNetworkInterfaces();
            byteRecieved = new long[nicArr.Length];
            byteSent = new long[nicArr.Length];
            connection = new bool[nicArr.Length];

            for(int i = 0; i < nicArr.Length; i++)
            {
                connection[i] = false;
                byteSent[i] = 0;
                byteRecieved[i] = 0;
            }
            for (int i = 0; i < 20; i++)
                chartDownloadSpeed[i] = chartUploadSpeed[i] = 0;
            
            
            chart1.ChartAreas[0].AxisY.ScaleView.Zoom(0.0, 20.0);
            chart1.ChartAreas[0].AxisX.ScaleView.Zoom(0.0, 20.0);
            
            chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart1.ChartAreas[0].CursorY.IsUserEnabled = true;
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
            //chart1.Series.Add("download");
            //chart1.Series[1].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;

        }
        private void checkAvailableConnection()
        {
            bool flag = false;
            int pos = 0;
            IPv4InterfaceStatistics interfaceStats;
            
            for(int i = 0; i < nicArr.Length; i++)
            {
                interfaceStats = nicArr[i].GetIPv4Statistics();
                if(interfaceStats.BytesReceived != byteRecieved[i] || interfaceStats.BytesSent != byteSent[i])
                {
                    activeConnection = nicArr[i];
                    flag = true;
                    pos = i;
                   
                }

              

            }
            
            int byteSentPerSec;
            int byteRecievedPerSec;

            if(flag){
                interfaceStats = nicArr[pos].GetIPv4Statistics();
                byteSentPerSec = (int)(interfaceStats.BytesSent - (double)byteSent[pos]) / 1024;
                byteRecievedPerSec = (int)(interfaceStats.BytesReceived - (double)byteRecieved[pos]) / 1024;
            }

            else
            {
                byteRecievedPerSec = 0;
                byteSentPerSec = 0;
            }

            int modForChart = x % 20;


            if (byteSentPerSec > 1000)
                byteSentPerSec = 1000;
            if (byteRecievedPerSec > 1000)
                byteRecievedPerSec = 1000;
            chartDownloadSpeed[modForChart] = byteSentPerSec;
            chartUploadSpeed[modForChart] = byteRecievedPerSec;
            //for (int i = 0; i < chart1.Series[0].Points.Count; i++)
            //{
            //    chart1.Series[0].Points.RemoveAt(i);
            //}
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            int j = 0;
            for (int i = modForChart + 1; i < 20; i++, j++)
            {
                System.Console.WriteLine(modForChart);
                chart1.Series[1].Points.AddXY((double)j, (double)chartUploadSpeed[i]);
                //chart1.Series[0].Points[j].Color = System.Drawing.Color.Red;
                chart1.Series[0].Points.AddXY((double)j, (double) chartDownloadSpeed[i]);
            }
            for (int i = 0; i <= modForChart; i++,j++)
            {
                chart1.Series[1].Points.AddXY((double)j, (double) chartUploadSpeed[i]);
                chart1.Series[0].Points.AddXY((double)j, (double) chartDownloadSpeed[i]);
            }
           // chart1.Series[0].Points.AddXY(0.0, 00.0);
            
           // chart1.Series[0].Points.AddXY(1.0, 10.0);
           // chart1.Series[0].Points.AddXY(2.0, 5.0);
           //// chart1.Series[0].Points.AddXY(3.0, 300.0);
           // chart1.Series[0].Points.AddXY(3.0, 20.0);

            lblUploadSpeed.Text = byteSentPerSec + " kb/s";
            lblAgueUp.Text = byteSentPerSec + " kb/s";

            lbldownload.Text = byteRecievedPerSec + " kb/s";
            lblAgueDown.Text = byteRecievedPerSec + " kb/s";


            aGaugeDown.Value = byteRecievedPerSec;
            aGaugeUp.Value = byteSentPerSec;

            if (close && x > 0)
            {
                this.Size = new Size(339, 36);
                close = false;
            }

            for(int i = 0; i < nicArr.Length; i++)
            {
                interfaceStats = nicArr[i].GetIPv4Statistics();

                byteSent[i] = interfaceStats.BytesSent;
                byteRecieved[i] = interfaceStats.BytesReceived;
            }

            

        }

       


        private void initialize()
        {
           
            nicArr = NetworkInterface.GetAllNetworkInterfaces();
            NetworkInterface n = nicArr[3];
            for(int i = 0; i < nicArr.Length; i++)
            {
                System.Console.WriteLine(nicArr[i].Name);
                if (nicArr[i].Name == "Wi-Fi")
                {
                    n = nicArr[i];
                    break;
                }
                
            }
            
            IPv4InterfaceStatistics interfaceStats = n.GetIPv4Statistics();


            int bytesSentSpeed = (int)(interfaceStats.BytesSent - double.Parse(bytesent)) / 1024;
            int bytesReceivedSpeed = (int)(interfaceStats.BytesReceived - double.Parse(byterecieved)) / 1024;

            bytesent = interfaceStats.BytesSent.ToString();
            byterecieved = interfaceStats.BytesReceived.ToString();

            lblUploadSpeed.Text = bytesSentSpeed.ToString() + " kb/s";
            lblAgueUp.Text = bytesSentSpeed.ToString() + " kb/s";

            lbldownload.Text = bytesReceivedSpeed.ToString() + " kb/s";
            lblAgueDown.Text = bytesReceivedSpeed.ToString() + " kb/s";

            aGaugeDown.Value = bytesReceivedSpeed;
            aGaugeUp.Value = bytesSentSpeed;

            if (close && x > 0)
            {
                this.Size = new Size(339, 36);
                close = false;
            }


        }

                                                                                                    

        private void timer1_Tick(object sender, EventArgs e)
        {
            x++;
            System.Console.WriteLine(x);
           // initialize();

            checkAvailableConnection();
            
        }




        private void button2_Click_1(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (isMaximized == true)
            {
                this.Size = new Size(339, 36);
                button1.Text = "Max";
                isMaximized = false;
                //this.Location = new Point(this.Location.X + 120, this.Location.Y + 300);
            }
            else if (isMaximized == false)
            {
                this.Size = new Size(864, 352);
                button1.Text = "Min";
                isMaximized = true;
               // this.Location = new Point(this.Location.X - 120, this.Location.Y - 300);
            }

        }

         private void FromMoveWithouse(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void Form1_MouseDown_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            About obj = new About();
            obj.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void lblAgueUp_Click(object sender, EventArgs e)
        {

        }

        private void lblUploadSpeed_Click(object sender, EventArgs e)
        {

        }

        private void lblAgueDown_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
