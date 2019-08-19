using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Data.SQLite;
using System.Threading;
namespace arduinoserial
{
    public partial class Form1 : Form
    {
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter DB;
        private int GridlinesOffset = 0;
        private int kanan;
        private int kiri;
        private int datab;
        
        public Form1()
        {
            InitializeComponent();
            timer1.Enabled = false;
            chart1.ChartAreas[0].AxisY.Maximum = (int)numericUpDown1.Value;
            chart1.ChartAreas[0].AxisY.Minimum = 200;
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum = 60;
            for (int i = 0; i < 60; i++)
            {
                chart1.Series["Suhu"].Points.AddY(0);
            }
        }

        private void Chart1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            portConfig.Items.AddRange(ports);
            
            CreateConnection();
        }
        private void CreateConnection()
        {
            sql_con = new SQLiteConnection(@"Data Source=D:\Project C# or VB\arduinoserial\arduinoserial\databaru.db;Version=3;New=False;Compress=True;");
            try
            {
                sql_con.Open();
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = portConfig.Text;
                serialPort1.BaudRate = Convert.ToInt32(comboBaudrate.Text);
                serialPort1.DataBits = Convert.ToInt32(comboDatabits.Text);
                serialPort1.StopBits = (StopBits)Enum.Parse(typeof(StopBits), comboStopbits.Text);
                serialPort1.Parity = (Parity)Enum.Parse(typeof(Parity), comboParity.Text);

                serialPort1.Open();
                if (!backgroundWorker1.IsBusy)
                {
                    backgroundWorker1.RunWorkerAsync();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message,"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                timer1.Enabled = false;
                serialPort1.Close();
               
            }
        }
        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 100;
            String dataku = serialPort1.ReadLine().ToString();
            kanan = Convert.ToInt32(dataku) - 40;
            kiri = Convert.ToInt32(dataku) + 40;
            if (datab < kanan)
            {
                label8.Text = "Kanan";
            }
            if (datab > kiri)
            {
                label8.Text = "Kiri";
            }
            //label8.Text = kanan.ToString();
            label6.Text = dataku;
            chart1.Series["Suhu"].Points.AddY(dataku);
            chart1.Series["Suhu"].Points.RemoveAt(0);
            chart1.ChartAreas[0].AxisX.MajorGrid.IntervalOffset = -GridlinesOffset;

            //Calculate Next Offset
            GridlinesOffset++;
            GridlinesOffset %= (int)chart1.ChartAreas[0].AxisX.MajorGrid.Interval;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sql_con.CreateCommand();
            sqlite_cmd.CommandText = "INSERT INTO baca (tanggal,baca) VALUES ('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + dataku + "')";
            //sqlite_cmd.ExecuteNonQuery();
            datab = Convert.ToInt32(dataku);
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisY.Maximum = (int)numericUpDown1.Value;
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                Thread.Sleep(100);
                String dataku = serialPort1.ReadLine().ToString();
                kanan = Convert.ToInt32(dataku) - 40;
                kiri = Convert.ToInt32(dataku) + 40;
                backgroundWorker1.ReportProgress(Convert.ToInt32(dataku));
            }
            
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int dataku = e.ProgressPercentage;
            kanan = dataku - 40;
            kiri = dataku + 40;
            if (datab < kanan)
            {
                label8.Text = "Kanan";
            }
            if (datab > kiri)
            {
                label8.Text = "Kiri";
            }
            //label8.Text = kanan.ToString();
            label6.Text = dataku.ToString();
            chart1.Series["Suhu"].Points.AddY(dataku);
            chart1.Series["Suhu"].Points.RemoveAt(0);
            chart1.ChartAreas[0].AxisX.MajorGrid.IntervalOffset = -GridlinesOffset;

            //Calculate Next Offset
            GridlinesOffset++;
            GridlinesOffset %= (int)chart1.ChartAreas[0].AxisX.MajorGrid.Interval;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sql_con.CreateCommand();
            sqlite_cmd.CommandText = "INSERT INTO baca (tanggal,baca) VALUES ('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + dataku + "')";
            //sqlite_cmd.ExecuteNonQuery();
            datab = dataku;
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
    }
}
