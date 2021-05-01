using System;
using Core.Interfaces;
using Core.Models;
using Core.Tokens;

namespace Core.Extensions
{
    internal static class SemantExtension
    {
        public static bool IsInsideFunction(this Semant semant, IToken token, out IToken target)
        {
            return semant.IsInsideOf(token, x => x is FunctionDeclToken, out target);
        }

        public static bool IsInsideOf(this Semant semant, IToken token, Func<IToken, bool> test, out IToken target)
        {
            while (token != null)
            {
                if (test(token))
                {
                    target = token;
                    return true;
                }

                token = semant.Parent;
            }

            target = null;
            return false;
        }
    }
}