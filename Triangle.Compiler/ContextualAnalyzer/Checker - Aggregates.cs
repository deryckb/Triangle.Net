using Triangle.Compiler.SyntaxTrees.Aggregates;
using Triangle.Compiler.SyntaxTrees.Terminals;
using Triangle.Compiler.SyntaxTrees.Types;
using Triangle.Compiler.SyntaxTrees.Visitors;

namespace Triangle.Compiler.ContextualAnalyzer
{
    public partial class Checker : IArrayAggregateVisitor<Void, TypeDenoter>,
            IRecordAggregateVisitor<Void, FieldTypeDenoter>
    {
        public TypeDenoter VisitMultipleArrayAggregate(MultipleArrayAggregate ast, Void arg)
        {
            var expressionType = ast.Expression.Visit(this);
            var elemType = ast.ArrayAggregate.Visit(this);
            CheckAndReportError(expressionType.Equals(elemType), "incompatible array-aggregate element",
                ast.Expression);
            return elemType;
        }

        public TypeDenoter VisitSingleArrayAggregate(SingleArrayAggregate ast, Void arg)
        {
            var elemType = ast.Expression.Visit(this);
            return elemType;
        }

        public FieldTypeDenoter VisitMultipleRecordAggregate(MultipleRecordAggregate ast, Void arg)
        {
            var expressionType = ast.Expression.Visit(this);
            var recordType = ast.RecordAggregate.Visit(this);
            var fieldType = CheckFieldIdentifier(recordType, ast.Identifier);
            CheckAndReportError(fieldType == StandardEnvironment.ErrorType,
                "duplicate field \"%\" in record", ast.Identifier);
            return ast.Type = new MultipleFieldTypeDenoter(ast.Identifier, expressionType, recordType,
                ast.Position);
        }

        public FieldTypeDenoter VisitSingleRecordAggregate(SingleRecordAggregate ast, Void arg)
        {
            var expressionType = ast.Expression.Visit(this);
            return ast.Type = new SingleFieldTypeDenoter(ast.Identifier, expressionType, ast.Position);
        }

        TypeDenoter CheckFieldIdentifier(FieldTypeDenoter ast, Identifier identifier, Void arg)
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