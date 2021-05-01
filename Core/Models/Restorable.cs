using System;
using System.Collections.Generic;
using Core.Interfaces;

namespace Core.Models
{
    public class Restorable<T> : IRestorable<T>
    {
        private Guid _current;

        private readonly IDictionary<Guid, T> _table = new Dictionary<Guid, T>();

        public Restorable(T instance)
        {
            _table[_current = Guid.NewGuid()] = instance;
        }

        public T Value => _table[_current];

        public Guid Mark()
        {
            return _current;
        }

        public void Restore(Guid id)
        {
            _current = id;
        }

        public void Apply(Func<T, T> apply)
        {
            var newKey = Guid.NewGuid();
            
            _table[newKey] = apply(_table[_current]);

            _current = newKey;
        }
    }
}