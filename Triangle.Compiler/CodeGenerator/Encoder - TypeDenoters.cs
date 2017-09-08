using Triangle.AbstractMachine;
using Triangle.Compiler.CodeGenerator.Entities;
using Triangle.Compiler.SyntaxTrees.Types;
using Triangle.Compiler.SyntaxTrees.Visitors;

namespace Triangle.Compiler.CodeGenerator
{
    public partial class Encoder : ITypeDenoterVisitor<Frame, int>
    // Type Denoters
    {
        public int VisitAnyTypeDenoter(AnyTypeDenoter ast, Frame frame)
        {
            return 0;
        }

        public int VisitArrayTypeDenoter(ArrayTypeDenoter ast, Frame frame)
        {
            int typeSize;
            if (ast.Entity == null)
            {
                var elemSize = ast.Type.Visit(this, null);
                typeSize = ast.IntegerLiteral.Value * elemSize;
                ast.Entity = new TypeRepresentation(typeSize);
                Encoder.WriteTableDetails(ast);
            }
            else
            {
                typeSize = ast.Entity.Size;
            }
            return typeSize;
        }

        public int VisitBoolTypeDenoter(BoolTypeDenoter ast, Frame frame)
        {
            if (ast.Entity == null)
            {
                ast.Entity = new TypeRepresentation(Machine.BooleanSize);
                Encoder.WriteTableDetails(ast);
            }
            return Machine.BooleanSize;
        }

        public int VisitCharTypeDenoter(CharTypeDenoter ast, Frame frame)
        {
            if (ast.Entity == null)
            {
                ast.Entity = new TypeRepresentation(Machine.IntegerSize);
                Encoder.WriteTableDetails(ast);
            }
            return Machine.IntegerSize;
        }

        public int VisitErrorTypeDenoter(ErrorTypeDenoter ast, Frame frame)
        {
            return 0;
        }

        public int VisitSimpleTypeDenoter(SimpleTypeDenoter ast, Frame frame)
        {
            return 0;
        }

        public int VisitIntTypeDenoter(IntTypeDenoter ast, Frame frame)
        {
            if (ast.Entity == null)
            {
                ast.Entity = new TypeRepresentation(Machine.IntegerSize);
                Encoder.WriteTableDetails(ast);
            }
            return Machine.IntegerSize;
        }

        public int VisitRecordTypeDenoter(RecordTypeDenoter ast, Frame frame)
        {
            int typeSize;
            if (ast.Entity == null)
            {
                typeSize = ast.FieldType.Visit(this, Frame.Initial);
                ast.Entity = new TypeRepresentation(typeSize);
                Encoder.WriteTableDetails(ast);
            }
            else
            {
                typeSize = ast.Entity.Size;
            }
            return typeSize;
        }

        public int VisitMultipleFieldTypeDenoter(MultipleFieldTypeDenoter ast, Frame frame)
        {
            var offset = frame.Size;
            int fieldSize;

            if (ast.Entity == null)
            {
                fieldSize = ast.Type.Visit(this, null);
                ast.Entity = new Field(fieldSize, offset);
                Encoder.WriteTableDetails(ast);
            }
            else
            {
                fieldSize = ast.Entity.Size;
            }

            int offset1 = offset + fieldSize;
            int recSize = ast.FieldType.Visit(this, Frame.Initial.Push(offset1));
            return fieldSize + recSize;
        }

        public int VisitSingleFieldTypeDenoter(SingleFieldTypeDenoter ast, Frame frame)
        {
            int offset = frame.Size;
            int fieldSize;

            if (ast.Entity == null)
            {
                fieldSize = ast.Type.Visit(this, null);
                ast.Entity = new Field(fieldSize, offset);
                Encoder.WriteTableDetails(ast);
            }
            else
            {
                fieldSize = ast.Entity.Size;
            }

            return fieldSize;
        }
    }
}