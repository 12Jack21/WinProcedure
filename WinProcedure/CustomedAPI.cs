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
        public string Id { get; set; }
        //房号
        public string No { get; set; }
        //房型编号
        public string Address { get; set; }

        public string Feature { get; set; }
        public Double Price { get; set; }
        public string State { get; set; }

        public Room() { }
        public Room(string id, string no, string address, string feature, Double price, string state)
        {
            Id = id;
            No = no;
            Address = address;
            Feature = feature;
            Price = price;
            State = state;
        }
    }

    public class AttachFile
    {
        public string ID { get; set; }
        public string No { get; set; }
        public string Name { get; set; }
        public string Theme { get; set; }
        public string PublishUser { get; set; }
        public string PublishTime { get; set; }
        public string CarryTime { get; set; }

        public AttachFile() { }
        public static int Count;
        public AttachFile(string no,string name,string theme,string publishUser,string publishTime,string carryTime)
        {
            No = no;
            Name = name;
            Theme = theme;
            PublishUser = publishUser;
            PublishTime = publishTime;
            CarryTime = carryTime;
        }

        public AttachFile(string no, string name, string publishUser, string publishTime, string carryTime)
        {
            No = no;
            Name = name;
            PublishUser = publishUser;
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
        public Double Price { get; set; }

        public static int Count;
        public Book() { }

        public Book(string no, string name, string authorSurname, string authorName, string publisher, Double price)
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
