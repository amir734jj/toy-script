using System;
using Core.Interfaces;

namespace Core.Models
{
    public record Token : IToken
    {
        // ReSharper disable once UnusedMember.Global
        public Guid Id => Guid.NewGuid();
    }
}