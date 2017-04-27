using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 串口助手
{
    public partial class Form1 : Form
    {
        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。
        public Form1()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void boxenter()
        {

            string str, str1;
            int ii = 150;
            for (int i = 1; i < 9; i++)
            {
                ii = ii * 2;
                str1 = ii.ToString();
                comboBox2.Items.Add(str1);
            }
            comboBox2.Items.Add(43000);
            comboBox2.Items.Add(56000);
            comboBox2.Items.Add(57600);
            comboBox2.Items.Add(115200);
            for (int i = 0; i < 20; i++)
            {
                str = i.ToString();
                str = "COM" + str;
                comboBox1.Items.Add(str);
            }
            comboBox3.Items.Add("None");
            comboBox3.Items.Add("Odd");
            comboBox3.Items.Add("Event");
            comboBox3.Items.Add("Mark");
            comboBox3.Items.Add("Space");
            comboBox4.Items.Add(8);
            comboBox4.Items.Add(7);
            comboBox4.Items.Add(6);
            comboBox5.Items.Add("None");
            comboBox5.Items.Add("One");
            comboBox5.Items.Add("OnePointFive");
            comboBox5.Items.Add("Two");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            boxenter();
        }
        private void serialPort1_DataReceived(object sender, EventArgs e)
        {
            while (serialPort1.BytesToRead != 0)
            {
                System.Threading.Thread.Sleep(100);
                /*延时为了让所有数据到达,不延时会有数据丢失
                如果数据太长延时100ms也是不够的，最好把接
                收的数据存入缓存区中,校检数据完整后再发出*/
                int count = serialPort1.BytesToRead;
                byte[] data = new byte[count];
                serialPort1.Read(data, 0, data.Length);
                builder.Clear();//清除字符串构造器的内容
                //因为要访问ui资源，所以需要使用invoke方式同步ui。
                this.Invoke((EventHandler)(delegate
                {
                    //判断是否是显示为16进制
                    if (radioButton2.Checked)
                    {
                        //依次的拼接出16进制字符串
                        foreach (byte b in data)
                        {
                            builder.Append(b.ToString("X2") + " ");
                        }
                    }
                    else if(radioButton1.Checked)
                    {
                        //直接按UTF8规则转换成字符串，ASCII码转中文乱码，UTF8不会
                        builder.Append(Encoding.UTF8.GetString(data));
                    }
                    //追加的形式添加到文本框末端，并滚动到最后。
                    //textBox2.AppendText(System.DateTime.Now.ToString() + ": " + builder.ToString() +  "\n") ;
                    textBox1.AppendText(count.ToString() + "个字符: " + builder.ToString() + "\n");
                }));
            }
        }

        private void boxwrite()
        {
            serialPort1.PortName = comboBox1.Text;
            serialPort1.BaudRate = int.Parse(comboBox2.Text);

            //serialPort1.Parity = (System.IO.Ports)comboBox3.Text;//校验位
            if(comboBox3.Text== "Even")
                serialPort1.Parity = Parity.Even;
            if (comboBox3.Text == "Mark")
                serialPort1.Parity = Parity.Mark;
            if (comboBox3.Text == "None")
                serialPort1.Parity = Parity.None;
            if (comboBox3.Text == "Odd")
                serialPort1.Parity = Parity.Odd;
            if (comboBox3.Text == "Space")
                serialPort1.Parity = Parity.Space;

            if(comboBox4.Text!="")
                serialPort1.DataBits = int.Parse(comboBox4.Text);//数据位

            //serialPort1.StopBits = StopBits.None;//停止位
            if (comboBox5.Text == "None")
                serialPort1.StopBits = StopBits.None;
            if (comboBox5.Text == "One")
                serialPort1.StopBits = StopBits.One;
            if (comboBox5.Text == "OnePointFive")
                serialPort1.StopBits = StopBits.OnePointFive;
            if (comboBox5.Text == "Two")
                serialPort1.StopBits = StopBits.Two;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                try
                {
                    boxwrite();
                    serialPort1.Open();
                    button2.Text = "关闭串口";
                    serialPort1.DataReceived += serialPort1_DataReceived;
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.ToString());
                }
            }
            else if (serialPort1.IsOpen)
            {
                button2.Text = "打开串口";
                serialPort1.Close();
            }
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                MessageBox.Show("发送数据为空！");
                return;
            }
            if (radioButton3.Checked)//ASCII码直接发送
            {
                string serialStringTemp = this.textBox2.Text;
                this.serialPort1.WriteLine(serialStringTemp);
            }
            else if (radioButton4.Checked)
            {
                byte[] BSendTemp = System.Text.Encoding.Default.GetBytes(textBox2.Text); //string转字节存入数组
                serialPort1.Write(BSendTemp, 0, BSendTemp.Length);//发送数据    
            }

        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
