using System;

namespace Core.Interfaces
{
    public interface IRestorable<T>
    {
        public T Value { get; }
        
        public Guid Mark();
        
        public void Restore(Guid id);

        void Apply(Func<T, T> apply);
    }
}