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
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace 串口助手
{
    public partial class Form1 : Form
    {
        //Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。
                                    //StringBuilder对象是动态对象，在需要对字符串执行重复修改的情况下
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); 
        public int isclose;                         
        public Form1()                                      
        {
            CheckForIllegalCrossThreadCalls = false;
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

        private void boxenter()//将数据添加到下拉列表
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
            comboBox1.SelectedText = "COM3";
            comboBox2.SelectedText = "115200";
            isclose = 1;
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
                    socketsend(data);
                    textBox1.AppendText("s" + count.ToString() + "个字符: " + builder.ToString() + "\n");
                }));
            }
        }
        void socketsend(byte[] data) 
        {                // 判断是否连接
            if (checksocket(socket))
            {
                if (checkBox1.Checked)
                {
                    //客户端给服务器发消息
                    if (isclose != 1)
                    {
                        try
                        {
//                            byte[] buffer = Encoding.UTF8.GetBytes(textBox2.Text);
                            socket.Send(data);
                            ShowMsg(builder.ToString());
                        }
                        catch (Exception ex)
                        {
                            ShowMsg(ex.Message);
                        }
                    }
                }
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
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);//订阅委托
        }

        void serialwrite()
        {
            if (serialPort1.IsOpen)
            {
                if (radioButton3.Checked)//ASCII码直接发送
                {
                    string serialStringTemp = this.textBox2.Text;
                    this.serialPort1.WriteLine(serialStringTemp);
                }
                else if (radioButton4.Checked)
                {
                    byte[] BSendTemp = System.Text.Encoding.UTF8.GetBytes(textBox2.Text); //string转字节存入数组
                    serialPort1.Write(BSendTemp, 0, BSendTemp.Length);//发送数据  
                }
            }
            else
            {
                byte[] BSendTemp = System.Text.Encoding.UTF8.GetBytes(textBox2.Text); //string转字节存入数组
                socketsend(BSendTemp);  
            } 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                MessageBox.Show("发送数据为空！");
                return;
            }
            serialwrite();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (isclose==1)//如果没连接
//          if (!client.Connected)//如果没连接
            {
                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //连接到的目标IP
                IPAddress ip = IPAddress.Parse(textBox3.Text);
                //IPAddress ip = IPAddress.Any;
                //连接到目标IP的哪个应用(端口号！)
                IPEndPoint point = new IPEndPoint(ip, int.Parse(textBox4.Text));
                try
                {
                    if (!client.Connected)//如果之前没连接
                    {
                        client.Connect(point);//连接到服务器
                        ShowMsg("连接成功");
                        ShowMsg("服务器" + client.RemoteEndPoint.ToString());
                        ShowMsg("客户端:" + client.LocalEndPoint.ToString());
                        //连接成功后，就可以接收服务器发送的信息了
                        socketreceive(client);
                        isclose = 0;
                        socket = client;
                        //byte[] buffer = new byte[1024 * 1024];
                        //int n = client.Receive(buffer);
                        //serialPort1.Write(buffer, 0, n);//发送数据 
                    }
                    else if (client.Connected)//如果之前已连接
                    {
                        client.Connect(point);//连接到服务器
                    }
                    button3.Text = "关闭web";
                }
                catch (Exception ex)
                {
                    ShowMsg(ex.Message);
                } 
            }
            else
            {//判断Socket是否存在且连接正常，存在且连接正常的Socket才运行进行断开操作   
                if (socket != null && socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    Thread.Sleep(10);
                    socket.Close();
//                    client.Dispose();
//                    client = null;
                    button3.Text = "连接web";
                    isclose = 1;
                }
            }
        }
        void socketreceive(Socket client)
        {
            if (checkBox2.Checked)
            {
                try
                {
                    Thread th = new Thread(new ThreadStart(()=>ThreadMethod(client)));
                    // Thread th = new Thread(ReceiveMsg);
                   // Thread th = new Thread(ThreadMethod);
                    th.IsBackground = true;
                    th.Start();
                }
                catch (Exception ex)
                {
                    ShowMsg(ex.Message);
                }
            }
        }
        
        /// <summary>  
        /// 多线程执行指定方法  
        /// </summary>  
        //private void ThreadMethod( client)
        private void ThreadMethod(Socket client)
        {
            while (true)
            {
                Thread.Sleep(100);  //线程暂停100毫秒  
                ReceivesocketMsg(client);
                if (!checksocket(client))
                {
                    break;
                }
            }
            Thread.CurrentThread.Abort();
        }

        Boolean checksocket(Socket client)//判断socket连接后是否断开，不可判断第一次
        {
            byte[] data= new byte[1];
            try
            {
                if (client.Poll(1, SelectMode.SelectRead))//Poll 将在指定的时段（以 microseconds 为单位）内阻止执行。
                                                          //如果希望无限期的等待响应，则将 microSeconds 设置为一个负整数。
                {
                    int nRead = client.Receive(data);
                    if (nRead == 0)
                    {
                        //socket连接已断开
                        return false;
                    }
                    else return true;
                }
                else return true;
                //return !client.Poll(1, SelectMode.SelectRead) && (client.Available == 0);
            }
            catch (Exception)
            { return false; }
        }
    //接收服务器的消息

    void ReceivesocketMsg(Socket client)
        {
            try
            {
                byte[] buffer = new byte[1024 * 1024];
                int n = client.Receive(buffer);
//                 string s = Encoding.UTF8.GetString(buffer, 0, n);
//                  ShowMsg(client.RemoteEndPoint.ToString() + ":" + s);
                serialPort1.Write(buffer, 0, n);//发送数据  
            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message);
            }
        }

        void ShowMsg(string msg)
        {
            textBox1.AppendText("w" + msg + "\n");
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
