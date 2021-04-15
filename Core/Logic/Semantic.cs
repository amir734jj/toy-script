using System.Collections.Generic;
using System.Linq;
using Core.Abstracts;
using Core.Interfaces;
using Core.Tokens;
using Core.Utils;

namespace Core.Logic
{
    public class Semantic : Visitor<Semantic>
    {
        private Contour _contour = Contour.Empty();
        
        private IToken _currentNode;

        public Semantic()
        {
            LookupTable = new Dictionary<IToken, (IToken Parent, Contour contour)>();
            Errors = new HashSet<string>();
        }

        public override Semantic Visit(AssignToken assignToken)
        {
            var currentContour = _contour;
            var currentParentNode = _currentNode;

            _contour = _contour.Push();
            _currentNode = assignToken;

            var (variable, token) = assignToken;
            if (_contour[variable] == null)
            {
                Errors.Add($"Unbound variable {variable}");
            }

            Visit(token);

            _contour = currentContour;
            _currentNode = currentParentNode;

            LookupTable[assignToken] = (_currentNode, _contour);

            return this;
        }

        public override Semantic Visit(AtomicToken atomicToken)
        {
            LookupTable[atomicToken] = (_currentNode, _contour);
            
            return this;
        }

        public override Semantic Visit(BlockToken blockToken)
        {
            var currentContour = _contour;
            var currentParentNode = _currentNode;

            _contour = _contour.Push();
            _currentNode = blockToken;

            foreach (var token in blockToken.Tokens)
            {
                Visit(token);
            }

            _contour = currentContour;
            _currentNode = currentParentNode;

            LookupTable[blockToken] = (_currentNode, _contour);

            return this;
        }

        public override Semantic Visit(FunctionDeclToken functionDeclToken)
        {
            _contour = _contour.Append(functionDeclToken.Name, functionDeclToken);
            
            var currentContour = _contour;
            var currentParentNode = _currentNode;

            _contour = _contour.Push();
            _currentNode = functionDeclToken;
            
            _contour = _contour
                .Push()
                .AppendMany(functionDeclToken.Formals.Select(x => (((VariableToken) x).Variable, x)).ToArray());
            
            Visit(functionDeclToken.Body);

            _contour = currentContour;
            _currentNode = currentParentNode;

            LookupTable[functionDeclToken] = (_currentNode, _contour);

            return this;
        }

        public override Semantic Visit(FunctionCallToken functionCallToken)
        {
            var currentContour = _contour;
            var currentParentNode = _currentNode;

            if (_contour[functionCallToken.Name] == null)
            {
                Errors.Add($"{functionCallToken.Name} function to be invoked is unbound");
            }

            foreach (var actual in functionCallToken.Actuals)
            {
                _contour = _contour.Push();
                Visit(actual);
                _contour = currentContour;
            }
            
            _contour = currentContour;
            _currentNode = currentParentNode;

            LookupTable[functionCallToken] = (_currentNode, _contour);

            return this;
        }

        public override Semantic Visit(VarDeclToken varDeclToken)
        {
            var currentContour = _contour;
            var currentParentNode = _currentNode;

            _contour = _contour.Push();
            _currentNode = varDeclToken;

            var (variable, token) = varDeclToken;

            Visit(token);

            _contour = currentContour;
            _currentNode = currentParentNode;

            if (_contour[variable] != null)
            {
                Errors.Add($"Redefinition of variable {variable}");
            }
            else
            {
                _contour = _contour.Append(variable, varDeclToken);
            }
            
            LookupTable[varDeclToken] = (_currentNode, _contour);

            return this;
        }

        public override Semantic Visit(CondToken condToken)
        {
            var currentContour = _contour;
            var currentParentNode = _currentNode;

            _currentNode = condToken;
            
            _contour = _contour.Push();
            Visit(condToken.Condition);
            _contour = currentContour;
            
            _contour = _contour.Push();
            Visit(condToken.IfToken);
            _contour = currentContour;
            
            _contour = _contour.Push();
            Visit(condToken.ElseToken);
            _contour = currentContour;
            
            _currentNode = currentParentNode;
            LookupTable[condToken] = (_currentNode, _contour);

            return this;
        }

        public override Semantic Visit(VariableToken variableToken)
        {
            LookupTable[variableToken] = (_currentNode, _contour);
            
            if (_contour[variableToken.Variable] == null)
            {
                Errors.Add($"Unbound variable {variableToken.Variable}");
            }

            return this;
        }

        public override Semantic Visit(IgnoredToken ignoredToken)
        {
            LookupTable[ignoredToken] = (_currentNode, _contour);
            
            return this;
        }

        public IDictionary<IToken, (IToken Parent, Contour contour)> LookupTable { get; }
        
        public HashSet<string> Errors { get; set; }
    }
}