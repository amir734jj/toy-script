using System;
using System.Collections.Generic;
using Core.Interfaces;
using Core.Models;

namespace Core.Tokens
{
    public record IgnoredToken : Token;

    public record AssignToken(string Variable, IToken Body) : Token;

    public record CondToken(IToken Condition, IToken IfToken, IToken ElseToken) : Token;

    public record VarDeclToken(string Variable, IToken Body) : Token;

    public record FunctionDeclToken(string Name, List<VariableToken> Formals, IToken Body) : Token;
    public record BlockToken(List<IToken> Tokens) : Token;

    public record FunctionCallToken(string Name, List<IToken> Actuals) : Token;

    public record AddToken(IToken Left, IToken Right) : Token;
    
    public record SubtractToken(IToken Left, IToken Right) : IgnoredToken;
    
    public record DivideToken(IToken Left, IToken Right) : IgnoredToken;
    
    public record MultiplyToken(IToken Left, IToken Right) : IgnoredToken;

    public record AtomicToken(IConvertible Value) : Token;

    public record VariableToken(string Variable) : Token;

    public record CommentToken(string Text) : IgnoredToken;
}