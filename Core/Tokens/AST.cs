using System;
using System.Collections.Generic;
using Core.Interfaces;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
}

namespace Core.Tokens
{
    public record AssignToken(string Variable, IToken Body) : IToken;
    public record CondToken(IToken Condition, IToken IfToken, IToken ElseToken) : IToken;
    public record VarDeclToken(string Variable, IToken Body) : IToken;
    public record FunctionDeclToken(string Name, List<string> Formals, IToken Body) : IToken;
    public record BlockToken(List<IToken> Tokens) : IToken;
    public record FunctionCallToken(string Name, List<IToken> Actuals) : IToken;
    public record AtomicToken(IConvertible Value) : IToken;
    public record VariableToken(string Variable) : IToken;
}