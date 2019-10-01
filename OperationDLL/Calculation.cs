using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationDLL
{
    public class Calculation
    {
        public static int AddInt(int i1,int i2)
        {
            return i1 + i2;
        }
        public static String Add(double src1, double src2)
        {
            return src1 + " + " + src2 + " = " + (src1 + src2);
        }

        public static String Multiply(double src1, double src2)
        {
            double result = src1 * src2;
            //最大小数点后1两位
            return src1 + " * " + src2 + " = " + result.ToString("0.##");
        }
        //斐波那契数列
        public static long Fibonacci(int src)
        {
            if (src == 1 || src == 2)
                return 1;
            return Fibonacci(src - 1) + Fibonacci(src - 2);
        }

        //阶乘(返回值为 long 防止结果过大)
        public static long Factorial(int src)
        {
            long result = 1;
            if(src > 1)
            {
                for(int i = 1;i <= src; i++)
                {
                    result *= i;
                }
            }
            return result;
        }
    }
}
