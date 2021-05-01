using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;

namespace Core.Utils
{
    internal record Context(Context Parent, IDictionary<string, IToken> Table);

    internal class Contour : IContour
    {
        private Context _context = new(null, new Dictionary<string, IToken>());
        
        public bool Lookup(string key, out IToken result)
        {
            var current = _context;

            while (current != null)
            {
                if (_context.Table.ContainsKey(key))
                {
                    result = _context.Table[key];
                    return true;
                }
                
                current = current.Parent;
            }

            result = default;
            
            return false;
        }

        public IContour Push()
        {
            return new Contour
            {
                _context = new Context(_context, new Dictionary<string, IToken>())
            };
        }

        public IContour Append(params (string name, IToken value)[] pairs)
        {
            var table = _context.Table
                .Concat(pairs.Select(x => new KeyValuePair<string, IToken>(x.name, x.value)))
                .ToDictionary(x => x.Key, x => x.Value);

            return new Contour
            {
                _context = _context with {Table = table}
            };
        }

        public object Clone()
        {
            return new Contour
            {
                _context = _context with { Table = _context.Table }
            };
        }
    }
}