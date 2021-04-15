using System.Collections.Generic;
using Core.Interfaces;
using Core.Logic;
using Core.Utils;

namespace Core.Pipeline
{
    public class Context
    {
        public string Code { get; set; }
        
        public IToken[] AST { get; set; }
        
        public string Error { get; set; }
        
        public string JSCode { get; set; }
        
        public Semantic Semants { get; set; }
        
        public List<IType> Types { get; set; }
    }
}