using System;
using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;
using Core.Logic;
using Core.Tokens;

namespace Core.Extensions
{
    internal static class SemantExtension
    {
        public static bool IsInsideFunction(this Semantic semant, IToken token, out IToken target)
        {
            return semant.IsInsideOf(token, x => x is FunctionDeclToken, out target);
        }

        public static List<IToken> ExitNodes(this Semantic semantic, IToken token)
        {
            return semantic.LookupTable.Where(x => x.Value.Parent == token)
                .Select(x => x.Key)
                .SelectMany(x =>
                {
                    var re = semantic.ExitNodes(x);
                    return re.Count == 0 ? new List<IToken> {x} : re;
                }).ToList();
        }
        
        public static bool IsInsideOf(this Semantic semant, IToken token,Func<IToken, bool> test, out IToken target)
        {
            while (token != null)
            {
                if (test(token))
                {
                    target = token;
                    return true;
                }
                
                token = semant.LookupTable[token].Parent;
            }

            target = null;
            return false;
        }
    }
}