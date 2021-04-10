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
            var (value) = atomicToken;
            
            return $"{value}";
        }

        public override string Visit(BlockToken blockToken)
        {
            var (tokens) = blockToken;
            
            return string.Join($";{Environment.NewLine}", tokens.Select(Visit));
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
            
            return $"if ({Visit(condition)}) {{ {Visit(ifToken)} }} else {{ {Visit(elseToken)} }}";
        }

        public override string Visit(ReturnToken returnToken)
        {
            var (body) = returnToken;

            return $"return {Visit(body)};";
        }
    }
}