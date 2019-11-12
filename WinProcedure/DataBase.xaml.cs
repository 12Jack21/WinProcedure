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

        public DataTable GetData(string Path)
        {
            //连接语句，读取文件路劲
            string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + Path + ";" + "Extended Properties=Excel 12.0;";      
            string strExcel = "select * from [Sheet1$]";                                   //查询Excel表名，默认是Sheet1
            OleDbConnection ole = new OleDbConnection(strConn);
            ole.Open();                                                                                          //打开连接
            DataTable schemaTable = new DataTable();
            OleDbDataAdapter odp = new OleDbDataAdapter(strExcel, strConn);
            odp.Fill(schemaTable);
            ole.Close();
            return schemaTable;

        }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
