using System.Collections.Generic;
using System.Linq;
using Core.Abstracts;
using Core.Interfaces;
using Core.Tokens;

namespace Core.Logic
{
    public record Semant(Semant Parent, IToken Token);

    public class Semantic : Visitor<IReadOnlyList<Semant>>
    {
        private Semant _parent;

        public override IReadOnlyList<Semant> Visit(AssignToken assignToken)
        {
            _parent = new Semant(_parent, assignToken);

            var bodyResult = Visit(assignToken.Body);

            return bodyResult.Concat(new[] {_parent}).ToList();
        }

        public override IReadOnlyList<Semant> Visit(AtomicToken atomicToken)
        {
            return new Semant[] { new(_parent, atomicToken) };
        }

        public override IReadOnlyList<Semant> Visit(BlockToken blockToken)
        {
            _parent = new Semant(_parent, blockToken);

            var bodyResult = blockToken.Tokens.SelectMany(Visit);

            return bodyResult.Concat(new[] {_parent}).ToList();
        }

        public override IReadOnlyList<Semant> Visit(FunctionDeclToken functionDeclToken)
        {
            _parent = new Semant(_parent, functionDeclToken);

            var bodyResult = Visit(functionDeclToken.Body);

            return bodyResult.Concat(new[] {_parent}).ToList();
        }

        public override IReadOnlyList<Semant> Visit(FunctionCallToken functionCallToken)
        {
            _parent = new Semant(_parent, functionCallToken);

            var bodyResult = functionCallToken.Actuals.SelectMany(Visit);

            return bodyResult.Concat(new[] {_parent}).ToList();
        }

        public override IReadOnlyList<Semant> Visit(VarDeclToken varDeclToken)
        {
            _parent = new Semant(_parent, varDeclToken);

            var bodyResult = Visit(varDeclToken.Body);

            return bodyResult.Concat(new[] {_parent}).ToList();
        }

        public override IReadOnlyList<Semant> Visit(CondToken condToken)
        {
            _parent = new Semant(_parent, condToken);

            var bodyResult = Visit(condToken.Condition).Concat(Visit(condToken.IfToken)).Concat(Visit(condToken.ElseToken));

            return bodyResult.Concat(new[] {_parent}).ToList();
        }

        public override IReadOnlyList<Semant> Visit(VariableToken variableToken)
        {
            return new[] {new Semant(_parent, variableToken)};
        }
    }
}