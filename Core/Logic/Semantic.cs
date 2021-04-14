using System;
using System.Collections.Generic;
using System.Linq;
using Core.Abstracts;
using Core.Interfaces;
using Core.Tokens;
using Core.Utils;

namespace Core.Logic
{
    public record Semant(Semant Parent, IToken Token, Contour Contour, HashSet<string> Errors);

    internal class Semantic : Visitor<IReadOnlyList<Semant>>
    {
        private Semant _parent;

        private Contour _contour = Contour.Empty();

        private readonly HashSet<string> _errors = new();

        public override IReadOnlyList<Semant> Visit(AssignToken assignToken)
        {
            _parent = new Semant(_parent, assignToken, _contour, _errors);

            var bodyResult = Visit(assignToken.Body);

            return bodyResult.Concat(new[] {_parent}).ToList();
        }

        public override IReadOnlyList<Semant> Visit(AtomicToken atomicToken)
        {
            return new Semant[] { new(_parent, atomicToken, _contour, _errors) };
        }

        public override IReadOnlyList<Semant> Visit(BlockToken blockToken)
        {
            _parent = new Semant(_parent, blockToken, _contour, _errors);

            var bodyResult = blockToken.Tokens.SelectMany(Visit);

            return bodyResult.Concat(new[] {_parent}).ToList();
        }

        public override IReadOnlyList<Semant> Visit(FunctionDeclToken functionDeclToken)
        {
            var prevScope = _contour;   // backup contour

            _contour = _contour
                .Push()
                .Append(functionDeclToken.Name, functionDeclToken)
                .AppendMany(functionDeclToken.Formals.Select(x => (((VariableToken) x).Variable, x)).ToArray());

            _parent = new Semant(_parent, functionDeclToken, _contour, _errors);

            var bodyResult = Visit(functionDeclToken.Body);

            var result = bodyResult.Concat(new[] {_parent}).ToList();

            _contour = prevScope;   // restore contour

            return result;
        }

        public override IReadOnlyList<Semant> Visit(FunctionCallToken functionCallToken)
        {
            _parent = new Semant(_parent, functionCallToken, _contour, _errors);

            var bodyResult = functionCallToken.Actuals.SelectMany(Visit);

            return bodyResult.Concat(new[] {_parent}).ToList();
        }

        public override IReadOnlyList<Semant> Visit(VarDeclToken varDeclToken)
        {
            _contour = _contour.Append(varDeclToken.Variable, varDeclToken);
            _parent = new Semant(_parent, varDeclToken, _contour, _errors);

            var bodyResult = Visit(varDeclToken.Body);

            return bodyResult.Concat(new[] {_parent}).ToList();
        }

        public override IReadOnlyList<Semant> Visit(CondToken condToken)
        {
            _parent = new Semant(_parent, condToken, _contour, _errors);

            var bodyResult = Visit(condToken.Condition).Concat(Visit(condToken.IfToken)).Concat(Visit(condToken.ElseToken));

            return bodyResult.Concat(new[] {_parent}).ToList();
        }

        public override IReadOnlyList<Semant> Visit(VariableToken variableToken)
        {
            if (_contour[variableToken.Variable] == null)
            {
                _errors.Add("Unbound variable " + variableToken.Variable);
            }
            
            return new[] {new Semant(_parent, variableToken, _contour, _errors)};
        }

        public override IReadOnlyList<Semant> Visit(IgnoredToken ignoredToken)
        {
            return new[] {new Semant(_parent, ignoredToken, _contour, _errors)};
        }
    }
}