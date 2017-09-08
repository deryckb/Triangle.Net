using Triangle.Compiler.SyntaxTrees.Declarations;

namespace Triangle.Compiler.SyntaxTrees.Types
{
    public abstract class FieldTypeDenoter : TypeDenoter, IDeclaration
    {
        protected FieldTypeDenoter(SourcePosition position)
            : base(position)
        {
        }
    }
}