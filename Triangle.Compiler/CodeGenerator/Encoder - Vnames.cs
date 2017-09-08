using Triangle.AbstractMachine;
using Triangle.Compiler.CodeGenerator.Entities;
using Triangle.Compiler.SyntaxTrees.Visitors;
using Triangle.Compiler.SyntaxTrees.Vnames;

namespace Triangle.Compiler.CodeGenerator
{
    public partial class Encoder : IVnameVisitor<Frame, IFetchableEntity>
    {

        // Value-or-variable names

        public IFetchableEntity VisitDotVname(DotVname ast, Frame frame)
        {
            return ast.Vname.Visit(this, frame);
        }

        public IFetchableEntity VisitSimpleVname(SimpleVname ast, Frame frame)
        {
            return ast.Identifier.Declaration.Entity as IFetchableEntity;
        }

        public IFetchableEntity VisitSubscriptVname(SubscriptVname ast, Frame frame)
        {
            var baseObject = ast.Vname.Visit(this, frame);
            var elemSize = ast.Type.Visit(this, null);
            if (!ast.Expression.IsLiteral)
            {
                // v-name is indexed by a proper expression, not a literal
                if (ast.Vname.IsIndexed)
                {
                    frame = frame.Expand(Machine.IntegerSize);
                }
                ast.Expression.Visit(this, frame);
                if (elemSize != 1)
                {
                    _emitter.Emit(OpCode.LOADL, elemSize);
                    _emitter.Emit(OpCode.CALL, Register.SB, Register.PB, Primitive.MULT);
                }
                if (ast.Vname.IsIndexed)
                {
                    _emitter.Emit(OpCode.CALL, Register.SB, Register.PB, Primitive.ADD);
                }
            }
            return baseObject;
        }
    }
}