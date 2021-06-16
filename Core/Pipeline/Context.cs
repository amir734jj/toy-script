using System.Collections.Generic;
using Core.Interfaces;
using Core.Logic;
using Core.Models;

namespace Core.Pipeline
{
    public class Context
    {
        public string Code { get; set; }
        
        public IToken[] AST { get; set; }
        
        public string Error { get; set; }
        
        public string JSCode { get; set; }
        
        public IDictionary<IToken, Semant> Semants { get; set; }
    }
}