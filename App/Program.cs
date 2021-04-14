using System;
using System.Linq;
using Core;
using Core.Logic;
using FParsec.CSharp;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            var code = @"
// Hello world
let y = 123
y = 456
def amir(x) = { if (x) x else { z = amir(y) } }
let z = amir(y)
";

            var result = new ToyScript().Analyze(code);

            Console.WriteLine(result.Error);
        }
    }
}