using System;

namespace Core.Interfaces
{
    public interface IContour : ICloneable
    {
        bool Lookup(string key, out IToken result);

        IContour Push();

        IContour Append(params (string name, IToken value)[] pairs);
    }
}