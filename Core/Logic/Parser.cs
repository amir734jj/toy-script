using Core.Interfaces;
using FParsec.CSharp; // extension functions (combinators & helpers)
using static FParsec.CSharp.PrimitivesCS; // combinator functions
using static FParsec.CSharp.CharParsersCS; // pre-defined parsers

namespace Core.Logic
{
    public class Parser
    {
        public Parser()
        {
            var nameP = Many1Chars(NoneOf(new []{ '"', ' ', '{', '}'})).Label("name");

            var variableP = Many1Chars(CharP(char.IsLetter)).Label("variable").Map(x => (IToken) new ParameterToken(x));
            var stringP = Between(CharP('"'), ManyChars(NoneOf(new []{'"'})).Label("stringPValue"), CharP('"')).Label("string")
                .Map(x => (IToken) new StringToken(x));

            var numberP = Int.Lbl("number").Map(x => (IToken) new NumberToken(x));

            var nullP = StringP("null").Lbl("null").Return((IToken) new NullToken());

            var boolP = StringP("true").Or(StringP("false")).Lbl("bool")
                .Map(x => (IToken) new BoolLiteral(bool.Parse(x)));

            var atomicP = Choice(numberP, stringP, nullP, boolP, variableP).Label("atomic");
            
            var commentP = StringP(";;").AndR(ManyChars(NoneOf(new[] {'\n'}))).Map(x => (IToken) new Comment(x));
        }
    }
}