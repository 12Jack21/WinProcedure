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
            //添加事件到事件队列中
            this.FireEvent += new FireEventHandler(ExtinguishFire);
        }
        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowSender msgSender = new WindowSender();
            Window window = Application.Current.MainWindow;
            // 得到 Window
            //msgSender.hWnd = new WindowInteropHelper(window).Handle;
            msgSender.Show();
            // Console.WriteLine("当前进程 {0}", new WindowInteropHelper(this).Handle.ToInt32());
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            //Message message = (Message)this.MessageTab.Content;
            MsgReceived.Text = "";
        }


        /* ------------以下为事件调用机制-----------*/

        //将火情处理定义为FireEventHandler 代理(delegate) 类型，这个代理声明的事件的参数列表
        public delegate void FireEventHandler(object sender, FireEventArgs fe);
        //定义FireEvent 为FireEventHandler delegate 事件(event) 类型.
        public event FireEventHandler FireEvent;
        //激活事件的方法，创建了FireEventArgs 对象，发起事件，并将事件参数对象传递过去
        public void ActivateFireAlarm(string room, int ferocity)
        {
            FireEventArgs fireArgs = new FireEventArgs(room, ferocity);
            //执行对象事件处理函数指针，事件触发
            FireEvent(this, fireArgs);
        }
        
        // 灭火
        private void ExtinguishFire(object sender,FireEventArgs fe)
        {
            int ferocity = fe.ferocity; //得到火情
            string room = fe.room;
            if(ferocity <= 5)
            {
                string report = String.Format(@"群众在{0} 发现火情，级别为{1}，群众自行控制住了火势。", room, ferocity);
                fireText.AppendText(report + "\n");
            }
            else
            {
                string report = String.Format(@"群众在{0} 发现火情，级别为{1}，群众无法控制火势。消防官兵赶来灭火", room, ferocity);
                fireText.AppendText(report + "\n");
            }
        }
        //触发小的火情（群众自己灭火）
        private void fireBtn1_Click(object sender, RoutedEventArgs e)
        {
            fireText.AppendText("ActivateFireAlarm函数被调用（小火情）\n");
            Random random = new Random();
            ActivateFireAlarm("room_" + random.Next(0,100) ,random.Next(1, 6));
        }

        // 触发大的火情（消防员灭火）
        private void fireBtn2_Click(object sender, RoutedEventArgs e)
        {
            fireText.AppendText("ActivateFireAlarm函数被调用(大火情)\n");
            Random random = new Random();
            ActivateFireAlarm("room_" + random.Next(0, 100), random.Next(6, 11));
        }
    }

    public class FireEventArgs : EventArgs
    {
        public string room { get; set; }
        public int ferocity { get; set; }
        public FireEventArgs(string room,int ferocity)
        {
            this.room = room;
            this.ferocity = ferocity;
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
