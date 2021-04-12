using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Core.Interfaces;

namespace Core.Utils
{
    public record Contour(Contour Parent, ImmutableDictionary<string, IToken> Table)
    {
        public static Contour Empty()
        {
            return new(null, ImmutableDictionary<string, IToken>.Empty);
        }
        
        public bool Lookup(string key, out IToken result)
        {
            if (Table.ContainsKey(key))
            {
                result = Table[key];
                return true;
            }

            if (Parent != null)
            {
                return Parent.Lookup(key, out result);
            }

            result = default;
            
            return false;
        }
        
        public Contour Append(string key, IToken value)
        {
            return new(Parent, Table.Add(key, value));
        }
        
        public Contour AppendMany(params (string key, IToken value)[] items)
        {
            var current = Table;
            
            foreach (var (key, value) in items)
            {
                current = current.Add(key, value);
            }

            return new Contour(Parent, current);
        }

        public Contour Push()
        {
            return new(this, ImmutableDictionary<string, IToken>.Empty);
        }

        [IndexerName("Item")]
        public IToken this[string key]
        {
            get
            {
                Lookup(key, out var result);

                return result;
            }
        }
    }
}