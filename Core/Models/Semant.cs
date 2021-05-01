using Core.Interfaces;

namespace Core.Models
{
    public record Semant(IToken Token, IToken Parent, IContour Contour);
}