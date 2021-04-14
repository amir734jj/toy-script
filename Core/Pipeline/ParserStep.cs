using System;
using Core.Logic;
using FParsec;
using FParsec.CSharp;
using Tamarack.Pipeline;

namespace Core.Pipeline
{
    internal class ParserStep : IFilter<Context, Context>
    {
        public Context Execute(Context context, Func<Context, Context> executeNext)
        {
            var parser = new Parser();

            var (status, result, error) = parser.ParserP.ParseString(context.Code);

            if (status == ReplyStatus.Ok)
            {
                context.AST = result;
                
                return executeNext(context);
            }

            context.Error = "Parser Step Failed: " + error;

            return context;
        }
    }
}