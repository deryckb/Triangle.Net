using Triangle.Compiler.SyntaxTrees.Aggregates;
using Triangle.Compiler.SyntaxTrees.Visitors;

namespace Triangle.Compiler.CodeGenerator
{
    public partial class Encoder : IArrayAggregateVisitor<Frame, int>, IRecordAggregateVisitor<Frame, int>
    {
        // Array Aggregates
        public int VisitMultipleArrayAggregate(MultipleArrayAggregate ast, Frame frame)
        {
            var elemSize = ast.Expression.Visit(this, frame);
            var frame1 = frame.Expand(elemSize);
            var arraySize = ast.ArrayAggregate.Visit(this, frame1);
            return elemSize + arraySize;
        }

        public int VisitSingleArrayAggregate(SingleArrayAggregate ast, Frame frame)
        {
            return ast.Expression.Visit(this, frame);
        }

        // Record Aggregates
        public int VisitMultipleRecordAggregate(MultipleRecordAggregate ast, Frame frame)
        {
            var fieldSize = ast.Expression.Visit(this, frame);
            var frame1 = frame.Expand(fieldSize);
            var recordSize = ast.RecordAggregate.Visit(this, frame1);
            return fieldSize + recordSize;
        }

        public int VisitSingleRecordAggregate(SingleRecordAggregate ast, Frame frame)
        {
            return ast.Expression.Visit(this, frame);
        }

    }
}