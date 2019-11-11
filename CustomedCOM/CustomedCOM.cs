using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CustomedCOM
{
    [Guid("CA4EB293-48E7-404E-9E38-AC92F562B31C")]
    [ComVisible(true)]
    public interface ICustomedOperation
    {
        void Connect(string connectString);
        void Disconnect();

        string GetVersion();

        int Add(int a, int b);
        int Multiply(int a, int b);
    }

    [Guid("0695719A-8D21-4428-AB41-4068431F1C54")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class CustomedOperation : ICustomedOperation
    {
        public int Add(int a, int b)
        {
            return a + b;
        }
        public int Multiply(int a, int b)
        {
            return a * b;
        }
        public void Connect(string connectString)
        {
            Console.WriteLine("Connecting......");
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public string GetVersion()
        {
            throw new NotImplementedException();
        }

        
    }
}
