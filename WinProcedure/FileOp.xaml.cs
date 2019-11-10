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

using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace WinProcedure
{
    /// <summary>
    /// FileOp.xaml 的交互逻辑
    /// </summary>
    public partial class FileOp : Page
    {
        public FileOp()
        {
            InitializeComponent();
        }

        private static String dirpath; //要打开的目录
        private static String dest_file; //合并后的文件名

        private void SeldirBtn_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog browserDialog = new FolderBrowserDialog();
            if(browserDialog.ShowDialog() == DialogResult.OK)
            {
                dirpath = browserDialog.SelectedPath;
            }
            browserDialog.Dispose();
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            if(dirpath == null || dirpath == "")
            {
                System.Windows.MessageBox.Show("请选择要搜索的目录！！！");
                return;
            }

            String[] folder_files;
            //检查文件目录是否存在
            if (Directory.Exists(dirpath))
            {
                folder_files = Directory.GetFiles(dirpath, dirpatText.Text, SearchOption.AllDirectories);
                srcList.Items.Clear();

                int selected_index;
                foreach(string folder_file in folder_files)
                {
                    selected_index = srcList.Items.Add(folder_file);
                    srcList.SelectedIndex = selected_index; //设置一种添加的连续动作
                }
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if(srcList.SelectedItems.Count == 0)
            {
                System.Windows.MessageBox.Show("请选择要填入目标集的文件！");
                return;
            }
            foreach(object file in srcList.SelectedItems)
            {
                if(!tarList.Items.Contains(file))
                    tarList.Items.Add(file);
            }
        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (srcList.SelectedItems.Count == 0)
            {
                System.Windows.MessageBox.Show("请选择要移出目标集的文件！");
                return;
            }

            for(int i = tarList.SelectedItems.Count - 1; i >= 0; i--)
            {
                tarList.Items.Remove(tarList.SelectedItems[i]);
            }
        }

        private void UpBtn_Click(object sender, RoutedEventArgs e)
        {
            if (tarList.SelectedItems.Count > 1)
            {
                System.Windows.MessageBox.Show("不能选择多个文件项");
            }
            else if (tarList.SelectedItems.Count == 0)
            {
                System.Windows.MessageBox.Show("请选择要上移的文件项");
            }
            else
            {
                int sel_index = tarList.SelectedIndex;
                string sel_str = tarList.SelectedItem.ToString();
                if (sel_index > 0)
                {
                    //当前项与前一项交换
                    tarList.Items[sel_index] = tarList.Items[sel_index - 1];
                    tarList.Items[sel_index - 1] = sel_str;
                    tarList.SelectedIndex = sel_index - 1;
                }
            }
        }

        private void DownBtn_Click(object sender, RoutedEventArgs e)
        {
            if (tarList.SelectedItems.Count > 1)
            {
                System.Windows.MessageBox.Show("不能选择多个文件项");
            }
            else if (tarList.SelectedItems.Count == 0)
            {
                System.Windows.MessageBox.Show("请选择要下移的文件项");
            }
            else
            {
                int sel_index = tarList.SelectedIndex;
                string sel_str = tarList.SelectedItem.ToString();
                if (sel_index < tarList.Items.Count - 1)
                {
                    //当前项与前一项交换
                    tarList.Items[sel_index] = tarList.Items[sel_index + 1];
                    tarList.Items[sel_index + 1] = sel_str;
                    tarList.SelectedIndex = sel_index + 1;
                }
            }
        }

        private void TarFilenameBtn_Click(object sender, RoutedEventArgs e)
        {
            //用一个 textbox 
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Title = "选择要合并后的文件名";
            fileDialog.InitialDirectory = System.Environment.SpecialFolder.DesktopDirectory.ToString(); //初始化保存目录为桌面
            fileDialog.OverwritePrompt = false;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                dest_file = fileDialog.FileName;
                destNameLabel.Content = dest_file;
            }
            fileDialog.Dispose();
        }

        private void MergeBtn_Click(object sender, RoutedEventArgs e)
        {
            if(dest_file == null || dest_file == "")
            {
                System.Windows.MessageBox.Show("请选择合并后文件的位置和名称");
                return;
            }
            if(tarList.SelectedItems.Count == 0)
            {
                System.Windows.MessageBox.Show("请选择要合并的文件");
                return;
            }

            bool isChLine = chLineChk.IsChecked.Value;
            bool isAddName = addNameChk.IsChecked.Value;
            bool isOpenFile = openFileChk.IsChecked.Value;

            if (File.Exists(dest_file))
            {
                File.Delete(dest_file);
            }

            FileStream fs_dest = null;
            FileStream fs_source;
            try
            {
                fs_dest = new FileStream(dest_file, FileMode.CreateNew, FileAccess.Write);
                byte[] dataBuffer = new byte[100000];
                byte[] file_name_buff;
                int read_len;
                FileInfo fi_a = null;

                for (int i = 0; i < tarList.SelectedItems.Count; i++)
                {
                    fi_a = new FileInfo(tarList.SelectedItems[i].ToString()); //为了拿到文件的实际名字
                    file_name_buff = Encoding.Default.GetBytes(fi_a.Name);

                    if (isAddName)
                    {
                        fs_dest.Write(file_name_buff, 0, file_name_buff.Length); //写入文件名
                        fs_dest.WriteByte((byte)13);
                        fs_dest.WriteByte((byte)10); //写入文件名后换行
                    }
                    if (isChLine)
                    {
                        fs_dest.WriteByte((byte)13);// 回车的编码
                        fs_dest.WriteByte((byte)10);// 换行的编码
                    }

                    fs_source = new FileStream(fi_a.FullName, FileMode.Open, FileAccess.Read);
                    read_len = fs_source.Read(dataBuffer, 0, 100000);
                    while (read_len > 0)
                    {
                        fs_dest.Write(dataBuffer, 0, read_len); //写入读到的字节即可
                        read_len = fs_source.Read(dataBuffer, 0, 100000);
                    }

                    if (isChLine)
                    {
                        fs_dest.WriteByte((byte)13);// 回车的编码
                        fs_dest.WriteByte((byte)10);// 换行的编码
                    }
                    fs_source.Dispose(); //释放资源
                    fs_dest.Flush();

                }
            }
            catch (Exception ee)
            {
                System.Windows.MessageBox.Show(ee.Message);
            }
            finally
            {
                fs_dest.Close();
                fs_dest.Dispose();

                if (isOpenFile)
                    Process.Start(dest_file);
            }
        }

        private void SelCurBtn_Click(object sender, RoutedEventArgs e)
        {
            //打开选中文件
            if(tarList.SelectedItems.Count == 0)
            {
                System.Windows.MessageBox.Show("请选择要打开的文件");
            }
            else
            {
                foreach(object file in tarList.SelectedItems)
                {
                    if (File.Exists(file.ToString()))
                    {
                        Process.Start(file.ToString());
                    }
                }
            }
        }
    }
}
