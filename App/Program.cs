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
let x = 123
let y = 123
";

            var p = new Parser().ParserP.ParseString(str);
            
            Console.WriteLine("Hello World!");
        }
    }
}