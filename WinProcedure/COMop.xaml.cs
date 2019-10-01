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

using System.Runtime.InteropServices;
using System.Reflection;

using Microsoft.Office.Interop.Excel;
using MsWord = Microsoft.Office.Interop.Word;
using System.IO;

namespace WinProcedure
{
    /// <summary>
    /// COMop.xaml 的交互逻辑
    /// </summary>
    public partial class COMop : UserControl
    {
        public COMop()
        {
            InitializeComponent();
        }

        private void WordBtn_Click(object sender, RoutedEventArgs e)
        {
            MsWord.Application oApplic = new MsWord.Application();
            object missing = Missing.Value;
            MsWord.Document odoc = oApplic.Documents.Add(ref missing, ref missing, ref missing, ref missing);
            odoc.Activate();
            Console.WriteLine("文件已打开");
        }
    }
}
