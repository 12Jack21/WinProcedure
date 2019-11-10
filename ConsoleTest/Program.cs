using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Reflection;

using Microsoft.Office.Interop.Excel;
using MsWord = Microsoft.Office.Interop.Word;
using System.IO;
using System.Net.NetworkInformation;
using Microsoft.Win32;
using System.Management;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            GetMacAddressByNetworkInformation();
            Console.ReadKey();
        }



        public static string GetMacAddressByNetworkInformation()
        {
            string key = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\";
            string macAddress = string.Empty;
            ManagementClass mc;
            try
            {
                mc = new ManagementClass("Win32_NetworkAdapterCOnfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach(ManagementObject mo in moc)
                {
                    if(mo["IPEnabled"].ToString() == "True")
                        macAddress += mo["MacAddress"].ToString() + "\n";
                }
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in nics)
                {
                    
                }
                String outPut = String.Format("MAC地址：{0}",macAddress);
                Console.WriteLine(outPut);
            }
            catch (Exception ex)
            {
                //这里写异常的处理
                Console.WriteLine(ex.Message);
            }
            return macAddress;
        }


        //调用 COM组件
        private void CallCOM()
        {

        }



        //托管方式调用dll
        private void DllManaged()
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
                            ParameterInfo[] methodParameter = m.GetParameters();

                            if (m.Name == "Add")
                            {
                                object[] parameters = new object[2];
                                parameters[0] = 2.4;
                                parameters[1] = 5.6;
                                object result = m.Invoke(null, parameters);

                                Console.WriteLine("Final Result From Managed = " + result.ToString());

                                double re = Add(2, 6);

                                Console.WriteLine("Final Result From Unmanaged = " + re);

                            }
                        }
                    }
                }
            }
            Console.ReadKey();
        }

        // att: 此方式一般适用于 C++ 生成的 DLL文件，不用通过在类中写方法
        [DllImport("../../../Release/OpDLL.dll")]
        public static extern double Add(double src1, double src2);
        [DllImport("../../../Release/OpDLL.dll")]
        public static extern double Multiply(double src1, double src2);


    }
}
