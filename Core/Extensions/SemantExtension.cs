using Core.Logic;
using Core.Tokens;

namespace Core.Extensions
{
    internal static class SemantExtension
    {
        public static bool IsInsideFunction(this Semant semant)
        {
            var current = semant?.Parent;
            while (current != null)
            {
                switch (current.Token)
                {
                    case FunctionDeclToken:
                        return true;
                    default:
                        current = current.Parent;
                        break;
                }
            }

            return false;
        }
    }
}