using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;
using Core.Tokens;
using FParsec;
using FParsec.CSharp;
using Microsoft.FSharp.Core; // extension functions (combinators & helpers)
using static FParsec.CSharp.PrimitivesCS; // combinator functions
using static FParsec.CSharp.CharParsersCS; // pre-defined parsers

namespace Core.Logic
{
    internal class Parser
    {
        public Parser()
        {
            FSharpFunc<CharStream<Unit>, Reply<IToken>> expressionP = null;
            var expressionRec = Rec(() => expressionP.AndL(WS));

            var nameP = Many1Chars(NoneOf(new[] {'"', ' ', '{', '}', '=', '(', ')', '\n', ';', ','})).Label("name");
            var variableP = nameP.Map(x => (IToken) new VariableToken(x));

            var stringP = Between(CharP('"'), ManyChars(NoneOf(new[] {'"'})), CharP('"')).Label("string")
                .Map(x => (IToken) new AtomicToken(x));
            var numberP = Float.Lbl("number").Map(x => (IToken) new AtomicToken(x));
            var boolP = StringP("true").Or(StringP("false")).Lbl("bool")
                .Map(x => (IToken) new AtomicToken(x == "true"));

            var atomicP = Choice(numberP, stringP, boolP).Label("atomic");

            var varDeclP = StringP("let").And_(WS1).AndRTry(nameP).AndL(WS).AndLTry(CharP('=')).AndL(WS)
                .AndTry(expressionRec)
                .Label("let")
                .Map(x => (IToken) new VarDeclToken(x.Item1, x.Item2));

            var assignP = nameP.AndL(WS).AndLTry(CharP('=')).AndL(WS).AndTry(expressionRec)
                .Label("assign")
                .Map(x => (IToken) new AssignToken(x.Item1, x.Item2));

            var commentP = StringP("//").AndR(ManyChars(NoneOf(new[] {'\n'}))).Label("comment").Map(x => (IToken) new CommentToken(x));

            var actualsP = SepBy('(', expressionRec, ')');
            var functionCallP = nameP.AndLTry(WS).AndTry(actualsP).Label("app")
                .Map(x => (IToken) new FunctionCallToken(x.Item1, x.Item2));

            var formalsP = SepBy('(', nameP, ')');
            var functionDeclP = StringP("def").AndTry_(WS1).AndRTry(nameP).AndL(WS).AndTry(formalsP).AndL(WS)
                .AndLTry(CharP('='))
                .AndL(WS)
                .AndTry(expressionRec)
                .Label("def")
                .Map(x => (IToken) new FunctionDeclToken(x.Item1.Item1,
                    x.Item1.Item2.Select(y => (IToken) new VariableToken(y)).ToList(), x.Item2));

            var conditionalP = StringP("if").And(WS).AndRTry(Between(CharP('(').And(WS), expressionRec, CharP(')')))
                .AndL(WS)
                .AndTry(expressionRec).And(WS).AndLTry(StringP("else")).AndL(WS).AndTry(expressionRec).Label("cond")
                .Map(x =>
                    (IToken) new CondToken(x.Item1.Item1, x.Item1.Item2, x.Item2));

            var blockExprP = SepBy('{', expressionRec, '}').Label("block").Map(x => (IToken) new BlockToken(x));

            expressionP = WS.AndR(Choice(varDeclP, assignP, atomicP, functionDeclP, conditionalP, functionCallP, variableP, blockExprP ));

            var tokensP = Many(Choice(commentP, expressionRec), sep: WS, canEndWithSep: true).Map(x => x.ToList());

            ParserP = WS.AndR(tokensP);
        }

        public FSharpFunc<CharStream<Unit>, Reply<List<IToken>>> ParserP { get; }

        public FSharpFunc<CharStream<Unit>, Reply<List<T>>> SepBy<T>(char start,
            FSharpFunc<CharStream<Unit>, Reply<T>> initial, char end)
        {
            var arrItems = Many(initial, sep: CharP(',').And(WS), canEndWithSep: false);
            var arrayP = Between(CharP(start).And(WS), arrItems, CharP(end))
                .Map(elems => elems.ToList());
            return arrayP;
        }
    }
}