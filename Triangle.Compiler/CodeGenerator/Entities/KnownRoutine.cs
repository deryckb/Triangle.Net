using Triangle.AbstractMachine;

namespace Triangle.Compiler.CodeGenerator.Entities
{
    public class KnownRoutine : RuntimeEntity, IRoutineEntity
    {

        readonly ObjectAddress _address;

        public KnownRoutine(int size, int level, int displacement)
            : base(size)
        {
            _address = new ObjectAddress(level, displacement);
        }

        public void EncodeCall(Emitter emitter, Frame frame)
        {
            emitter.Emit(OpCode.CALL, frame.DisplayRegister(_address), Register.CB,
                _address.Displacement);
        }

        public void EncodeFetch(Emitter emitter, Frame frame)
        {
            emitter.Emit(OpCode.LOADA, 0, frame.DisplayRegister(_address), 0);
            emitter.Emit(OpCode.LOADA, 0, Register.CB, _address.Displacement);
        }

    }
}