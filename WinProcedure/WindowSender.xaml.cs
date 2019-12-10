using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static WinProcedure.CustomedAPI;

namespace WinProcedure
{
    /// <summary>
    /// WindowSender.xaml 的交互逻辑
    /// </summary>
    public partial class WindowSender : Window
    {
        public WindowSender()
        {
            InitializeComponent();
            FindWndHandlerByName("WinProcedure");
        }


        // 用户文本消息
        const int WM_COPYDATA = 0x004A;
        public IntPtr hWnd; // 要发送消息的目标窗体句柄

        [DllImport("user32.dll")]
        public static extern void SendMessage(
            IntPtr hWnd,
            int msg,
            IntPtr wParam, //应该是具体的控件句柄
            ref COPYDATASTRUCT lParam);

        // 通过进程名找到窗口句柄
        private void FindWndHandlerByName(string name)
        {
            // 遍历所有进程
            Process[] procs = Process.GetProcesses();
            foreach (Process p in procs)
            {
                if (p.ProcessName.Equals(name))
                {
                    // 获取目标进程句柄
                    hWnd = p.MainWindowHandle;
                }
            }
        }
        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            string text = msg.Text;
            if (hWnd != null)
            {
                COPYDATASTRUCT cds;
                cds.lpData = text;
                cds.dwData = (IntPtr)100;
                byte[] arr = Encoding.UTF8.GetBytes(text);
                Console.WriteLine("向进程{1}发送{0}\n", text, hWnd.ToInt32());
                cds.cbData = arr.Length + 1; // 发送的字节数
                // 同步发送消息
                // 异步发送存在字符串指针空间回收问题
                SendMessage(hWnd, WM_COPYDATA, IntPtr.Zero, ref cds);
            }
            msg.Text = "";
        }
    }
}
