using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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

        public DataTable GetData(string Path,string sheet)
        {
            //连接语句，读取文件路劲
            string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + Path + ";" + "Extended Properties=Excel12.0;";      
            string strExcel = "select * from [" + sheet  +"$]";                                   //查询Excel表名，默认是Sheet1
            OleDbConnection ole = new OleDbConnection(strConn);
            //打开连接
            ole.Open();                                                                                        
            DataTable schemaTable = new DataTable();
            OleDbDataAdapter odp = new OleDbDataAdapter(strExcel, strConn);
            odp.Fill(schemaTable);
            ole.Close();
            return schemaTable;

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DataTable dt = GetData(@"D:\File\Special\Desktop\WinDemand\hotel.xls","sheet1");
            if(dt.Rows.Count > 0)
            {
                List<RoomType> lists = new List<RoomType>();
                for(int i = 0; i < dt.Rows.Count; i++)
                {
                    RoomType roomType = new RoomType();
                    roomType.id = dt.Rows[i][0].ToString();
                    roomType.name = dt.Rows[i][1].ToString();
                    roomType.feature = dt.Rows[i][2].ToString();
                    roomType.state = dt.Rows[i][3].ToString();
                    lists.Add(roomType);
                }
                roomTypeGrid.ItemsSource = lists;
            }
        }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void showNumBtn_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = roomTypeGrid.SelectedItem as DataRowView;

            DataTable dt = GetData(@"D:\File\Special\Desktop\WinDemand\hotel.xls", "sheet2");
            if (dt.Rows.Count > 0)
            {
                List<Room> lists = new List<Room>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Room room = new Room();
                    room.id = dt.Rows[i][0].ToString();
                    room.address = dt.Rows[i][1].ToString();
                    lists.Add(room);
                }
                roomTypeGrid.ItemsSource = lists;
            }
        }
    }
}
