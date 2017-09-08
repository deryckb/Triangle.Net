namespace Triangle.Compiler.CodeGenerator.Entities
{
    public interface IRoutineEntity
    {

        void EncodeCall(Emitter emitter, Frame frame);

        void EncodeFetch(Emitter emitter, Frame frame);

    }

}