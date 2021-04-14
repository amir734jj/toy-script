using System;
using Core.Pipeline;
using Tamarack.Pipeline;

namespace Core
{
    public class ToyScript
    {
        public Context Analyze(string code)
        {
            var pipeline = new Pipeline<Context, Context>()
                .Add<ParserStep>()
                .Add<SemanticStep>()
                .Add<TypeInterferenceStep>()
                .Finally(ctx => ctx);

            var result = pipeline.Execute(new Context {Code = code});

            if (result.Error != null)
            {
                throw new Exception(result.Error);
            }

            return result;
        }

        public string Compile(string code)
        {
            var pipeline = new Pipeline<Context, Context>()
                .Add<ParserStep>()
                .Add<SemanticStep>()
                .Add<TypeInterferenceStep>()
                .Add<JSCodeGenStep>()
                .Finally(ctx => ctx);

            var result = pipeline.Execute(new Context {Code = code});

            if (result.Error != null)
            {
                throw new Exception(result.Error);
            }

            return result.JSCode;
        }
        
        public T Interpret<T>(string code)
        {
            throw new NotImplementedException();
        }
    }
}