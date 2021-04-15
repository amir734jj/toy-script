using System;
using Core;

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
let z = -1
def amir(x, y) = { if (x) x else { z = amir(y) } }
z = amir(y)
";

            var result = new ToyScript().Analyze(code);

            Console.WriteLine(result.Error);
        }
    }
}