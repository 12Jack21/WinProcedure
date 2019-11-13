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
}
