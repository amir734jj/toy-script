using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Core.Abstracts;
using Core.Models;
using Core.Tokens;

namespace Core.Logic
{
    public class Interpreter : Visitor<Expression>
    {
        private readonly Dictionary<string, Expression> _ctx = new();

        private GotoExpression _gotoExpression = Expression.Goto(Expression.Label());

        public override Expression Visit(AssignToken assignToken)
        {
            return Expression.Assign(Expression.Constant(assignToken.Variable), Visit(assignToken.Body));
        }

        public override Expression Visit(AtomicToken atomicToken)
        {
            return Expression.Constant(atomicToken.Value);
        }

        public override Expression Visit(BlockToken blockToken)
        {
            return Expression.Block(blockToken.Tokens.Select(((token, i) => i + 1 == blockToken.Tokens.Count ? Expression.Return(Visit(token)) : Visit(token))));
        }

        public override Expression Visit(FunctionDeclToken functionDeclToken)
        {
            return _ctx[functionDeclToken.Name] = Expression.Lambda(Visit(functionDeclToken.Body),
                functionDeclToken.Formals
                    .Select(x => _ctx[x.Variable] = Expression.Parameter(typeof(object), x.Variable))
                    .Cast<ParameterExpression>()
                    .ToArray());
        }

        public override Expression Visit(FunctionCallToken functionCallToken)
        {
            ((LambdaExpression) _ctx[functionCallToken.Name]).Compile().DynamicInvoke(functionCallToken.Actuals);
        }

        public override Expression Visit(VarDeclToken varDeclToken)
        {
           return  _ctx[varDeclToken.Variable] = Visit(varDeclToken.Body);
        }

        public override Expression Visit(CondToken condToken)
        {
            return Expression.Condition(
                Visit(condToken.Condition),
                Visit(condToken.IfToken),
                Visit(condToken.ElseToken));
        }

        public override Expression Visit(VariableToken variableToken)
        {
            return _ctx[variableToken.Variable];
        }

        public override Expression Visit(AddToken addToken)
        {
            return Expression.Add(Visit(addToken.Left), Visit(addToken.Right));
        }

        public override Expression Visit(MultiplyToken multiplyToken)
        {
            return Expression.Multiply(Visit(multiplyToken.Left), Visit(multiplyToken.Right));
        }

        public override Expression Visit(SubtractToken subtractToken)
        {
            return Expression.Subtract(Visit(subtractToken.Left), Visit(subtractToken.Right));
        }

        public override Expression Visit(DivideToken divideToken)
        {
            return Expression.Divide(Visit(divideToken.Left), Visit(divideToken.Right));
        }

        public override Expression Visit(IgnoredToken ignoredToken)
        {
            return Expression.Empty();
        }
        
        private static Type StaticFuncType(int count)
        {
            return count switch
            {
                0 => typeof(Func<>).MakeGenericType(typeof(object)),
                1 => typeof(Func<,>).MakeGenericType(typeof(object), typeof(object)),
                2 => typeof(Func<,,>).MakeGenericType(typeof(object), typeof(object), typeof(object)),
                3 => typeof(Func<,,,>).MakeGenericType(typeof(object), typeof(object), typeof(object), typeof(object)),
                _ => throw new Exception(
                    $"function with {count} many formals is not supported")
            };
        }
    }
}