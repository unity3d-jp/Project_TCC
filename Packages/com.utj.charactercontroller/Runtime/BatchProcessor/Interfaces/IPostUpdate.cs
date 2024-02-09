namespace Unity.TinyCharacterController.Interfaces.Subsystem
{
    public interface IPostUpdate : ISystemBase
    {
        void OnLateUpdate();
    }
}