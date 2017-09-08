using Triangle.Compiler.SyntaxTrees.Terminals;
using Triangle.Compiler.SyntaxTrees.Visitors;

namespace Triangle.Compiler.SyntaxTrees.Types
{
    public class SingleFieldTypeDenoter : FieldTypeDenoter
    {
        Identifier _identifier;

        TypeDenoter _type;

        public SingleFieldTypeDenoter(Identifier identifier, TypeDenoter type,
                SourcePosition position)
            : base(position)
        {
            _identifier = identifier;
            _type = type;
        }

        public Identifier Identifier { get { return _identifier; } }

        public TypeDenoter Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public override int Size { get { return _type.Size; } }

        public override TResult Visit<TArg, TResult>(ITypeDenoterVisitor<TArg, TResult> visitor, TArg arg)
        {
            return visitor.VisitSingleFieldTypeDenoter(this, arg);
        }
    }
}