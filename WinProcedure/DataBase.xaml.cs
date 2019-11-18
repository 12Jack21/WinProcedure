using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SQLite;
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

namespace WinProcedure
{
    /// <summary>
    /// DataBase.xaml 的交互逻辑
    /// </summary>
    public partial class DataBase : UserControl
    {
        public DataBase()
        {
            InitializeComponent();
        }

        public DataTable GetExcelData(string Path, string sheet)
        {
            DataTable schemaTable = null;
            OleDbConnection ole = null;
            try
            {
                //连接语句，读取文件路劲
                string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + Path + ";" + @"Extended Properties=""Excel 12.0 Xml;HDR=YES""";
                string strExcel = "select * from [" + sheet + "$]";                                   //查询Excel表名，默认是Sheet1
                ole = new OleDbConnection(strConn);
                //打开连接
                ole.Open();
                schemaTable = new DataTable();
                OleDbDataAdapter odp = new OleDbDataAdapter(strExcel, strConn);
                odp.Fill(schemaTable);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (ole != null)
                    ole.Close();

            }
            return schemaTable;
        }
        private void showNoBtn_Click(object sender, RoutedEventArgs e)
        {

            RoomType roomType = (RoomType)roomTypeGrid.SelectedItem;
            if (roomType == null)
            {
                MessageBox.Show("请先选择一个房型!");
                return;
            }
            DataTable dt = GetExcelData(@"..\..\data\hotel.xls", "Sheet2");
            if (dt.Rows.Count > 0)
            {
                ObservableCollection<Room> lists = new ObservableCollection<Room>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string roomType_id = dt.Rows[i][0].ToString();
                    if (!roomType_id.Equals(roomType.Id))
                        continue;
                    Room room = new Room(roomType_id, dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), dt.Rows[i][3].ToString(), Convert.ToDouble(dt.Rows[i][4].ToString()), dt.Rows[i][5].ToString());
                    lists.Add(room);
                }
                roomGrid.ItemsSource = lists;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // 导入 Excel的数据
            DataTable dt = GetExcelData(@"..\..\data\hotel.xls", "Sheet1");
            if (dt.Rows.Count > 0)
            {
                ObservableCollection<RoomType> lists = new ObservableCollection<RoomType>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    RoomType roomType = new RoomType();
                    roomType.Id = dt.Rows[i][0].ToString();
                    roomType.Name = dt.Rows[i][1].ToString();
                    roomType.Feature = dt.Rows[i][2].ToString();
                    roomType.Price = Convert.ToDouble(dt.Rows[i][3].ToString());
                    roomType.State = dt.Rows[i][4].ToString();
                    lists.Add(roomType);
                }
                roomTypeGrid.ItemsSource = lists;
            }

            //初始化 SQlite 数据库(可先读取 .db文件)
            GetSQLiteData("../../data/demo.db");
            ShowSQLiteData();


        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        ObservableCollection<AttachFile> fileList = new ObservableCollection<AttachFile>();
        public void addToList(AttachFile file)
        {
            AddSQLiteData(file);
            ShowSQLiteData();
            //fileList.Add(file);
        }

        SQLiteConnection conn;
        private int count;

        private void ShowSQLiteData()
        {
            count = 0;
            string sql = "Select ID_KEY,FILE_NO,FILE_NAME,SUBJECT,PUBLISH_ORG,PUBLISH_DATE,IMPLEMENT_DATE from tbl_fgwj";
            SQLiteCommand command = conn.CreateCommand();
            command.CommandText = sql;

            SQLiteDataReader reader = command.ExecuteReader();
            fileList.Clear();
            while (reader.Read())
            {
                AttachFile attachFile = new AttachFile(reader.GetValue(1).ToString(), reader.GetValue(2).ToString(), reader.GetValue(3).ToString(),
                    reader.GetValue(4).ToString(), reader.GetValue(5).ToString(), reader.GetValue(6).ToString());
                attachFile.ID = reader.GetString(0);

                fileList.Add(attachFile);
            }
            fileGrid.ItemsSource = fileList;
        }
        private void GetSQLiteData(string path)
        {
            conn = new SQLiteConnection();
            conn.ConnectionString = @"Data Source=" + path;
            conn.Open();
        }
        private void AddSQLiteData(AttachFile file)
        {
            SQLiteCommand cmd = conn.CreateCommand();

            string sql = String.Format("Insert Into tbl_fgwj(ID_KEY,FILE_NO,FILE_NAME,SUBJECT,PUBLISH_DATE,IMPLEMENT_DATE) Values(@0,@1,@2,@3,@4,@5)");
            cmd.CommandText = sql;
            SQLiteParameter parameter = new SQLiteParameter("@0");

            parameter.Value = (++AttachFile.Count).ToString();

            cmd.Parameters.Add(parameter);
            parameter = new SQLiteParameter("@1");
            parameter.Value = file.No;
            cmd.Parameters.Add(parameter);
            parameter = new SQLiteParameter("@2");
            parameter.Value = file.Name;
            cmd.Parameters.Add(parameter);
            parameter = new SQLiteParameter("@3");
            parameter.Value = file.Theme;
            cmd.Parameters.Add(parameter);
            parameter = new SQLiteParameter("@4");
            parameter.Value = file.PublishTime;
            cmd.Parameters.Add(parameter);
            parameter = new SQLiteParameter("@5");
            parameter.Value = file.CarryTime;
            cmd.Parameters.Add(parameter);

            int tag = cmd.ExecuteNonQuery();
            if (tag >= 1)
            {
                MessageBox.Show("新增成功");
            }
        }
        //修改窗体
        private void ModifySQLiteData(AttachFile file)
        {
            SQLiteCommand cmd = conn.CreateCommand();
            //TODO 根据空值来判断要不要更新
            string sql = "Update tbl_fgwj Set FILE_NO =" + "\"" + file.No + "\"" + "," + "FILE_NAME=" + "\""+file.Name +"\""+ "," + "SUBJECT=" +
                file.Theme + ",PUBLISH_DATE=" + file.PublishTime + ",IMPLEMENT_DATE=" + file.CarryTime + " WHERE ID_KEY=" +"\""+ file.ID +"\"";
            cmd.CommandText = sql;
            int tag = cmd.ExecuteNonQuery();
            if (tag >= 1)
            {
                MessageBox.Show("修改成功");
            }
        }
        private void DeleteSQLiteData(string ID)
        {
            SQLiteCommand cmd = conn.CreateCommand();

            string sql = "Delete From tbl_fgwj Where ID_KEY=@0";
            cmd.CommandText = sql;
            SQLiteParameter parameter = new SQLiteParameter("@0");
            parameter.Value = ID;
            cmd.Parameters.Add(parameter);
            int tag = cmd.ExecuteNonQuery();
            if (tag >= 1)
            {
                MessageBox.Show("删除成功");
            }
        }
        private void createBtn_Click(object sender, RoutedEventArgs e)
        {
            CreateFile create = new CreateFile();
            create.database = this;

            create.Show();
        }

        private void modifyBtn_Click(object sender, RoutedEventArgs e)
        {
            int index = fileGrid.SelectedIndex;
            if (index < 0)
            {
                MessageBox.Show("请先选择要修改的记录！");
                return;
            }
            else
            {
                var selected = fileGrid.SelectedItem as DataRowView;
                DataGridRow row = (DataGridRow)fileGrid.ItemContainerGenerator.ContainerFromIndex(index);
                var file = row.Item;
                //AttachFile file = new AttachFile(selected.Row[0].ToString(), selected.Row[1].ToString(), selected.Row[2].ToString(), null,
                //    selected.Row[3].ToString(), selected.Row[4].ToString());

                ModifySQLiteData(file as AttachFile);
            }
        }
        private void deleteBtn__Click(object sender, RoutedEventArgs e)
        {
            object selected = fileGrid.SelectedItem;
            if (selected == null)
            {
                MessageBox.Show("请先选择删除的记录！");
                return;
            }
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("确定删除?", "删除确认", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DeleteSQLiteData((selected as AttachFile).ID);
                ShowSQLiteData();
            }
        }

        //更新数据表
        private void flashBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowSQLiteData();
        }

        /* ADO.NET 访问MySQL */

        //创建command对象	 
        private MySqlCommand cmd = null;
        //创建connection连接对象
        private MySqlConnection con = null;

        private void FindInstanceBtn_Click(object sender, RoutedEventArgs e)
        {
            MySqlDataReader reader = null;
            MySqlConnection con = null;
            try
            {
                con = ConnectDatabase();    //②打开数据库连接
                cmd = new MySqlCommand("show databases", con); //③使用指定的SQL命令和连接对象创建SqlCommand对象
                reader = cmd.ExecuteReader(); //④执行Command的ExecuteReader()方法

                //⑤将DataReader绑定到数据控件中 
                DataTable dt = new DataTable();
                dt.Load(reader);

                //添加进入 Combo中
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    String b = dt.Rows[i].ToString();
                    String a = dt.Rows[i][0].ToString();
                    instanceCombo.Items.Add(dt.Rows[i][0].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (reader != null)
                    //⑥关闭DataReader 
                    reader.Close();
                if (con != null)
                    //⑦关闭连接 
                    conn.Close();
            }
        }
        private MySqlConnection ConnectDatabase()
        {
            String username_ = username.Text;
            string password_ = password.Password;
            //if (String.IsNullOrEmpty(username_) || String.IsNullOrEmpty(password_))
            //{
            //    MessageBox.Show("用户名和密码不能为空！");
            //    return null;
            //}  
            username_ = "root";
            password_ = "zaoMENG45.";
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = "Server=localhost;Database =book;Uid=" + username_ + ";Pwd=" + password_ + ";";
            con.Open();
            return con;
        }
        private void connectBtn_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection con = null;
            try
            {
                String username_ = username.Text;
                string password_ = password.Password;
                if (String.IsNullOrEmpty(username_) || String.IsNullOrEmpty(password_))
                {
                    MessageBox.Show("用户名和密码不能为空！");
                    return;
                }
                con = ConnectDatabase();
                MessageBox.Show("连接MySQL数据库成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void dataReaderSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            MySqlDataReader reader = null;
            MySqlConnection con = null;
            try
            {
                con = ConnectDatabase();    //②打开数据库连接
                cmd = new MySqlCommand("select * from book", con); //③使用指定的SQL命令和连接对象创建SqlCommand对象
                reader = cmd.ExecuteReader(); //④执行Command的ExecuteReader()方法

                //⑤将DataReader绑定到数据控件中 
                DataTable dt = new DataTable();
                dt.Load(reader);
                bookGrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //⑥关闭DataReader 
                reader.Close();
                //⑦关闭连接 
                conn.Close();
            }
        }

        private void dataAdapterSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection con = ConnectDatabase();
            //建立DataSet对象(相当于建立前台的虚拟数据库)
            DataSet ds = new DataSet();
            //建立DataTable对象(相当于建立前台的虚拟数据库中的数据表)
            DataTable dtable = new DataTable();

            string sltStr = "select * from book";
            MySqlCommand sqlCmd = new MySqlCommand(sltStr, con);
            //建立DataAdapter对象  
            MySqlDataAdapter msda = new MySqlDataAdapter(sqlCmd);

            //拿到 DataGrid的数据
            msda.Fill(dtable);

            bookGrid.ItemsSource = dtable.DefaultView;
        }

        //批量更新
        private void dataAdapterBatchUpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection con = ConnectDatabase();
            //建立DataSet对象(相当于建立前台的虚拟数据库)
            DataSet ds = new DataSet();
            //建立DataTable对象(相当于建立前台的虚拟数据库中的数据表)
            DataTable dtable;

            string sltStr = "select * from book";
            MySqlCommand sqlCmd = new MySqlCommand(sltStr, con);
            //建立DataAdapter对象  
            MySqlDataAdapter msda = new MySqlDataAdapter(sqlCmd);

            //dtable
            //DataView dv = bookGrid.Items
            //拿到 DataGrid的数据
            dtable = ((DataView)bookGrid.ItemsSource).Table;
            var item = bookGrid.Items;
            DataRowView items = bookGrid.Items[0] as DataRowView;
            DataTable dt = items.DataView.Table;


            //msda.Update();
            int a = msda.Update(dtable);
            if (a > 0)
            {
                MessageBox.Show("批量更新成功！");
            }
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection con = ConnectDatabase();
            //建立DataSet对象(相当于建立前台的虚拟数据库)
            DataSet ds = new DataSet();
            //建立DataTable对象(相当于建立前台的虚拟数据库中的数据表)
            DataTable dtable;

            string sltStr = "select * from book ";
            MySqlCommand sqlCmd = new MySqlCommand(sltStr, con);
            //建立DataAdapter对象  
            MySqlDataAdapter msda = new MySqlDataAdapter(sqlCmd);

            //拿到 DataGrid的数据
            dtable = (bookGrid.ItemsSource as DataView).Table;
            int a = msda.Update(dtable);
            if (a > 0)
            {
                MessageBox.Show("新增成功！");
            }
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection con = ConnectDatabase();
            int index = bookGrid.SelectedIndex;
            if (index < 0)
            {
                MessageBox.Show("请先选择要删除的记录");
                return;
            }

            //建立DataSet对象(相当于建立前台的虚拟数据库)
            DataSet ds = new DataSet();
            //建立DataTable对象(相当于建立前台的虚拟数据库中的数据表)
            DataTable dtable;

            string sltStr = "select * from book";
            MySqlCommand sqlCmd = new MySqlCommand(sltStr, con);
            //建立DataAdapter对象  
            MySqlDataAdapter msda = new MySqlDataAdapter(sqlCmd);

            //拿到 DataGrid的数据
            dtable = (bookGrid.ItemsSource as DataView).Table;
            DataRow row = dtable.Rows[index];
            row.Delete();

            int a = msda.Update(dtable);
            if (a > 0)
            {
                MessageBox.Show("删除成功");
            }
        }
    }
}
