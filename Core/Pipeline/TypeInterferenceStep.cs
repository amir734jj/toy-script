using System;
using Core.Logic;
using Tamarack.Pipeline;

namespace Core.Pipeline
{
    public class TypeInterferenceStep : IFilter<Context, Context>
    {
        public Context Execute(Context context, Func<Context, Context> executeNext)
        {
            var typeInterference = new TypeInterference(context);

            typeInterference.Visit(context.AST);

            context.Types = typeInterference.Flatten;

            return executeNext(context);
        }
    }
}