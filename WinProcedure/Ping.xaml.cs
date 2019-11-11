using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        // 调用CMD命令并重定向
        private void RedirectCMD(string command)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            // 是否使用外壳程序   
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            // 重定向输入输出流  
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;

            // 禁用按钮(不能同时按两次)
            asyncPingBtn.IsEnabled = false;
            getMacBtn.IsEnabled = false;
            try
            {
                process.Start();

                process.StandardInput.WriteLine(command);
                process.StandardInput.WriteLine("exit");
                //  Console.WriteLine("开始执行");
                process.OutputDataReceived += (s, _e) => AppendResult(_e.Data);
                // 退出时的回调函数，恢复按钮
                process.Exited += (s, _e) => asyncPingBtn.Dispatcher.BeginInvoke(new Action(() => asyncPingBtn.IsEnabled = true));
                process.Exited += (s, _e) => getMacBtn.Dispatcher.BeginInvoke(new Action(() => getMacBtn.IsEnabled = true));

                process.EnableRaisingEvents = true;
                process.BeginOutputReadLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // 异步更新结果
        private void AppendResult(string data)
        {
            outputText.Dispatcher.Invoke(new Action(() => outputText.Text += data + "\n"));
        }
        private void SyncPingBtn_Click(object sender, RoutedEventArgs e)
        {
            String site = siteText.Text;
            if (String.IsNullOrEmpty(site))
            {
                MessageBox.Show("请输入要ping的地址！");
                return;
            }
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            //是否使用外壳程序
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            //重定向IO
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;

            //使Ping命令执行 6 次
            string strCmd = "ping " + site + " -n 6";
            process.Start();
            process.StandardInput.WriteLine(strCmd); //输入命令
            process.StandardInput.WriteLine("exit"); //上一条命令执行完之后才会执行该条

            //获取输出信息
            string output = process.StandardOutput.ReadToEnd();
            outputText.Text += output;
            process.WaitForExit();
            process.Close();
        }
        private void AsyncPingBtn_Click(object sender, RoutedEventArgs e)
        {
            RedirectCMD("ping " + siteText.Text + " -n 6");
        }

        private void GetMacBtn_Click(object sender, RoutedEventArgs e)
        {
            RedirectCMD("getmac");
        }

        private void current_Click(object sender, RoutedEventArgs e)
        {
            outputText.Text = "当前线程的信息如下所示";
            Thread currentThread = Thread.CurrentThread;
            outputText.Text += "\n线程名 :" + currentThread.Name;
            outputText.Text += "\n线程状态 :" + currentThread.ThreadState.ToString();
            outputText.Text += "\n运行的上下文环境 :" + currentThread.ExecutionContext;
            outputText.Text += "\n线程优先级 :" + currentThread.Priority + "\n";
        }

        //定义无参方法
        private void NormalMethod()
        {
            outputText.Dispatcher.Invoke(new Action(() => outputText.Text += "一个无参的方法\n"));
        }
        //定义有参方法
        private void ParameterizedMethod(object src)
        {
            Console.WriteLine("线程运行中..."); //TODO use UI to display the output result
            outputText.Dispatcher.Invoke(new Action(() =>
            {
                outputText.Text += "一个有参数的委托\n";
                outputText.Text += "接受参数 " + src + "\n";
                outputText.Text += "计算结果 " + (int)src + " + " + "100= " + ((int)src + 100) + "\n";
            }));
            Console.WriteLine("线程运行结束");
        }
        private void create_Click(object sender, RoutedEventArgs e)
        {
            outputText.Text += "线程创建...\n";
            //创建线程
            //启动线程
            Thread thread1 = new Thread(new ThreadStart(NormalMethod));
            thread1.Start();

            //通过匿名委托创建
            Thread thread2 = new Thread(delegate ()
            {
                outputText.Dispatcher.Invoke(new Action(() => outputText.Text += "一个通过匿名委托创建的线程\n"));
            });
            thread2.Start();

            //通过Lambda表达式创建
            Thread thread3 = new Thread(() =>
            {
                outputText.Dispatcher.Invoke(new Action(() => outputText.Text += "一个通过Lambda表达式创建的线程\n"));
            });
            thread3.Start();

            Thread thread4 = new Thread(new ParameterizedThreadStart(ParameterizedMethod));
            thread4.Start(2000);

        }

        private int front_num = 0;
        private int back_num = 0;
        private void Front_Click(object sender, RoutedEventArgs e)
        {
            //默认创建的即为前台进程
            Thread front;
            for (int i = 0; i < 3; i++)
            {
                front = new Thread(() => { });
                front.Start();
                outputText.Text += "前台进程计数：" + front_num++ + "\n";
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            //默认创建的即为前台进程
            Thread back;
            for (int i = 0; i < 3; i++)
            {
                back = new Thread(() => { });
                back.IsBackground = true; // 设置为后台进程
                back.Start();
                outputText.Text += "后台进程计数：" + back_num++ + "\n";
            }
        }

        private void Join_Click(object sender, RoutedEventArgs e)
        {
            Thread thread1 = new Thread(() =>  Thread.Sleep(600));
            Thread thread2 = new Thread(() => Thread.Sleep(1000));

            thread1.Start();
            thread2.Start();
            thread1.Join();
            outputText.Text += "600毫秒线程结束\n";

            thread2.Join();
            outputText.Text += "1000毫秒线程结束\n";
        }

        private void doSomeThingLong()
        {
            Thread.Sleep(200);
        }
        private void doSomeThingLongAsync()
        {
            String Name = Thread.CurrentThread.Name;
            outputText.Dispatcher.Invoke(new Action(() => outputText.Text += Name + " start\n"));
            Thread.Sleep(200);
        }
        //TODO 同步方法
        private void Sync_Click(object sender, RoutedEventArgs e)
        {
            outputText.Text +=  "****************Sync_Btn_Click start" + " ,Time: " + DateTime.Now + "******************\n";
            Thread thread1 = new Thread(new ThreadStart(doSomeThingLong));
            thread1.Name = "doSomeThingLong thread_1";
            Thread thread2 = new Thread(new ThreadStart(doSomeThingLong));
            thread2.Name = "doSomeThingLong thread_2";
            Thread thread3 = new Thread(new ThreadStart(doSomeThingLong));
            thread3.Name = "doSomeThingLong thread_3";
            Thread thread4 = new Thread(new ThreadStart(doSomeThingLong));
            thread4.Name = "doSomeThingLong thread_4";

            outputText.Text += thread1.Name + " start, Time: " + DateTime.Now + "\n";
            thread1.Start();
            thread1.Join();
            outputText.Text += "doSomeThingLong thread_1 end" + " ,Time: " + DateTime.Now + "\n";
            outputText.Text += thread2.Name + " start, Time: " + DateTime.Now + "\n";
            thread2.Start();
            thread2.Join();
            outputText.Text += "doSomeThingLong thread_2 end" + " ,Time: " + DateTime.Now + "\n";
            outputText.Text += thread3.Name + " start, Time: " + DateTime.Now + "\n";
            thread3.Start();
            thread3.Join();
            outputText.Text += "doSomeThingLong thread_3 end" + " ,Time: " + DateTime.Now + "\n";
            outputText.Text += thread4.Name + " start, Time: " + DateTime.Now + "\n";
            thread4.Start();
            thread4.Join();
            outputText.Text += "doSomeThingLong thread_4 end" + " ,Time: " + DateTime.Now + "\n";

            outputText.Text += "****************Sync_Btn_Click end" + " ,Time: " + DateTime.Now + "******************\n";
        }

        private void Async_Click(object sender, RoutedEventArgs e)
        {
            Thread thread1 = new Thread(new ThreadStart(doSomeThingLongAsync));
            Thread thread2 = new Thread(new ThreadStart(doSomeThingLongAsync));
            Thread thread3 = new Thread(new ThreadStart(doSomeThingLongAsync));
            Thread thread4 = new Thread(new ThreadStart(doSomeThingLongAsync));
            thread1.Name = "doSomeThingLong thread_1";
            thread2.Name = "doSomeThingLong thread_2";
            thread3.Name = "doSomeThingLong thread_3";
            thread4.Name = "doSomeThingLong thread_4";
            outputText.Text += "****************Sync_Btn_Click start" + " ,Time: " + DateTime.Now + "******************\n";

            thread1.Start();
            thread2.Start();
            thread3.Start();
            thread4.Start();
        }

        //// 定义一个回调
        //AsyncCallback callback = p =>
        //{
        //    Console.WriteLine($"到这里计算已经完成了。{Thread.CurrentThread.ManagedThreadId.ToString("00")}。");
        //    update($"到这里计算已经完成了。" + Thread.CurrentThread.ManagedThreadId.ToString("00") + "。");
        //};


        private void AsyncCall_Click(object sender, RoutedEventArgs e)
        {
            ////异步调用回调
            //for (int i = 0; i < 5; i++)
            //{
            //    string name = string.format($"btnsync_click_{i}");

            //    asyncresult = action.begininvoke(name, callback, null);
            //}
        }
    }
}
