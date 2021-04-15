using Core.Interfaces;
using Core.Logic;
using Core.Tokens;

namespace Core.Extensions
{
    internal static class SemantExtension
    {
        public static bool IsInsideFunction(this Semantic semant, IToken token)
        {
            while (token != null)
            {
                if (token is FunctionDeclToken)
                {
                    return true;
                }
                
                token = semant.LookupTable[token].Parent;
            }

            return false;
        }
    }
}