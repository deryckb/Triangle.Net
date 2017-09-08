using Triangle.Compiler.SyntaxTrees.Declarations;
using Triangle.Compiler.SyntaxTrees.Types;
using Triangle.Compiler.SyntaxTrees.Visitors;

namespace Triangle.Compiler.ContextualAnalyzer
{
    public partial class Checker : ITypeDenoterVisitor<Void, TypeDenoter>
    {
        // Type Denoters

        // Returns the expanded version of the TypeDenoter. Does not
        // use the given object.

        public TypeDenoter VisitAnyTypeDenoter(AnyTypeDenoter ast, Void arg)
        {
            return StandardEnvironment.AnyType;
        }


        public TypeDenoter VisitArrayTypeDenoter(ArrayTypeDenoter ast, Void arg)
        {
            ast.Type = ast.Type.Visit(this);
            CheckAndReportError(ast.IntegerLiteral.Value != 0, "arrays must not be empty",
                ast.IntegerLiteral);
            return ast;
        }

        public TypeDenoter VisitBoolTypeDenoter(BoolTypeDenoter ast, Void arg)
        {
            return StandardEnvironment.BooleanType;
        }

        public TypeDenoter VisitCharTypeDenoter(CharTypeDenoter ast, Void arg)
        {
            return StandardEnvironment.CharType;
        }

        public TypeDenoter VisitErrorTypeDenoter(ErrorTypeDenoter ast, Void arg)
        {
            return StandardEnvironment.ErrorType;
        }

        public TypeDenoter VisitSimpleTypeDenoter(SimpleTypeDenoter ast, Void arg)
        {

            var binding = ast.Identifier.Visit(this);
            var decl = binding as TypeDeclaration;
            if (decl != null)
            {
                return decl.Type;
            }

            ReportUndeclaredOrError(binding, ast.Identifier, "\"%\" is not a type identifier");
            return StandardEnvironment.ErrorType;
        }

        public TypeDenoter VisitIntTypeDenoter(IntTypeDenoter ast, Void arg)
        {
            return StandardEnvironment.IntegerType;
        }

        public TypeDenoter VisitRecordTypeDenoter(RecordTypeDenoter ast, Void arg)
        {
            ast.FieldType.Visit(this);
            return ast;
        }

        public TypeDenoter VisitMultipleFieldTypeDenoter(MultipleFieldTypeDenoter ast, Void arg)
        {
            ast.Type = ast.Type.Visit(this);
            ast.FieldType.Visit(this);
            return ast;
        }

        public TypeDenoter VisitSingleFieldTypeDenoter(SingleFieldTypeDenoter ast, Void arg)
        {
            ast.Type = ast.Type.Visit(this);
            return ast;
        }

    }
}