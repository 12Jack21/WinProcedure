using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WinProcedure
{
    /// <summary>
    /// Ping.xaml 的交互逻辑
    /// </summary>
    public partial class Ping : Page
    {
        public Ping()
        {
            InitializeComponent();
        }

        private void AsyncPingBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GetMacBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SyncPingBtn_Click(object sender, RoutedEventArgs e)
        {

        }



        private void current_Click(object sender, RoutedEventArgs e)
        {
            outputText.Items.Clear();
            Thread currentThread = Thread.CurrentThread;
            outputText.Items.Add("Thread Name :" + currentThread.Name);
            outputText.Items.Add("Thread State :" + currentThread.ThreadState.ToString());
            outputText.Items.Add("Execution Context :" + currentThread.ExecutionContext);
            outputText.Items.Add("Thread Priority :" + currentThread.Priority);
        }

        //定义无参方法 
        static void method()
        {
            Console.WriteLine("线程运行中..."); //TODO use UI to display the output result
            int a = 10 + 100;
            Console.WriteLine("线程运行结束");
        }
        private void autoReset_Click(object sender, RoutedEventArgs e)
        {
            outputText.Items.Add("创建线程");
            //创建无参数方法的托管线程
            //创建线程
            Thread thread1 = new Thread(new ThreadStart(method));
            outputText.Items.Add("");
            //启动线程
            thread1.Start();
            Console.WriteLine("线程启动");
        }
    }
}
