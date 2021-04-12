using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core.Abstracts;
using Core.Interfaces;
using Core.Tokens;
using Core.Utils;

namespace Core.Logic
{
    public record Semant(Semant Parent, IToken Token, Contour Contour);

    public class Semantic : Visitor<IReadOnlyList<Semant>>
    {
        private Semant _parent;

        private Contour _contour = Contour.Empty();

        public override IReadOnlyList<Semant> Visit(AssignToken assignToken)
        {
            _parent = new Semant(_parent, assignToken, _contour);

            var bodyResult = Visit(assignToken.Body);

            return bodyResult.Concat(new[] {_parent}).ToList();
        }

        public override IReadOnlyList<Semant> Visit(AtomicToken atomicToken)
        {
            return new Semant[] { new(_parent, atomicToken, _contour) };
        }

        public override IReadOnlyList<Semant> Visit(BlockToken blockToken)
        {
            _parent = new Semant(_parent, blockToken, _contour);

            var bodyResult = blockToken.Tokens.SelectMany(Visit);

            return bodyResult.Concat(new[] {_parent}).ToList();
        }

        public override IReadOnlyList<Semant> Visit(FunctionDeclToken functionDeclToken)
        {
            var prevScope = _contour;

            _contour = _contour
                .Push()
                .Append(functionDeclToken.Name, functionDeclToken)
                .AppendMany(functionDeclToken.Formals.Select(x => (((VariableToken) x).Variable, x)).ToArray());

            _parent = new Semant(_parent, functionDeclToken, _contour);

            var bodyResult = Visit(functionDeclToken.Body);

            var result = bodyResult.Concat(new[] {_parent}).ToList();

            _contour = prevScope;

            return result;
        }

        public override IReadOnlyList<Semant> Visit(FunctionCallToken functionCallToken)
        {
            _parent = new Semant(_parent, functionCallToken, _contour);

            var bodyResult = functionCallToken.Actuals.SelectMany(Visit);

            return bodyResult.Concat(new[] {_parent}).ToList();
        }

        public override IReadOnlyList<Semant> Visit(VarDeclToken varDeclToken)
        {
            _contour = _contour.Append(varDeclToken.Variable, varDeclToken);
            _parent = new Semant(_parent, varDeclToken, _contour);

            var bodyResult = Visit(varDeclToken.Body);

            return bodyResult.Concat(new[] {_parent}).ToList();
        }

        public override IReadOnlyList<Semant> Visit(CondToken condToken)
        {
            _parent = new Semant(_parent, condToken, _contour);

            var bodyResult = Visit(condToken.Condition).Concat(Visit(condToken.IfToken)).Concat(Visit(condToken.ElseToken));

            return bodyResult.Concat(new[] {_parent}).ToList();
        }

        public override IReadOnlyList<Semant> Visit(VariableToken variableToken)
        {
            if (_contour[variableToken.Variable] == null)
            {
                throw new Exception("Unbound variable");
            }
            
            return new[] {new Semant(_parent, variableToken, _contour)};
        }
    }
}