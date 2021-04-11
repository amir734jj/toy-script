using System;
using System.Linq;
using Core.Tokens;

namespace Core.Logic
{
    public class JSCodeGen : Visitor<string>
    {
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
            return string.Join($";{Environment.NewLine}", blockToken.Tokens.Select(Visit));
        }

        public override string Visit(FunctionDeclToken functionDeclToken)
        {
            var (name, formals, body) = functionDeclToken;

            return $@"function {name}({string.Join(", ", formals)}) {{ {Visit(body)} }}";
        }

        public override string Visit(FunctionCallToken functionCallToken)
        {
            var (name, actuals) = functionCallToken;

            return $"{name}({string.Join(", ", actuals.Select(Visit))});";
        }

        public override string Visit(VarDeclToken varDeclToken)
        {
            var (variable, token) = varDeclToken;

            return $"var {variable} = {Visit(token)};";
        }

        public override string Visit(CondToken condToken)
        {
            var (condition, ifToken, elseToken) = condToken;
            
            return $"({Visit(condition)}) ? {Visit(ifToken)} : {Visit(elseToken)};";
        }
    }
}