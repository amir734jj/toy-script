using System;
using System.Linq;
using Core.Abstracts;
using Core.Interfaces;
using Core.Tokens;

namespace Core.Utils
{
    public class AdHocVisitor<T> : Visitor<T>
    {
        private readonly Func<IToken, T> _adHocVisitor;

        private readonly Func<T, T, T> _combinator;

        private readonly T _defaultValue;

        public AdHocVisitor(Func<IToken, T> adHocVisitor, Func<T, T, T> combinator, T defaultValue)
        {
            _adHocVisitor = adHocVisitor;
            _combinator = combinator;
            _defaultValue = defaultValue;
        }

        public override T Visit(AssignToken assignToken)
        {
            return _combinator(_adHocVisitor(assignToken), Visit(assignToken.Body));
        }

        public override T Visit(AtomicToken atomicToken)
        {
            return _adHocVisitor(atomicToken);
        }

        public override T Visit(BlockToken blockToken)
        {
            return _combinator(_adHocVisitor(blockToken), blockToken.Tokens.Select(Visit).Aggregate(_defaultValue, _combinator));
        }

        public override T Visit(FunctionDeclToken functionDeclToken)
        {
            return _combinator(_adHocVisitor(functionDeclToken), Visit(functionDeclToken.Body));
        }

        public override T Visit(FunctionCallToken functionCallToken)
        {
            return _combinator(_adHocVisitor(functionCallToken),
                functionCallToken.Actuals.Select(Visit).Aggregate(_defaultValue, _combinator));
        }

        public override T Visit(VarDeclToken varDeclToken)
        {
            return _combinator(_adHocVisitor(varDeclToken), Visit(varDeclToken.Body));
        }

        public override T Visit(CondToken condToken)
        {
            return new[]
            {
                _adHocVisitor(condToken),
                Visit(condToken.Condition),
                Visit(condToken.IfToken),
                Visit(condToken.ElseToken)
            }.Aggregate(_defaultValue, _combinator);
        }

        public override T Visit(VariableToken variableToken)
        {
            return _adHocVisitor(variableToken);
        }

        public override T Visit(AddToken addToken)
        {
            return _adHocVisitor(addToken);
        }

        public override T Visit(IgnoredToken ignoredToken)
        {
            return _adHocVisitor(ignoredToken);
        }
    }
}