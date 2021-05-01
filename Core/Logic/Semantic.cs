using System.Collections.Generic;
using System.Linq;
using Core.Abstracts;
using Core.Interfaces;
using Core.Models;
using Core.Tokens;
using Core.Utils;

namespace Core.Logic
{
    internal class Semantic : Visitor<Semant>
    {
        private readonly IRestorable<IContour> _contour = new Restorable<IContour>(new Contour());

        private readonly IRestorable<IToken> _parent = new Restorable<IToken>(null);
        
        public HashSet<string> Errors { get; }

        public Semantic()
        {
            Errors = new HashSet<string>();
        }

        public override Semant Visit(AssignToken assignToken)
        {
            var currentParent = _parent.Mark();
            var currentContour = _contour.Mark();

            _contour.Apply(_ => _.Push());
            _parent.Apply(_ => assignToken);

            if (!_contour.Value.Lookup(assignToken.Variable, out _))
            {
                Errors.Add($"Unbound variable {assignToken.Variable}");
            }

            Visit(assignToken.Body);

            _parent.Restore(currentParent);
            _contour.Restore(currentContour);

            return new Semant(assignToken, _parent.Value, _contour.Value);
        }

        public override Semant Visit(AtomicToken atomicToken)
        {
            return new(atomicToken, _parent.Value, _contour.Value);
        }

        public override Semant Visit(BlockToken blockToken)
        {
            var currentParent = _parent.Mark();
            var currentContour = _contour.Mark();

            _contour.Apply(_ => _.Push());
            _parent.Apply(_ => blockToken);

            foreach (var token in blockToken.Tokens)
            {
                Visit(token);
            }

            _parent.Restore(currentParent);
            _contour.Restore(currentContour);

            return new Semant(blockToken, _parent.Value, _contour.Value);
        }

        public override Semant Visit(FunctionDeclToken functionDeclToken)
        {
            _contour.Apply(_ => _.Append((functionDeclToken.Name, functionDeclToken)));
            
            var currentParent = _parent.Mark();
            var currentContour = _contour.Mark();

            _contour.Apply(_ => _.Push().Append(functionDeclToken.Formals.Select(x => (((VariableToken) x).Variable, x)).ToArray()));
            _parent.Apply(_ => functionDeclToken);
            
            Visit(functionDeclToken.Body);

            _parent.Restore(currentParent);
            _contour.Restore(currentContour);
            
            return new Semant(functionDeclToken, _parent.Value, _contour.Value);
        }

        public override Semant Visit(FunctionCallToken functionCallToken)
        {
            var currentParent = _parent.Mark();
            var currentContour = _contour.Mark();

            if (!_contour.Value.Lookup(functionCallToken.Name, out _))
            {
                Errors.Add($"{functionCallToken.Name} function to be invoked is unbound");
            }
            
            _parent.Apply(_ => functionCallToken);

            foreach (var actual in functionCallToken.Actuals)
            {
                var actualCurrentParent = _parent.Mark();
                var actualCurrentContour = _contour.Mark();
                
                Visit(actual);
                
                _parent.Restore(actualCurrentParent);
                _contour.Restore(actualCurrentContour);
            }
            
            _parent.Restore(currentParent);
            _contour.Restore(currentContour);
            
            return new Semant(functionCallToken, _parent.Value, _contour.Value);
        }

        public override Semant Visit(VarDeclToken varDeclToken)
        {
            var currentParent = _parent.Mark();
            var currentContour = _contour.Mark();

            if (_contour.Value.Lookup(varDeclToken.Variable, out _))
            {
                Errors.Add($"Redefinition/shadowing of variable {varDeclToken.Variable}");
            }
            
            _contour.Apply(_ => _.Append((varDeclToken.Variable, varDeclToken)));
            _parent.Apply(_ => varDeclToken);
            
            Visit(varDeclToken.Body);

            _parent.Restore(currentParent);
            _contour.Restore(currentContour);
            
            return new Semant(varDeclToken, _parent.Value, _contour.Value);
        }

        public override Semant Visit(CondToken condToken)
        {
            var currentParent = _parent.Mark();
            var currentContour = _contour.Mark();
            
            _parent.Apply(_ => condToken);
            Visit(condToken.Condition);
            _parent.Restore(currentParent);
            _contour.Restore(currentContour);
            
            _parent.Apply(_ => condToken);
            Visit(condToken.IfToken);
            _parent.Restore(currentParent);
            _contour.Restore(currentContour);
            
            _parent.Apply(_ => condToken);
            Visit(condToken.ElseToken);
            _parent.Restore(currentParent);
            _contour.Restore(currentContour);
            
            return new Semant(condToken, _parent.Value, _contour.Value);
        }

        public override Semant Visit(VariableToken variableToken)
        {
            if (!_contour.Value.Lookup(variableToken.Variable, out _))
            {
                Errors.Add($"Unbound variable {variableToken.Variable}");
            }

            return new Semant(variableToken, _parent.Value, _contour.Value);
        }

        public override Semant Visit(AddToken addToken)
        {
            var currentParent = _parent.Mark();
            var currentContour = _contour.Mark();
            
            _parent.Apply(_ => addToken);
            Visit(addToken.Left);
            _parent.Restore(currentParent);
            _contour.Restore(currentContour);
            
            _parent.Apply(_ => addToken);
            Visit(addToken.Right);
            _parent.Restore(currentParent);
            _contour.Restore(currentContour);

            return new Semant(addToken, _parent.Value, _contour.Value);
        }

        public override Semant Visit(IgnoredToken ignoredToken)
        {
            return new(ignoredToken, _parent.Value, _contour.Value);
        }
    }
}