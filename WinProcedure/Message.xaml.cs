using System;
using System.Collections.Generic;
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
using static WinProcedure.CustomedAPI;

namespace WinProcedure
{
    /// <summary>
    /// Message.xaml 的交互逻辑
    /// </summary>
    public partial class Message : UserControl
    {
        public Message()
        {
            InitializeComponent();
        }
        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowSender msgSender = new WindowSender();
            Window window = Application.Current.MainWindow;
            // 得到 Window
            msgSender.hWnd = new WindowInteropHelper(window).Handle;
            msgSender.Show();
            // Console.WriteLine("当前进程 {0}", new WindowInteropHelper(this).Handle.ToInt32());
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            //Message message = (Message)this.MessageTab.Content;
            MsgReceived.Text = "";
        }
    }
    public partial class MainWindow
    {
        const int WM_COPYDATA = 0x004A;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                IntPtr handle = hwndSource.Handle;
                hwndSource.AddHook(new HwndSourceHook(WndProc));
            }
        }
        // 处理消息
        protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_COPYDATA:
                    COPYDATASTRUCT cds = (COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(COPYDATASTRUCT));
                    string str = cds.lpData;
                    Message message = (Message)this.MessageTab.Content;
                    message.MsgReceived.AppendText(str + "\n");
                    handled = true;
                    break;
            }
            return hwnd;
        }

    }
}
