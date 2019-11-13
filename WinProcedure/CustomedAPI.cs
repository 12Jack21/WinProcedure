using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinProcedure
{
    public class CustomedAPI
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;   // 传入自定义的数据，只能是 4字节整数
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;   // 传输的消息字符串
        }
    }
    public class RoomType
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Feature { get; set; }
        public double Price { get; set; }
        public string State { get; set; }


    }
    public class Room
    {
        //房号
        public string No { get; set; }
        //房型编号
        public string Address { get; set; }

        public string State { get; set; }
    }

    public class AttachFile
    {
        public string ID { get; set; }
        public string No { get; set; }
        public string Name { get; set; }
        public string Theme { get; set; }
        public string PublishTime { get; set; }
        public string CarryTime { get; set; }

        public AttachFile() { }
        public AttachFile(string no,string name,string publishTime,string carryTime)
        {
            No = no;
            Name = name;
            PublishTime = publishTime;
            CarryTime = carryTime;
        }
    }
    public class Book
    {
        public string No { get; set; }
        public string Name { get; set; }
        public string AuthorSurname { get; set; }
        public string AuthorName { get; set; }
        public string Publisher { get; set; }
        public double Price { get; set; }

        public Book() { }

        public Book(string no, string name, string authorSurname, string authorName, string publisher, double price)
        {
            No = no;
            Name = name;
            AuthorSurname = authorSurname;
            AuthorName = authorName;
            Publisher = publisher;
            Price = price;
        }


    }
}
