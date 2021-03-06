﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;
using System.IO.Ports;

using System.IO;

namespace Serial_Port
{
    public partial class Form1 : Form
    {
        static SerialPort MySerial;
        string select_file = "";
        public Form1()
        {
            InitializeComponent();

        }
        //打开串口
        private void button1_Click(object sender, EventArgs e)
        {
            if (MySerial.IsOpen == true)
                MySerial.Close();
            
            MySerial.PortName = PortList.Text;
            MySerial.BaudRate = int.Parse(BountRate.Text);
            MySerial.Parity = Parity.None;
            string date_bit = ui_dataBit.Text;
            MySerial.DataBits = int.Parse(date_bit.ToUpperInvariant());
            MySerial.StopBits = StopBits.One;
            MySerial.Handshake = Handshake.None;
            Messageshow.AppendText("opening\r\n");

            try
            {
                MySerial.Open();
            }
            catch (InvalidOperationException)
            {
                MySerial.Close();
                Messageshow.AppendText("端口被占用\r\n");
                return;
            }
            catch (ArgumentOutOfRangeException)
            {
                Messageshow.AppendText("属性的值无效\r\n");
                return;
            }
            catch (ArgumentException)
            {
                Messageshow.AppendText("不支持该端口的文件类型\r\n");
                return;
            }catch(IOException)
            {
                Messageshow.AppendText("该端口处于无效状态\r\n");
                return;
                //该端口处于无效状态
            }
            catch (UnauthorizedAccessException)
            {
                //访问被拒绝的端口已打开
                Messageshow.AppendText("访问被拒绝的端口/已打开\r\n");
                return;
            }

            MySerial.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            Messageshow.AppendText("open finish\r\n");
            button_close.Enabled = true;
            button_send.Enabled = true;
        }
        //接收串口数据
        private  void DataReceivedHandler(object sender,SerialDataReceivedEventArgs e)
        {
            byte[] readedData;
            SerialPort sp = (SerialPort)sender;
            readedData = new byte[sp.BytesToRead];
            //string indata = sp.ReadExisting();
            sp.Read(readedData, 0, sp.BytesToRead);
            Messageshow.AppendText(BitConverter.ToString(readedData).Replace("-"," ")+"\r\n");
        }
        //
        private void Form1_Load(object sender, EventArgs e)
        {
            MySerial = new SerialPort();
            string[] port = SerialPort.GetPortNames();
            if (port == null)
            {
                MessageBox.Show("未找到串口!", "Error");
                return;
            }
            PortList.Items.Clear();
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            {
             //   Messageshow.AppendText(s + "\r\n");
                PortList.Items.Add(s);
            }
            PortList.SelectedIndex = 0;
            PortList.Text = PortList.Items[0].ToString();//PortList.GetItemText(PortList.SelectedIndex);
            //打开之前，失能发送、关闭按钮
            button_close.Enabled = false;
            button_send.Enabled = false;
            //增加按键处理
            this.KeyUp += new KeyEventHandler(this.Form1_KeyUp); 

        }
        //发送
        private void button1_Click_1(object sender, EventArgs e)
        {
           byte[] send_buffer;
            send_buffer = new byte[2048];
            Messageshow.AppendText("The CMD is:"+cmd_list.Text + "\r\n");
           
            string hexvalue = "";
            string temp =  cmd_list.Text.ToString();
            if(cmd_list.Text.Length %2 !=0)
            {

            }else
            {
                for(int i=0;i<cmd_list.Text.Length/2;i++)
                {
                    hexvalue += temp.Substring(i * 2, 2)+' ';
                   
                }
                string[] hexValuesSplit = hexvalue.Split(' ');

                 for (int j = 0; j < cmd_list.Text.Length / 2; j++)
                 {
                     send_buffer[j] = Convert.ToByte(hexValuesSplit[j],16);//Convert.ToInt32(hexValuesSplit[j], 16);// hexValuesSplit[j]
                 }
                 MySerial.Write(send_buffer, 0, cmd_list.Text.Length / 2);
            }

        }
        //清除
        private void button1_Click_2(object sender, EventArgs e)
        {
            Messageshow.Text = "";
        }
        //选择文件
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "txt文件(*.txt)|*.txt";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                select_file = fileDialog.FileName;
                Messageshow.AppendText("select file:" + select_file+"\r\n");
            }

            string line;
            cmd_list.Items.Clear();
            cmd_list.Text = "";
            try
            {
                System.IO.StreamReader cmd_file = new System.IO.StreamReader(select_file);
                while ((line = cmd_file.ReadLine()) != null)
                {
                    cmd_list.Items.Add(line);
                }
                cmd_list.SelectedIndex = 0;
                cmd_list.Text = cmd_list.Items[0].ToString();//cmd_list.GetItemText(cmd_list.SelectedIndex);
            }
            catch (IOException error)
            {
                Messageshow.AppendText(error.ToString() + "\r\n");
            }
        }
        //关闭串口
        private void button3_Click(object sender, EventArgs e)
        {
            if(MySerial.IsOpen == true)
                MySerial.Close();
            button_close.Enabled = false;
            button_send.Enabled = false;
        }
        //键盘处理函数
        //F5 跟新串口
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
         //   MessageBox.Show(e.KeyCode.ToString(), "您所按动的健为：");
            //e.KeyCode.GetTypeCode
            if (e.KeyCode == Keys.F5)
            {
                Messageshow.AppendText("F5 press\r\n");
                string[] port = SerialPort.GetPortNames();
                if (port == null)
                {
                    MessageBox.Show("未找到串口!", "Error");
                    return;
                }
                PortList.Items.Clear();
                foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
                {
                    //   Messageshow.AppendText(s + "\r\n");
                    PortList.Items.Add(s);
                }
                PortList.SelectedIndex = 0;
                PortList.Text = PortList.Items[0].ToString();//PortList.GetItemText(PortList.SelectedIndex);
            }
        }
    }
}
