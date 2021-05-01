using System.Collections.Generic;
using System.Linq;
using Core.Abstracts;
using Core.Pipeline;
using Core.Tokens;

namespace Core.Logic
{
    public interface IType
    {
        IType Lub(IType other)
        {
            return this == other ? this : new AnyT();
        }
    }

    public record UnitT : IType;

    public record PrimitiveT : IType;

    public record FunctionT(List<IType> FormalsTypes, IType ReturnType) : IType;
    
    public record AnyT : IType;
    

    public class TypeInterference : Visitor<IType>
    {
        private readonly Context _context;

        public TypeInterference(Context context)
        {
            _context = context;
        }
        
        public override IType Visit(AssignToken assignToken)
        {
            return Visit(assignToken.Body);
        }

        public override IType Visit(AtomicToken atomicToken)
        {
            return new PrimitiveT();
        }

        public override IType Visit(BlockToken blockToken)
        {
            var types = blockToken.Tokens.Select(Visit).ToList();

            return types.Count == 0 ? new UnitT() : types.Last();
        }

        public override IType Visit(FunctionDeclToken functionDeclToken)
        {
            return new FunctionT(functionDeclToken.Formals.Select(x => (IType) new AnyT()).ToList(), Visit(functionDeclToken.Body));
        }

        public override IType Visit(FunctionCallToken functionCallToken)
        {
            // var functionDecl = _context.Semants.LookupTable[functionCallToken].contour[functionCallToken.Name];

            return new AnyT();
        }

        public override IType Visit(VarDeclToken varDeclToken)
        {
            return Visit(varDeclToken.Body);
        }

        public override IType Visit(CondToken condToken)
        {
            return Visit(condToken.IfToken).Lub(Visit(condToken.ElseToken));
        }

        public override IType Visit(VariableToken variableToken)
        {
            return new AnyT();
        }

        public override IType Visit(AddToken addToken)
        {
            return new PrimitiveT();
        }

        public override IType Visit(IgnoredToken ignoredToken)
        {
            return new UnitT();
        }
    }
}