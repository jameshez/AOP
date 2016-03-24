using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient
{
    class Program: ObjectWithAspects
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
            ButtonMgr bm = new ButtonMgr();
            var r = bm.AddButton(1);
        }


    }
}
