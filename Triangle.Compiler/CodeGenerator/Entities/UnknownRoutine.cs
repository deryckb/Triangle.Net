using Triangle.AbstractMachine;

namespace Triangle.Compiler.CodeGenerator.Entities
{
    public class UnknownRoutine : RuntimeEntity, IRoutineEntity
    {

        readonly ObjectAddress _address;

        public UnknownRoutine(int size, int level, int displacement)
            : base(size)
        {
            _address = new ObjectAddress(level, displacement);
        }

        public void EncodeCall(Emitter emitter, Frame frame)
        {
            emitter.Emit(OpCode.LOAD, Machine.ClosureSize, frame.DisplayRegister(_address),
                _address.Displacement);
            emitter.Emit(OpCode.CALLI, 0);
        }

        public void EncodeFetch(Emitter emitter, Frame frame)
        {
            emitter.Emit(OpCode.LOAD, Machine.ClosureSize, frame.DisplayRegister(_address),
                _address.Displacement);
        }

    }
}
