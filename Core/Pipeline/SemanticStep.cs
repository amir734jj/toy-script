using System;
using System.Linq;
using Core.Logic;
using Tamarack.Pipeline;

namespace Core.Pipeline
{
    public class SemanticStep : IFilter<Context, Context>
    {
        public Context Execute(Context context, Func<Context, Context> executeNext)
        {
            var semantic = new Semantic();

            var semants = context.AST.SelectMany(x => semantic.Visit(x)).ToList();
            
            if (semants.All(x => x.Errors.Count == 0))
            {
                context.Semants = semants;
                
                return executeNext(context);
            }

            context.Error = "Semantic Check Step Failed: " + string.Join(',', semants.SelectMany(x => x.Errors));

            return context;
        }
    }
}