using System;
using System.Collections.Generic;
using Core.Interfaces;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
}

namespace Core.Tokens
{
    public record Token(Guid Id) : IToken;

    public record IgnoredToken(Guid Id) : Token(Id);
    
    public record AssignToken(string Variable, IToken Body) : Token(Guid.NewGuid());
    public record CondToken(IToken Condition, IToken IfToken, IToken ElseToken) : Token(Guid.NewGuid());
    public record VarDeclToken(string Variable, IToken Body) : Token(Guid.NewGuid());
    public record FunctionDeclToken(string Name, List<IToken> Formals, IToken Body) : Token(Guid.NewGuid());
    public record BlockToken(List<IToken> Tokens) : Token(Guid.NewGuid());
    public record FunctionCallToken(string Name, List<IToken> Actuals) : Token(Guid.NewGuid());
    public record AtomicToken(IConvertible Value) : Token(Guid.NewGuid());
    public record VariableToken(string Variable) : Token(Guid.NewGuid());
    public record CommentToken(string Text) : IgnoredToken(Guid.NewGuid());
}