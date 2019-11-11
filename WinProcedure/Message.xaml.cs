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
            OnSourceInitialized();
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
            this.MsgReceived.Text = "";
        }

        const int WM_COPYDATA = 0x004A;

        //protected override void OnSourceInitialized(EventArgs e)
        //{
        //    base.OnSourceInitialized(e);
        //    HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
        //    if (hwndSource != null)
        //    {
        //        IntPtr handle = hwndSource.Handle;
        //        hwndSource.AddHook(new HwndSourceHook(WndProc));
        //    }
        //}

        private void OnSourceInitialized()
        {
            //this.OnSourceInitialized(e);
            Window window = Window.GetWindow(this);
            var window1 = VisualTreeHelper.GetParent(this);
            Window window2 = (Window)this.Parent;
            Window window3 = Application.Current.MainWindow;

            HwndSource hwndSource = PresentationSource.FromVisual(window3) as HwndSource;
            //HwndSource hwndSource = PresentationSource.FromVisual(Window.GetWindow(this)) as HwndSource;

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
                    MsgReceived.AppendText(str + "\n");
                    handled = true;
                    break;
            }
            return hwnd;
        }

    }
}
