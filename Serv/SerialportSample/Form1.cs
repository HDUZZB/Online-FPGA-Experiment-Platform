using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Net;  // IP，IPAddress, IPEndPoint，端口等；
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace SerialportSample
{
    public partial class SerialportSampleForm : Form
    {
        private SerialPort comm = new SerialPort();
        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。
        private long received_count = 0;//接收计数
        private long send_count = 0;//发送计数
        private string str_clnt = null;
        private byte send_data=0;

        public SerialportSampleForm()
        {
            InitializeComponent();
            TextBox.CheckForIllegalCrossThreadCalls = false;
        }
        Thread threadWatch = null; // 负责监听客户端连接请求的 线程；
        Socket socketWatch = null;

        Dictionary<string, Socket> dict = new Dictionary<string, Socket>();
        Dictionary<string, Thread> dictThread = new Dictionary<string, Thread>();
        //窗体初始化
        private void Form1_Load(object sender, EventArgs e)
        {
            //初始化下拉串口名称列表框
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            comboPortName.Items.AddRange(ports);
            comboPortName.SelectedIndex = comboPortName.Items.Count > 0 ? 0 : -1;
            comboBaudrate.SelectedIndex = comboBaudrate.Items.IndexOf("9600");

            //初始化SerialPort对象
            comm.NewLine = "\r\n";
            comm.RtsEnable = true;//根据实际情况吧。

            //添加事件注册
            comm.DataReceived += comm_DataReceived;
        }

        void comm_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = comm.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
            byte[] buf = new byte[n];//声明一个临时数组存储当前来的串口数据
            string str_tmp;
            received_count += n;//增加接收计数
            comm.Read(buf, 0, n);//读取缓冲数据
            builder.Clear();//清除字符串构造器的内容
            //因为要访问ui资源，所以需要使用invoke方式同步ui。
            this.Invoke((EventHandler)(delegate
            {
                //判断是否是显示为16禁止
                
                    //依次的拼接出16进制字符串
                    foreach (byte b in buf)
                    {
                        builder.Append(b.ToString("X2") + " ");
                    }
                
                //追加的形式添加到文本框末端，并滚动到最后。
                //this.txGet.AppendText(builder.ToString());
                str_tmp = builder.ToString();
                label4.BackColor = System.Drawing.Color.Gray;
                label5.BackColor = System.Drawing.Color.Gray;
                label6.BackColor = System.Drawing.Color.Gray;
                label7.BackColor = System.Drawing.Color.Gray;
                if (str_tmp == "FE ")
                {
                    
                    label4.BackColor = System.Drawing.Color.Red;
                }
                else if (str_tmp == "FD ")
                {
                    
                    label5.BackColor = System.Drawing.Color.Red;

                }
                else if (str_tmp == "FB ")
                {
                    
                    label6.BackColor = System.Drawing.Color.Red;
                }
                else if (str_tmp == "F7 ")
                {
                    
                    label7.BackColor = System.Drawing.Color.Red;
                }
                //修改接收计数
              //  labelGetCount.Text = "Get:" + received_count.ToString();
            }));
            //string strMsg = "服务器" + "\r\n" + "   -->" + builder.ToString() + "\r\n";
            string strMsg =builder.ToString();
            byte[] arrMsg = System.Text.Encoding.UTF8.GetBytes(strMsg); // 将要发送的字符串转换成Utf-8字节数组；
            byte[] arrSendMsg = new byte[arrMsg.Length + 1];
            arrSendMsg[0] = 0; // 表示发送的是消息数据
            Buffer.BlockCopy(arrMsg, 0, arrSendMsg, 1, arrMsg.Length);
            string strKey = "";
            strKey = str_clnt;
            if (send_data == 1) 
            { 
                if (string.IsNullOrEmpty(strKey))   // 判断是不是选择了发送的对象；
                {
                 MessageBox.Show("请选择你要发送的好友！！！");
                }
                else
                {
                 dict[strKey].Send(arrSendMsg);// 解决了 sokConnection是局部变量，不能再本函数中引用的问题；

                }
            }
            
           
        }

        private void buttonOpenClose_Click(object sender, EventArgs e)
        {
            //根据当前串口对象，来判断操作
            if (comm.IsOpen)
            {
                //打开时点击，则关闭串口
                comm.Close();
            }
            else
            {
                //关闭时点击，则设置好端口，波特率后打开
                comm.PortName = comboPortName.Text;
                comm.BaudRate = int.Parse(comboBaudrate.Text);
                try
                {
                    comm.Open();
                }
                catch(Exception ex)
                {
                    //捕获到异常信息，创建一个新的comm对象，之前的不能用了。
                    comm = new SerialPort();
                    //现实异常信息给客户。
                    MessageBox.Show(ex.Message);
                }
            }
            //设置按钮的状态
            buttonOpenClose.Text = comm.IsOpen ? "Close" : "Open";
        //    buttonSend.Enabled = comm.IsOpen;
        }

        //动态的修改获取文本框是否支持自动换行。
       

   /*     private void buttonSend_Click(object sender, EventArgs e)
        {
            //定义一个变量，记录发送了几个字节
            int n = 0;
            //16进制发送
            if (checkBoxHexSend.Checked)
            {
                //我们不管规则了。如果写错了一些，我们允许的，只用正则得到有效的十六进制数
                MatchCollection mc = Regex.Matches(txSend.Text, @"(?i)[\da-f]{2}");
                List<byte> buf = new List<byte>();//填充到这个临时列表中
                //依次添加到列表中
                foreach (Match m in mc)
                {
                    buf.Add(byte.Parse(m.Value));
                }
                //转换列表为数组后发送
                comm.Write(buf.ToArray(), 0, buf.Count);
                //记录发送的字节数
                n = buf.Count;
            }
            else//ascii编码直接发送
            {
                //包含换行符
                if (checkBoxNewlineSend.Checked)
                {
                    comm.WriteLine(txSend.Text);
                    n = txSend.Text.Length + 2;
                }
                else//不包含换行符
                {
                    comm.Write(txSend.Text);
                    n = txSend.Text.Length;
                }
            }
            send_count += n;//累加发送字节数
            labelSendCount.Text = "Send:" + send_count.ToString();//更新界面
        }
*/
       
        void WatchConnecting()
        {
            while (true)  // 持续不断的监听客户端的连接请求；
            {
                // 开始监听客户端连接请求，Accept方法会阻断当前的线程；
                Socket sokConnection = socketWatch.Accept(); // 一旦监听到一个客户端的请求，就返回一个与该客户端通信的 套接字；
                // 想列表控件中添加客户端的IP信息；
                str_clnt = sokConnection.RemoteEndPoint.ToString();
                lbOnline.Items.Add(sokConnection.RemoteEndPoint.ToString());
                // 将与客户端连接的 套接字 对象添加到集合中；
                dict.Add(sokConnection.RemoteEndPoint.ToString(), sokConnection);
                ShowMsg("客户端连接成功！");
                Thread thr = new Thread(RecMsg);
                thr.IsBackground = true;
                thr.Start(sokConnection);
                dictThread.Add(sokConnection.RemoteEndPoint.ToString(), thr);  //  将新建的线程 添加 到线程的集合中去。
            }
        }

        void RecMsg(object sokConnectionparn)
        {
            Socket sokClient = sokConnectionparn as Socket;
            Process proc = null;
            while (true)
            {
                // 定义一个2M的缓存区；
                byte[] arrMsgRec = new byte[1024 * 1024 * 2];
                // 将接受到的数据存入到输入  arrMsgRec中；
                int length = -1;
                try
                {
                    length = sokClient.Receive(arrMsgRec); // 接收数据，并返回数据的长度；
                }
                catch (SocketException se)
                {
                    ShowMsg("异常：" + se.Message);
                    // 从 通信套接字 集合中删除被中断连接的通信套接字；
                    dict.Remove(sokClient.RemoteEndPoint.ToString());
                    // 从通信线程集合中删除被中断连接的通信线程对象；
                    dictThread.Remove(sokClient.RemoteEndPoint.ToString());
                    // 从列表中移除被中断的连接IP
                    lbOnline.Items.Remove(sokClient.RemoteEndPoint.ToString());
                    break;
                }
                catch (Exception e)
                {
                    ShowMsg("异常：" + e.Message);
                    // 从 通信套接字 集合中删除被中断连接的通信套接字；
                    dict.Remove(sokClient.RemoteEndPoint.ToString());
                    // 从通信线程集合中删除被中断连接的通信线程对象；
                    dictThread.Remove(sokClient.RemoteEndPoint.ToString());
                    // 从列表中移除被中断的连接IP
                    lbOnline.Items.Remove(sokClient.RemoteEndPoint.ToString());
                    break;
                }
                if (arrMsgRec[0] == 0)  // 表示接收到的是数据；
                {
                    //string strMsg = System.Text.Encoding.UTF8.GetString(arrMsgRec, 1, length - 1);// 将接受到的字节数据转化成字符串；
                    //ShowMsg(strMsg);
                    send_data = 1;

                }
                if (arrMsgRec[0] == 2)
                {
                    try
                    {
                        proc = new Process();
                        proc.StartInfo.FileName = @"D:\sss.bat";
                        proc.StartInfo.Arguments = string.Format("10");//this is argument
                        proc.StartInfo.CreateNoWindow = false;
                        proc.Start();
                        proc.WaitForExit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
                    }
                }
                if (arrMsgRec[0] == 1) // 表示接收到的是文件；
                {
                    //   SaveFileDialog sfd = new SaveFileDialog();

                    //  if (sfd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    {// 在上边的 sfd.ShowDialog（） 的括号里边一定要加上 this 否则就不会弹出 另存为 的对话框，而弹出的是本类的其他窗口，，这个一定要注意！！！【解释：加了this的sfd.ShowDialog(this)，“另存为”窗口的指针才能被SaveFileDialog的对象调用，若不加thisSaveFileDialog 的对象调用的是本类的其他窗口了，当然不弹出“另存为”窗口。】

                        //    string fileSavePath = sfd.FileName;// 获得文件保存的路径；
                        //     FileInfo finfo = new FileInfo("D:\\www.sof");
                        //     finfo.Create();
                        // 创建文件流，然后根据路径创建文件；
                        send_data = 0;
                        string fileSavePath = "D:\\www.sof";
                        using (FileStream fs = new FileStream(fileSavePath, FileMode.Create))
                        {
                            fs.Write(arrMsgRec, 1, length - 1);
                            ShowMsg("文件保存成功：" + fileSavePath);
                        }


                    }
                }

            }

        }

        void ShowMsg(string str)
        {
            txGet.AppendText(str + "\r\n");
        }

        private void btnBeginListen_Click(object sender, EventArgs e)
        {
            // 创建负责监听的套接字，注意其中的参数；
            socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // 获得文本框中的IP对象；
            IPAddress address = IPAddress.Parse(txtIp.Text.Trim());
            // 创建包含ip和端口号的网络节点对象；
            IPEndPoint endPoint = new IPEndPoint(address, int.Parse(txtPort.Text.Trim()));
            try
            {
                // 将负责监听的套接字绑定到唯一的ip和端口上；
                socketWatch.Bind(endPoint);
            }
            catch (SocketException se)
            {
                MessageBox.Show("异常：" + se.Message);
                return;
            }
            // 设置监听队列的长度；
            socketWatch.Listen(10);
            // 创建负责监听的线程；
            threadWatch = new Thread(WatchConnecting);
            threadWatch.IsBackground = true;
            threadWatch.Start();
            ShowMsg("服务器启动监听成功！");
            //}
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
