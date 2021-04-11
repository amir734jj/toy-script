using System;
using Core.Logic;
using FParsec.CSharp;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            var str = @"
let y = 123
y = 456
def amir(x) = { if (x) x else { z = amir(x) } }
let z = amir(y)
";

            var p = new Parser().ParserP.ParseString(str);
            
            Console.WriteLine("Hello World!");
        }
    }
}