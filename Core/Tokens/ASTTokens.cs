using System.Collections.Generic;
using Core.Interfaces;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
}

namespace Core.Tokens
{
    public record AssignToken(string Variable, IToken Body) : IToken;
    public record VarDeclToken(string Variable, IToken Body) : IToken;
    public record ReturnToken(IToken Body) : IToken;
    public record FunctionDeclToken(List<string> Formals, IToken Body) : IToken;
    public record BlockToken(List<IToken> Tokens) : IToken;
    public record FunctionCallToken(string Name, List<IToken> Actuals) : IToken;
    public record AtomicToken<T>(T Value) : IToken where T: struct;
}