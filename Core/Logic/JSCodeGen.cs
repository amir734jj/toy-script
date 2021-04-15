using System;
using System.Linq;
using Core.Abstracts;
using Core.Extensions;
using Core.Interfaces;
using Core.Pipeline;
using Core.Tokens;

namespace Core.Logic
{
    public class JsCodeGen : Visitor<string>
    {
        private readonly Context _context;

        public JsCodeGen(Context context)
        {
            _context = context;
        }

        private string GetPrefix(IToken token)
        {
            var isInsideFunction = _context.Semants.IsInsideFunction(token, out var functionDecl);
            var exitNodes = _context.Semants.ExitNodes(functionDecl);

            return isInsideFunction && exitNodes.Contains(token) ? "return " : string.Empty;
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
            return string.Join($";{Environment.NewLine}", blockToken.Tokens
                .Select((x, i) => i + 1 == blockToken.Tokens.Count ? GetPrefix(x) + $"return {Visit(x)}" : Visit(x)));
        }

        public override string Visit(FunctionDeclToken functionDeclToken)
        {
            var (name, formals, body) = functionDeclToken;

            return $@"function {name}({string.Join(", ", formals.Select(x => ((VariableToken)x).Variable))} {{ {Visit(body)} }}";
        }

        public override string Visit(FunctionCallToken functionCallToken)
        {
            var (name, actuals) = functionCallToken;

            return $"{name}({string.Join(", ", actuals.Select(Visit))})";
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

        public override string Visit(VariableToken variableToken)
        {
            return GetPrefix(variableToken) + variableToken.Variable;
        }

        public override string Visit(IgnoredToken ignoredToken)
        {
            return string.Empty;
        }
    }
}