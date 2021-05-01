using System;
using Core.Logic;
using Tamarack.Pipeline;

namespace Core.Pipeline
{
    public class SemanticStep : IFilter<Context, Context>
    {
        public Context Execute(Context context, Func<Context, Context> executeNext)
        {
            var semantic = new Semantic();

            semantic.Visit(context.AST);
            
            if (semantic.Errors.Count > 0)
            {
                context.Semants = semantic.Flatten;
                
                return executeNext(context);
            }

            context.Error = "Semantic Check Step Failed: " + string.Join(", ", semantic.Errors);

            return context;
        }
    }
}