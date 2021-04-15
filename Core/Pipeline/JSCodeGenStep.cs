using System;
using Core.Logic;
using Tamarack.Pipeline;

namespace Core.Pipeline
{
    public class JSCodeGenStep : IFilter<Context, Context>
    {
        public Context Execute(Context context, Func<Context, Context> executeNext)
        {
            var codeGen = new JsCodeGen(context);

            context.JSCode = string.Join(Environment.NewLine, codeGen.Visit(context.AST));

            return executeNext(context);
        }
    }
}