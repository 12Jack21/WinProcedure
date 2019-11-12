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
        public string id { get; set; }
        public string name { get; set; }
        public string feature { get; set; }
        public double price { get; set; }
        public string state { get; set; }

    }
    public class Room
    {
        //房号
        public string id { get; set; }
        //房型编号
        public string address { get; set; }

    }
}
