using Triangle.AbstractMachine;
using Triangle.Compiler.SyntaxTrees.Vnames;

namespace Triangle.Compiler.CodeGenerator.Entities
{
    public class KnownAddress : AddressableEntity
    {

        public KnownAddress(int size, int level, int displacement)
            : base(size, level, displacement)
        {
        }

        public KnownAddress(int size, Frame frame)
             : base(size, frame)
        {
        }

        public override void EncodeStore(Emitter emitter, Frame frame, int size, Vname vname)
        {
            if (vname.IsIndexed)
            {
                emitter.Emit(OpCode.LOADA, 0, frame.DisplayRegister(Address),
                    Address.Displacement + vname.Offset);
                emitter.Emit(OpCode.CALL, Register.SB, Register.PB, Primitive.ADD);
                emitter.Emit(OpCode.STOREI, size, 0, 0);
            }
            else
            {
                emitter.Emit(OpCode.STORE, size, frame.DisplayRegister(Address),
                    Address.Displacement + vname.Offset);
            }
        }

        public override void EncodeFetch(Emitter emitter, Frame frame, int size, Vname vname)
        {
            if (vname.IsIndexed)
            {
                emitter.Emit(OpCode.LOADA, 0, frame.DisplayRegister(Address),
                    Address.Displacement + vname.Offset);
                emitter.Emit(OpCode.CALL, Register.SB, Register.PB, Primitive.ADD);
                emitter.Emit(OpCode.LOADI, size, 0, 0);
            }
            else
            {
                emitter.Emit(OpCode.LOAD, size, frame.DisplayRegister(Address),
                    Address.Displacement + vname.Offset);
            }
        }

        public override void EncodeFetchAddress(Emitter emitter, Frame frame, Vname vname)
        {
            emitter.Emit(OpCode.LOADA, 0, frame.DisplayRegister(Address),
                Address.Displacement + vname.Offset);
            if (vname.IsIndexed)
            {
                emitter.Emit(OpCode.CALL, Register.SB, Register.PB, Primitive.ADD);
            }
        }
    }
}