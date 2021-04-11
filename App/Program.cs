using System;
using System.Linq;
using Core.Logic;
using FParsec.CSharp;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            var str = @"
// Hello world
let y = 123
y = 456
def amir(x) = { if (x) x else { z = amir(y) } }
let z = amir(y)
";

            var p = new Parser().ParserP.ParseString(str);
            var codeGen = new JsCodeGen();
            
            Console.WriteLine(string.Join(Environment.NewLine, codeGen.Visit(p.Result.ToArray())));

            var semant = new Semantic();
            var re = semant.Visit(p.Result.ToArray()).SelectMany(x => x).ToList();
            var amir = re.Select(x => x.Token).GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.FirstOrDefault()).ToList();

            var dups = re.GroupBy(x => x).Where(x => x.Count() > 1).ToDictionary(x => x.Key, x => x.ToList());

            Console.WriteLine(re);
        }
    }
}