using System;
using System.Linq;
using Core.Logic;
using Tamarack.Pipeline;

namespace Core.Pipeline
{
    public class TypeInterferenceStep : IFilter<Context, Context>
    {
        public Context Execute(Context context, Func<Context, Context> executeNext)
        {
            var typeInterference = new TypeInterference();

            var types = context.AST.Select(x => typeInterference.Visit(x)).ToList();
            
            context.Types = types;

            return context;
        }
    }
}