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
namespace arduinoserial
{
    public partial class Form1 : Form
    {
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter DB;
        
        public Form1()
        {
            InitializeComponent();
            timer1.Enabled = false;
            
        }

        private void Chart1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            portConfig.Items.AddRange(ports);
            chart1.ChartAreas[0].AxisY.Maximum = 400;
            chart1.ChartAreas[0].AxisY.Minimum = 200;
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
                timer1.Enabled = true;
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
                label6.Text = "";
            }
        }
        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 100;
            String dataku = serialPort1.ReadLine().ToString();
            label6.Text = dataku;
            chart1.Series["Suhu"].Points.AddY(dataku);
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sql_con.CreateCommand();
            sqlite_cmd.CommandText = "INSERT INTO baca (tanggal,baca) VALUES ('"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + dataku + "')";
            //sqlite_cmd.ExecuteNonQuery();
        }
    }
}
