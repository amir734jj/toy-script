using System.Collections.Generic;
using Core.Interfaces;
using Core.Logic;

namespace Core.Pipeline
{
    public class Context
    {
        public string Code { get; set; }
        
        public List<IToken> AST { get; set; }
        
        public string Error { get; set; }
        
        public string JSCode { get; set; }
        
        public List<Semant> Semants { get; set; }
        public List<IType> Types { get; set; }
    }
}