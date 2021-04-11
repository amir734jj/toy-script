﻿using System;
using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;
using Core.Tokens;

namespace Core.Abstracts
{
    public abstract class Visitor<T>
    {
        public abstract T Visit(AssignToken assignToken);
        
        public abstract T Visit(AtomicToken atomicToken);
        
        public abstract T Visit(BlockToken blockToken);
        
        public abstract T Visit(FunctionDeclToken functionDeclToken);
        
        public abstract T Visit(FunctionCallToken functionCallToken);
        
        public abstract T Visit(VarDeclToken varDeclToken);
        
        public abstract T Visit(CondToken condToken);
        
        public abstract T Visit(VariableToken variableToken);

        public T Visit(IToken token)
        {
            return token switch
            {
                CondToken condToken => Visit(condToken),
                AssignToken assignToken => Visit(assignToken),
                AtomicToken atomicToken => Visit(atomicToken),
                BlockToken blockToken => Visit(blockToken),
                FunctionCallToken functionCallToken => Visit(functionCallToken),
                FunctionDeclToken functionDeclToken => Visit(functionDeclToken),
                VarDeclToken varDeclToken => Visit(varDeclToken),
                VariableToken variableToken => Visit(variableToken),
                IgnoredToken => default,
                _ => throw new ArgumentOutOfRangeException(nameof(token))
            };
        }

        public List<T> Visit(params IToken[] tokens)
        {
            return tokens.Select(Visit).Where(x => x != null).ToList();
        }
    }
}