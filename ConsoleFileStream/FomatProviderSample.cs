using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFileStream
{
    public class FomatProviderSample
    {
        public FomatProviderSample()
        {

        }

        public void EX1()
        {
            //有關數字格式化隱性使用IFomatProvider的例子#

            //貨幣            
            Console.WriteLine(string.Format("顯示貨幣格式{0:c3} ", 12));
            //十進制            
            Console.WriteLine("顯示貨幣十進制格式{0:d10} ", 12);
            //科學計數法            
            Console.WriteLine("科學計數法{0:e5} ", 12);
            //固定點格式            
            Console.WriteLine("固定點格式{ 0:f10} ", 12);
            //常規格式

            Console.WriteLine("常規格式{0:g10} ", 12);
            //數字格式(用分號隔開)
            Console.WriteLine("數字格式{0:n5}: ", 666666666);
            //百分號格式
            Console.WriteLine("百分號格式(不保留小數){0:p0} ", 0.55);
            // 16進制
            Console.WriteLine(" 16進制{0:x0} ", 12);
            // 0定位器此示例保留5位小數，如果小數部分小於5位，用0填充
            Console.WriteLine(" 0定位器{0:000.00000} ", 1222.133);
            //數字定位器
            Console.WriteLine("數字定位器{0:(#).###} ", 0200.0233000);
            //小數
            Console.WriteLine("小數保留一位{0:0.0} ", 12.222);
            //百分號的另一種寫法，注意小數的四捨五入
            Console.WriteLine("百分號的另一種寫法，注意小數的四捨五入{0:0%.00} ", 0.12345);
            Console.WriteLine(" \n\n ");
        }

        public void EX2()
        {
            //顯性使用IFomatProvider
            Console.WriteLine("顯性使用IFomatProvider的例子");
            //實例化numberFomatProvider對象
            NumberFormatInfo numberFomatProvider = new NumberFormatInfo();
            //設置該provider對於貨幣小數的顯示長度
            numberFomatProvider.CurrencyDecimalDigits = 10;
            //注意：我們可以使用C+數字形式來改變provider提供的格式
            Console.WriteLine(string.Format(numberFomatProvider, " provider設置的貨幣格式{0:C} ", 12));
            Console.WriteLine(string.Format(numberFomatProvider, " provider設置的貨幣格式被更改了：{0:C2} ", 12));
            Console.WriteLine(string.Format(numberFomatProvider, "默認百分號和小數形式{0:p2} ", 0.12));
            //將小數“.”換成"?"
            numberFomatProvider.PercentDecimalSeparator = " ? ";
            Console.WriteLine(string.Format(numberFomatProvider, " provider設置的百分號和小數形式{0:p2} ", 0.12));
            Console.ReadLine();
        }
    }
}
