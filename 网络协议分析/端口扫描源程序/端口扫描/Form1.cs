using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 端口扫描
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// 端口号
        /// </summary>
        private int port;
        /// <summary>
        /// IP地址
        /// </summary>
        private string IP;
        /// <summary>
        /// 数组
        /// </summary>
        private bool[] done = new bool[65536];
        /// <summary>
        /// 开始端口号
        /// </summary>
        private int start;
        /// <summary>
        /// 结束端口号
        /// </summary>
        private int end;
        /// <summary>
        /// 线程
        /// </summary>
        private Thread scanThread;
        /// <summary>
        /// 是否完成
        /// </summary>
        private bool isOK;
        /// <summary>
        /// 构造函数
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }
        /// <summary>
        /// 扫描线程
        /// </summary>
        private void PortScan()
        {
            start = Int32.Parse(txtStart.Text);
            end = Int32.Parse(txtEnd.Text);
            //检查输入范围合法性
            if ((start >= 0 && start <= 65536) && (end >= 0 && end <= 65536) && (start <= end))
            {
                lbResult.Items.Add("开始扫描... (可能需要请您等待几分钟)");

                IP = ipAddressTextBox1.Text;
                for (int i = start; i <= end; i++)
                {
                    port = i;
                    //使用该端口的扫描线程
                    scanThread = new Thread(new ThreadStart(Scan));
                    scanThread.Start();
                    //使线程睡眠
                    System.Threading.Thread.Sleep(100);
                    progressBar1.Value = i;
                    lblNow.Text = i.ToString();
                }
                //未完成时情况
                while (!isOK)
                {
                    isOK = true;
                    for (int i = start; i <= end; i++)
                    {
                        if (!done[i])
                        {
                            isOK = false;
                            break;
                        }
                    }
                    System.Threading.Thread.Sleep(1000);
                }
                lbResult.Items.Add("扫描结束!");
            }
            else
            {
                MessageBox.Show("输入错误，端口范围为[0-65536]");
            }
        }

        private void Scan()
        {
            int portnow = port;
            //创建线程变量
            Thread Threadnow = scanThread;
            done[portnow] = true;
            //创建TcpClient对象，TcpClient用于为TCP网络服务提供客户端连接
            TcpClient objTCP = null;
            //扫描端口，成功则写入信息
            try
            {
                //用TcpClient对象扫描端口
                objTCP = new TcpClient(IP, portnow);
                lbResult.Items.Add("端口 " + portnow.ToString() + " 开放!");
            }
            catch
            {
                //lbResult.Items.Add("端口 " + portnow.ToString() + " 未开放!");
            }
        }
        public Thread process;
        private void btnScan_Click_1(object sender, EventArgs e)
        {
            btnScan.Enabled = false;
            if (txtStart.Text.ToString() == "")
            {
                MessageBox.Show("IP地址不能为空");
                return;
            }
          
            //创建线程，并创建ThreadStart委托对象
            process = new Thread(new ThreadStart(PortScan));
            //线程开始
            process.Start();
            //显示端口扫描的范围
            progressBar1.Minimum = Int32.Parse(txtStart.Text);
            progressBar1.Maximum = Int32.Parse(txtEnd.Text);
            //显示框初始化
            lbResult.Items.Clear();
            lbResult.Items.Add("            端口扫描器           ");
            lbResult.Items.Add("---------------------------------"); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            process.Abort();
            btnScan.Enabled = true;
           // progressBar1.Value = 0;
             
        }
    }
}
