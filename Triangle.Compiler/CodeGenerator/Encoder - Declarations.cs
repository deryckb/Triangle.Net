using Triangle.AbstractMachine;
using Triangle.Compiler.CodeGenerator.Entities;
using Triangle.Compiler.SyntaxTrees.Declarations;
using Triangle.Compiler.SyntaxTrees.Visitors;

namespace Triangle.Compiler.CodeGenerator
{
    public partial class Encoder : IDeclarationVisitor<Frame, int>
    {
        // Declarations
        public int VisitBinaryOperatorDeclaration(BinaryOperatorDeclaration ast, Frame frame)
        {
            return 0;
        }

        public int VisitConstDeclaration(ConstDeclaration ast, Frame frame)
        {
            var extraSize = 0;
            var expr = ast.Expression;
            if (expr.IsLiteral)
            {
                ast.Entity = new KnownValue(expr.Type.Size, expr.Value);
            }
            else
            {
                extraSize = expr.Visit(this, frame);
                ast.Entity = new UnknownValue(extraSize, frame);
            }
            Encoder.WriteTableDetails(ast);
            return extraSize;
        }

        public int VisitFuncDeclaration(FuncDeclaration ast, Frame frame)
        {
            var argsSize = 0;
            var valSize = 0;
            var jumpAddr = _emitter.Emit(OpCode.JUMP, Register.CB);
            ast.Entity = new KnownRoutine(Machine.ClosureSize, frame.Level, _emitter.NextInstrAddr);
            Encoder.WriteTableDetails(ast);
            if (frame.Level == Machine.MaximumRoutineLevel)
            {
                _errorReporter.ReportRestriction("can't nest routines more than 7 deep");
            }
            else
            {
                argsSize = ast.Formals.Visit(this, frame.Push(0));
                valSize = ast.Expression.Visit(this, frame.Push(Machine.LinkDataSize));
            }
            _emitter.Emit(OpCode.RETURN, (byte)valSize, argsSize);
            _emitter.Patch(jumpAddr);
            return 0;
        }

        public int VisitProcDeclaration(ProcDeclaration ast, Frame frame)
        {
            var argsSize = 0;
            var jumpAddr = _emitter.Emit(OpCode.JUMP, Register.CB);
            ast.Entity = new KnownRoutine(Machine.ClosureSize, frame.Level, _emitter.NextInstrAddr);
            Encoder.WriteTableDetails(ast);
            if (frame.Level == Machine.MaximumRoutineLevel)
            {
                _errorReporter.ReportRestriction("can't nest routines so deeply");
            }
            else
            {
                argsSize = ast.Formals.Visit(this, frame.Push(0));
                ast.Command.Visit(this, frame.Push(Machine.LinkDataSize));
            }
            _emitter.Emit(OpCode.RETURN, argsSize);
            _emitter.Patch(jumpAddr);
            return 0;
        }

        public int VisitSequentialDeclaration(SequentialDeclaration ast, Frame frame)
        {
            var extraSize1 = ast.FirstDeclaration.Visit(this, frame);
            var extraSize2 = ast.SecondDeclaration.Visit(this, frame.Expand(extraSize1));
            return extraSize1 + extraSize2;
        }

        public int VisitTypeDeclaration(TypeDeclaration ast, Frame frame)
        {
            // just to ensure the type's representation is decided
            ast.Type.Visit(this, null);
            return 0;
        }

        public int VisitUnaryOperatorDeclaration(UnaryOperatorDeclaration ast, Frame frame)
        {
            return 0;
        }

        public int VisitVarDeclaration(VarDeclaration ast, Frame frame)
        {
            var extraSize = ast.Type.Visit(this, null);
            _emitter.Emit(OpCode.PUSH, (short)extraSize);
            ast.Entity = new KnownAddress(Machine.AddressSize, frame);
            Encoder.WriteTableDetails(ast);
            return extraSize;
        }

    }
}