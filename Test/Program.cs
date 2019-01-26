using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
namespace Test {
    class Program {
        static void Main(string[] args) {
            var s = "1\\n2\\n3\\n";
            Console.WriteLine(s);
            Console.WriteLine(string.Format(s + "4", new object[] { }));
            Thread.Sleep(2000);
        }
    }

    class A {
        public int value;
    }
    class B : A{
        public int v2;
        public void write() {
            Console.WriteLine($"A.value = {base.value}");
            Console.WriteLine($"B.value = {this.value}");
            Thread.Sleep(3000);
        }
    }
}
