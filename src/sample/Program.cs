using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PastecSharp;

namespace sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var p = new Pastec("http://localhost:4212");
            Console.WriteLine($"Reply from {p.Host}: time={p.Ping()}ms");
        }
    }
}
