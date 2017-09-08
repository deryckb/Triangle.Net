using Triangle.Compiler.SyntaxTrees.Declarations;
using Triangle.Compiler.SyntaxTrees.Terminals;
using Triangle.Compiler.SyntaxTrees.Types;
using Triangle.Compiler.SyntaxTrees.Visitors;
using Triangle.Compiler.SyntaxTrees.Vnames;

namespace Triangle.Compiler.ContextualAnalyzer
{
    public partial class Checker : IVnameVisitor<Void, TypeDenoter>
    {
        public TypeDenoter VisitDotVname(DotVname ast, Void arg)
        {
            ast.Type = null;
            var vnameType = ast.Vname.Visit(this);
            var record = vnameType as RecordTypeDenoter;
            if (record != null)
            {
                ast.Type = CheckFieldIdentifier(record.FieldType, ast.Identifier);
                CheckAndReportError(ast.Type != StandardEnvironment.ErrorType,
                    "no field \"%\" in this record type", ast.Identifier);
            }
            else
            {
                ReportError("record expected here", ast.Vname);
            }
            return ast.Type;
        }

        public TypeDenoter VisitSimpleVname(SimpleVname ast, Void arg)
        {
            var binding = ast.Identifier.Visit(this);
            if (binding is IConstantDeclaration constant)
            {
                return ast.Type = constant.Type;
            }

            if (binding is IVariableDeclaration variable)
            {
                return ast.Type = variable.Type;
            }

            ReportUndeclaredOrError(binding, ast.Identifier, "\"%\" is not a const or var identifier");
            return ast.Type = StandardEnvironment.ErrorType;
        }

        public TypeDenoter VisitSubscriptVname(SubscriptVname ast, Void arg)
        {
            var vnameType = ast.Vname.Visit(this);
            var expressionType = ast.Expression.Visit(this);
            if (vnameType != StandardEnvironment.ErrorType)
            {
                if (vnameType is ArrayTypeDenoter arrayType)
                {
                    CheckAndReportError(expressionType.Equals(StandardEnvironment.IntegerType),
                        "Integer expression expected here", ast.Expression);
                    ast.Type = arrayType.Type;
                }
                else
                {
                    ReportError("array expected here", ast.Vname);
                }
            }
            return ast.Type;
        }

        TypeDenoter CheckFieldIdentifier(FieldTypeDenoter ast, Identifier identifier)
        {
            if (ast is MultipleFieldTypeDenoter mft)
            {
                if (mft.Identifier.Equals(identifier))
                {
                    identifier.Declaration = ast;
                    return mft.Type;
                }
                return CheckFieldIdentifier(mft.FieldType, identifier);
            }

            if (ast is SingleFieldTypeDenoter sft)
            {
                if (sft.Identifier.Equals(identifier))
                {
                    identifier.Declaration = ast;
                    return sft.Type;
                }
            }
            return StandardEnvironment.ErrorType;
        }
    }
}