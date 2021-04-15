using Core.Interfaces;
using Core.Tokens;
using Core.Utils;

namespace Core.Extensions
{
    public static class TokenExtension
    {
        public static bool AnyBlockInside(this IToken token)
        {
            var visitor = new AdHocVisitor<bool>(t => t is BlockToken && t != token, (x, y) => x || y, false);

            return visitor.Visit(token);
        }
    }
}