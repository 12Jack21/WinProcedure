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
                if(ole != null)
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
                    Room room = new Room();
                    room.No = dt.Rows[i][1].ToString();
                    room.Address = dt.Rows[i][2].ToString();
                    room.State = dt.Rows[i][5].ToString();
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
            string sql = "Select ID,REPORT_NO,FILE_NAME,FILE_INDEX,UP_USER,UP_TIME,CARRY_TIME from attach_file";
            SQLiteCommand command = conn.CreateCommand();
            command.CommandText = sql;

            SQLiteDataReader reader = command.ExecuteReader();
            fileList.Clear();
            while (reader.Read())
            {
                AttachFile attachFile = new AttachFile(reader.GetValue(1).ToString(), reader.GetValue(2).ToString(), reader.GetValue(3).ToString(),
                    reader.GetValue(4).ToString(),reader.GetValue(5).ToString(),reader.GetValue(6).ToString());
                attachFile.ID =  reader.GetString(0);

                if (Convert.ToInt32(reader.GetString(0)) > count)
                    count = Convert.ToInt32(reader.GetString(0));
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

            string sql = String.Format("Insert Into attach_file(ID,REPORT_NO,FILE_NAME,FILE_INDEX,UP_TIME,CARRY_TIME) Values(@0,@1,@2,@3,@4,@5)");
            cmd.CommandText = sql;
            SQLiteParameter parameter = new SQLiteParameter("@0");
            parameter.Value = (AttachFile.Count++).ToString();
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
            if(tag >= 1)
            {
                MessageBox.Show("新增成功");
            }
        }
        private void ModifySQLiteData(AttachFile file)
        {
            SQLiteCommand cmd = conn.CreateCommand();

            string sql = "Update attach_file Set REPORT_NO =" + file.No + "," + "FILE_NAME=" + file.Name + "," + "FILE_INDEX=" + 
                file.Theme + "UP_TIME=" + file.PublishTime + "CARRY_TIME=" + file.CarryTime;
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

            string sql = "Delete From attach_file Where ID=@0";
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

        }
        private void deleteBtn__Click(object sender, RoutedEventArgs e)
        {
            object selected = fileGrid.SelectedItem;
            if(selected == null)
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
        private void FindInstanceBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
