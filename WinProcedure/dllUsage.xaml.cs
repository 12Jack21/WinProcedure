using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

using OperationDLL;
using System.Reflection;
using System.Runtime.InteropServices;

namespace WinProcedure
{
    /// <summary>
    /// dllUsage.xaml 的交互逻辑
    /// </summary>
    public partial class DllUsage : UserControl
    {
        public DllUsage()
        {
            InitializeComponent();
        }

        // att: 此方式一般适用于 C++ 生成的 DLL文件，不用通过在类中写方法
        [DllImport("../../../Release/OpDLL.dll")]
        public static extern double Add(double src1, double src2);
        [DllImport("../../../Release/OpDLL.dll")]
        public static extern double Multiply(double src1, double src2);

        private String FibonacciManaged(int src)
        {
            Assembly a = Assembly.LoadFrom("../../../related_dll/OperationDLL.dll");
            foreach (Type t in a.GetTypes())
            {
                Console.WriteLine("Type's Name :" + t.Name);
                if (t.IsClass && !t.IsAbstract)
                {
                    if (t.Name == "Calculation")
                    {
                        MethodInfo[] methodInfos = t.GetMethods();
                        foreach (MethodInfo m in methodInfos)
                        {
                            Console.WriteLine(m.Name + "    ");
                            if (m.Name == "Fibonacci")
                            {
                                object[] parameters = new object[1];
                                parameters[0] = src;
                                object result = m.Invoke(null, parameters);

                                Console.WriteLine("Final Fibonacci Result From Managed = " + result.ToString());

                                return result.ToString();
                            }
                        }
                    }
                }
            }
            return null;
        }

        private String FactorialManaged(int src)
        {
            Assembly a = Assembly.LoadFrom("../../../related_dll/OperationDLL.dll");
            foreach (Type t in a.GetTypes())
            {
                Console.WriteLine("Type's Name :" + t.Name);
                if (t.IsClass && !t.IsAbstract)
                {
                    if (t.Name == "Calculation")
                    {
                        MethodInfo[] methodInfos = t.GetMethods();
                        foreach (MethodInfo m in methodInfos)
                        {
                            Console.WriteLine(m.Name + "    ");
                            if (m.Name == "Factorial")
                            {
                                object[] parameters = new object[1];
                                parameters[0] = src;
                                object result = m.Invoke(null, parameters);

                                Console.WriteLine("Final Factorial Result From Managed = " + result.ToString());

                                return result.ToString();
                            }
                        }
                    }
                }
            }
            return null;
        }

        private void MultiBtn_Click(object sender, RoutedEventArgs e)
        {
            if (modeLabel.Content.ToString() != "非托管模式")
            {
                MessageBox.Show("请先切换到非托管模式!");
                return;
            }
            String src1 = this.src1.Text.Trim();
            String src2 = this.src2.Text.Trim();

            if(src1 == null || src1 == "" || src2 == null || src2 =="")
            {
                MessageBox.Show("请在输入框输入要计算的两个操作数！！！");
            }
            else
            {
                double d1, d2;
                bool b1 = Double.TryParse(src1, out d1);
                bool b2 = Double.TryParse(src2, out d2);

                if(b1 && b2)
                {
                    //通过 DllImport 调用 C++ 的dll类库
                    result1.Content = Multiply(d1, d2);
                }
                else
                {
                    MessageBox.Show("输入的操作数不符合规范！！！");
                }
            }

        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (modeLabel.Content.ToString() != "非托管模式")
            {
                MessageBox.Show("请先切换到非托管模式!");
                return;
            }
            String src1 = this.src1.Text.Trim();
            String src2 = this.src2.Text.Trim();

            if (src1 == null || src1 == "" || src2 == null || src2 == "")
            {
                MessageBox.Show("请在输入框输入要计算的两个操作数！！！");
            }
            else
            {
                double d1, d2;
                bool b1 = Double.TryParse(src1, out d1);
                bool b2 = Double.TryParse(src2, out d2);

                if (b1 && b2)
                {
                    //直接引用方式调用 c# 的dll 类库
                    result1.Content = Calculation.Add(d1, d2);
                }
                else
                {
                    MessageBox.Show("输入的操作数不符合规范！！！");
                }
            }
        }

        private void FactorBtn_Click(object sender, RoutedEventArgs e)
        {
            if (modeLabel.Content.ToString() != "托管模式")
            {
                MessageBox.Show("请先切换到托管模式!");
                return;
            }
            String src3 = this.src3.Text.Trim();
            if(src3 == null || src3 == "")
            {
                MessageBox.Show("请在输入框输入要计算的操作数！！！");
            }
            else
            {
                int iSrc;
                bool can = Int32.TryParse(src3, out iSrc);
                if(can)
                {
                    //托管方式调用dll
                    result2.Content = FactorialManaged(iSrc);
                }
                else
                {
                    MessageBox.Show("请输入整数！！！");
                }
            }
        }

        private void FiboBtn_Click(object sender, RoutedEventArgs e)
        {
            if (modeLabel.Content.ToString() != "托管模式")
            {
                MessageBox.Show("请先切换到托管模式!");
                return;
            }
            String src3 = this.src3.Text.Trim();
            if (src3 == null || src3 == "")
            {
                MessageBox.Show("请在输入框输入要计算的操作数！！！");
            }
            else
            {
                int iSrc;
                bool can = Int32.TryParse(src3, out iSrc);
                if (can)
                {
                    //托管方式调用 dll
                    result2.Content = FibonacciManaged(iSrc);
                }
                else
                {
                    MessageBox.Show("请输入整数！！！");
                }
            }
        }

        private void ModeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (modeLabel.Content.ToString() == "托管模式")
                modeLabel.Content = "非托管模式";
            else
                modeLabel.Content = "托管模式";
        }

        private void Reflection_Click(object sender, RoutedEventArgs e)
        {
            if(modeLabel.Content.ToString() != "托管模式")
            {
                MessageBox.Show("请先切换到托管模式!");
                return;
            }
            //托管模式导入
            Assembly a = Assembly.LoadFrom("../../../related_dll/OperationDLL.dll");
            foreach (Type t in a.GetTypes())
            {
                Console.WriteLine("Type's Name :" + t.Name);
                if (t.IsClass && !t.IsAbstract)
                {
                    //找到改类名称
                    if (t.Name == "Calculation")
                    {
                        //所有方法信息
                        MethodInfo[] methodInfos = t.GetMethods();
                        String funcInfo = null;
                        foreach (MethodInfo m in methodInfos)
                        {
                            Console.WriteLine(m.Name + "    ");

                            funcInfo = m.ReturnType.Name.ToString() + "  " + m.Name.ToString() + "(";
                            for(int i = 0; i < m.GetParameters().Length;i++){
                                funcInfo += m.GetParameters()[i].ParameterType.ToString() + " " + m.GetParameters()[i].Name;
                                if (i != m.GetParameters().Length - 1)
                                    funcInfo += ",";
                            }
                            funcInfo += ")";
                            funcListBox.Items.Add(funcInfo.ToString());
                            funcInfo = null;
                        }
                    }
                }
            }
        }
    }
}
