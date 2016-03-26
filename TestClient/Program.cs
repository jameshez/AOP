using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient
{
    class Program : ObjectWithAspects
    {
        public static void MethodOne()
        {
            Console.Write("In Method One");
        }

        public static void MethodTwo()
        {
            Console.Write("In Method One");
        }

        static void Main(string[] args)
        {
            try
            {
                ButtonMgr bm = new ButtonMgr();
                var r = bm.AddButton(1);
                var rs = bm.MultiParametersMethod(21, "sdss", new List<int>() { 1, 2, 34, 5 });
                var r1 = ButtonMgr.k;
            }
            catch (Exception EX)
            {
                Console.WriteLine(EX.ToString());
            }
            Console.Read();
        }


    }
}
