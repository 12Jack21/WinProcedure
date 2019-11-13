using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using Forms = System.Windows.Forms;

namespace WinProcedure
{
    /// <summary>
    /// CreateFile.xaml 的交互逻辑
    /// </summary>
    public partial class CreateFile : Window
    {
        public CreateFile()
        {
            InitializeComponent();
        }
        public DataBase database { get; set; }

        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            Forms.OpenFileDialog fileDialog = new Forms.OpenFileDialog();
            string filepath = null;
            if( fileDialog.ShowDialog() == Forms.DialogResult.OK)
            {
                filepath = fileDialog.FileName;
                if (File.Exists(filepath))
                {
                    FileInfo fileInfo = new FileInfo(filepath);
                    fileName.Text = fileInfo.Name;                   
                }
            }
            fileDialog.Dispose();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            AttachFile file = new AttachFile(fileNo.Text, fileName.Text, publishUnit.Text,publishTime.DisplayDate.Date.ToShortDateString(), carryTime.DisplayDate.Date.ToShortDateString());
            database.addToList(file);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
          
            this.Close();
        }
    }
}
