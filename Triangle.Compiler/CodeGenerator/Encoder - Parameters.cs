using Triangle.AbstractMachine;
using Triangle.Compiler.CodeGenerator.Entities;
using Triangle.Compiler.SyntaxTrees.Actuals;
using Triangle.Compiler.SyntaxTrees.Formals;
using Triangle.Compiler.SyntaxTrees.Visitors;

namespace Triangle.Compiler.CodeGenerator
{
    public partial class Encoder :
            IActualParameterVisitor<Frame, int>,
            IActualParameterSequenceVisitor<Frame, int>,
            IFormalParameterVisitor<Frame, int>,
            IFormalParameterSequenceVisitor<Frame, int>
    {
        // Formal Parameters

        public int VisitConstFormalParameter(ConstFormalParameter ast, Frame frame)
        {
            var valSize = ast.Type.Visit(this, null);
            ast.Entity = new UnknownValue(valSize, frame.Level, -frame.Size - valSize);
            Encoder.WriteTableDetails(ast);
            return valSize;
        }

        public int VisitFuncFormalParameter(FuncFormalParameter ast, Frame frame)
        {
            var argsSize = Machine.ClosureSize;
            ast.Entity = new UnknownRoutine(argsSize, frame.Level, -frame.Size - argsSize);
            Encoder.WriteTableDetails(ast);
            return argsSize;
        }

        public int VisitProcFormalParameter(ProcFormalParameter ast, Frame frame)
        {
            var argsSize = Machine.ClosureSize;
            ast.Entity = new UnknownRoutine(argsSize, frame.Level, -frame.Size - argsSize);
            Encoder.WriteTableDetails(ast);
            return argsSize;
        }

        public int VisitVarFormalParameter(VarFormalParameter ast, Frame frame)
        {
            ast.Type.Visit(this, null);
            var argSize = Machine.AddressSize;
            ast.Entity = new UnknownAddress(argSize, frame.Level, -frame.Size - argSize);
            Encoder.WriteTableDetails(ast);
            return Machine.AddressSize;
        }

        public int VisitEmptyFormalParameterSequence(EmptyFormalParameterSequence ast, Frame frame)
        {
            return 0;
        }

        public int VisitMultipleFormalParameterSequence(MultipleFormalParameterSequence ast,
            Frame frame)
        {
            var argsSize1 = ast.Formals.Visit(this, frame);
            var frame1 = frame.Expand(argsSize1);
            var argsSize2 = ast.Formal.Visit(this, frame1);
            return argsSize1 + argsSize2;
        }

        public int VisitSingleFormalParameterSequence(SingleFormalParameterSequence ast,
            Frame frame)
        {
            return ast.Formal.Visit(this, frame);
        }

        // Actual Parameters

        public int VisitConstActualParameter(ConstActualParameter ast, Frame frame)
        {
            return ast.Expression.Visit(this, frame);
        }

        public int VisitFuncActualParameter(FuncActualParameter ast, Frame frame)
        {
            var routine = ast.Identifier.Declaration.Entity as IRoutineEntity;
            routine.EncodeFetch(_emitter, frame);
            return Machine.ClosureSize;
        }

        public int VisitProcActualParameter(ProcActualParameter ast, Frame frame)
        {
            var routine = ast.Identifier.Declaration.Entity as IRoutineEntity;
            routine.EncodeFetch(_emitter, frame);
            return Machine.ClosureSize;
        }

        public int VisitVarActualParameter(VarActualParameter ast, Frame frame)
        {
            EncodeFetchAddress(ast.Vname, frame);
            return Machine.AddressSize;
        }

        public int VisitEmptyActualParameterSequence(EmptyActualParameterSequence ast, Frame frame)
        {
            return 0;
        }

        public int VisitMultipleActualParameterSequence(MultipleActualParameterSequence ast,
            Frame frame)
        {
            var argsSize1 = ast.Actual.Visit(this, frame);
            var frame1 = frame.Expand(argsSize1);
            var argsSize2 = ast.Actuals.Visit(this, frame1);
            return argsSize1 + argsSize2;
        }

        public int VisitSingleActualParameterSequence(SingleActualParameterSequence ast,
            Frame frame)
        {
            return ast.Actual.Visit(this, frame);
        }
    }
}