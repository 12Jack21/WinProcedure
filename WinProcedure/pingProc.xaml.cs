using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WinProcedure
{
    /// <summary>
    /// pingProc.xaml 的交互逻辑
    /// </summary>
    public partial class pingProc : UserControl
    {
        public pingProc()
        {
            InitializeComponent();
        }

        private void SyncPingBtn_Click(object sender, RoutedEventArgs e)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            //是否使用外壳程序
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            //重定向IO
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;

            //使Ping命令执行 5 次
            string strCmd = "ping www.163.com -n 9";
            process.Start();
            process.StandardInput.WriteLine(strCmd); //输入命令
            process.StandardInput.WriteLine("exit"); //上一条命令执行完之后才会执行该条

            //获取输出信息
            string output = process.StandardOutput.ReadToEnd();
            outputText.Text = output;
            process.WaitForExit();
            process.Close();
        }

        private static StringBuilder cmdOutput;

        //异步调用ping
        private void AsyncPingBtn_Click(object sender, RoutedEventArgs e)
        {
            Process cmdP = new Process();
            cmdP.StartInfo.FileName = "cmd.exe";
            cmdP.StartInfo.UseShellExecute = false;
            cmdP.StartInfo.CreateNoWindow = true;

            cmdP.StartInfo.RedirectStandardInput = true;
            cmdP.StartInfo.RedirectStandardOutput = true;

            cmdOutput = new StringBuilder("");

            //输出数据异步处理事件
            cmdP.OutputDataReceived += new DataReceivedEventHandler(strOutputHandler);

            //异步处理中通知您的应用程序某个进程已退出
            cmdP.EnableRaisingEvents = true;
            cmdP.Start();

            string strCmd = "ping www.baidu.com -n 9";
            cmdP.StandardInput.WriteLine(strCmd);

            ////重定向进程输入流，其他方法将向此流中写入数据
            //StreamWriter streamInput = cmdP.StandardInput;

            //开始异步输出的 数据读入
            cmdP.BeginOutputReadLine();

            cmdP.StandardInput.WriteLine("exit");

        }

        //自定义窗体消息(消息类型，在发送方和接收方都需要定义)
        public const int TRAN_FINISHED = 0x500;
        public const int WM_COPYDATA = 0x004A;

        //主窗体和文本控件的 句柄值
        public static IntPtr main_whandle;
        public static IntPtr text_whandle;

        #region 定义结构体
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            //marshal用于托管与非托管数据的交换
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }
        #endregion


        //钩子函数
        //当控制台进程的数据输出时将执行回调函数，outLine参数携带产生的数据传入到函数中处理
        private void strOutputHandler_winform(object sender,DataReceivedEventArgs outLine)
        {
            //将每次产生的数据附加到结果字符串中
            cmdOutput.AppendLine(outLine.Data);
            //设置输出文本框的内容
            SendMessage_winform(main_whandle, TRAN_FINISHED, 0, 0);
        }

        private void strOutputHandler(object sender,DataReceivedEventArgs outLine)
        {
            //将每次产生的数据附加到结果字符串中
            cmdOutput.AppendLine(outLine.Data);

            //outputText.Text += outLine.Data; 线程无法 操控控件

            //通过FindWindow API的方式找到目标进程句柄，然后发送消息
            IntPtr WINDOW_HANDLER = FindWindow(null, "demo");

            // TODO:未进入这个 Block 里，发送消息的地方存在问题------------------------------------------------------------
            if(WINDOW_HANDLER != IntPtr.Zero)
            {
                COPYDATASTRUCT mystr = new COPYDATASTRUCT();
                mystr.dwData = (IntPtr)0;
                //另一种方法 MyStringUtil.isEmpty()   ?????
                if(outLine.Data == null || outLine.Data.Trim() == "")
                {
                    mystr.cbData = 0;
                    mystr.lpData = "";
                }
                else
                {
                    byte[] sarr = System.Text.Encoding.Unicode.GetBytes(outLine.Data);
                    mystr.cbData = sarr.Length + 1;
                    mystr.lpData = outLine.Data;
                }
                SendMessage(WINDOW_HANDLER, WM_COPYDATA, (IntPtr)0, ref mystr); //发送消息（带着cmd的输出数据）给窗体
            }
        }

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage_winform(
            IntPtr hWnd, //目标窗体句柄值 (接收消息的显示窗体)
            int Msg,     //消息值
            int wParam,  //第一个消息参数
            int lParam   //第二个消息参数
           );

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(
            IntPtr hWnd, //目标窗体句柄值 (接收消息的显示窗体)
            int Msg,     //消息值
            IntPtr wParam,  //第一个消息参数
            ref COPYDATASTRUCT lParam   //第二个消息参数
           );


        //窗体（或控件）加载时，获取窗体和文本框对象的 Handle值
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //winform的方法
            //HwndSource hs = (HwndSource)PresentationSource.FromDependencyObject(this);
            //main_whandle = hs.Handle;
            //hs = (HwndSource)PresentationSource.FromDependencyObject(outputText);
            //text_whandle = hs.Handle;

            //ppt的方法-wpf
            HwndSource hWndSource;
            //得到该控件的父级窗体
            Window window = Window.GetWindow(this);
            WindowInteropHelper wih = new WindowInteropHelper(window);
            hWndSource = HwndSource.FromHwnd(wih.Handle);
            //指定由MainWindowProc处理消息
            hWndSource.AddHook(MainWindowProc);

        }

        //钩子函数，处理所收到的消息
        private IntPtr MainWindowProc(IntPtr hwnd, int msg,
         IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_COPYDATA:
                    //把内存指针处的数据结构化
                    COPYDATASTRUCT copyDataStruct = (COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(COPYDATASTRUCT));

                    showComment(copyDataStruct.lpData);

                    break;
                default:
                    break;
            }
            return hwnd;
        }

        //TODO: 将输出显示出来 , MainWindowProc函数并没有收到 消息类型为 WM_COPYDATA的 Msg ？？？-----------------
        private void showComment(string data)
        {
            outputText.Text += data;
        }

    }
}
