using System;
using System.Linq;
using Core.Abstracts;
using Core.Pipeline;
using Core.Tokens;

namespace Core.Logic
{
    internal class JsCodeGen : Visitor<string>
    {
        private readonly Context _context;

        public JsCodeGen(Context context)
        {
            _context = context;
        }

        public override string Visit(AssignToken assignToken)
        {
            var (variable, token) = assignToken;

            return $"{variable} = {Visit(token)}";
        }

        public override string Visit(AtomicToken atomicToken)
        {
            return $"{atomicToken.Value}";
        }

        public override string Visit(BlockToken blockToken)
        {
            var content = string.Join(Environment.NewLine, blockToken.Tokens
                .Select((x, i) =>
                {
                    var prefix = string.Empty;

                    if (i + 1 == blockToken.Tokens.Count)
                    {
                        prefix = "return ";
                    }

                    return prefix + Visit(x);
                }));
            
            return @$"(() => {{ {content} }})()";
        }

        public override string Visit(FunctionDeclToken functionDeclToken)
        {
            var (name, formals, body) = functionDeclToken;

            return $@"function {name}({string.Join(", ", formals.Select(x => ((VariableToken)x).Variable))}) {{ return {Visit(body)} }}";
        }

        public override string Visit(FunctionCallToken functionCallToken)
        {
            var (name, actuals) = functionCallToken;

            return $"{name}({string.Join(", ", actuals.Select(Visit))})";
        }

        public override string Visit(VarDeclToken varDeclToken)
        {
            var (variable, token) = varDeclToken;

            return $"var {variable} = {Visit(token)}";
        }

        public override string Visit(CondToken condToken)
        {
            var (condition, ifToken, elseToken) = condToken;
            
            return $"({Visit(condition)}) ? {Visit(ifToken)} : {Visit(elseToken)}";
        }

        public override string Visit(VariableToken variableToken)
        {
            return variableToken.Variable;
        }

        public override string Visit(AddToken addToken)
        {
            return $"{Visit(addToken.Left)} + {Visit(addToken.Right)}";
        }

        public override string Visit(MultiplyToken multiplyToken)
        {
            return $"{Visit(multiplyToken.Left)} * {Visit(multiplyToken.Right)}";
        }

        public override string Visit(SubtractToken subtractToken)
        {
            return $"{Visit(subtractToken.Left)} - {Visit(subtractToken.Right)}";
        }

        public override string Visit(DivideToken divideToken)
        {
            return $"{Visit(divideToken.Left)} / {Visit(divideToken.Right)}";
        }

        public override string Visit(IgnoredToken ignoredToken)
        {
            return string.Empty;
        }
    }
}