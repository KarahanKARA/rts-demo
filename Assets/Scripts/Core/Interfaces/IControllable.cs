namespace Core.Interfaces
{
    public interface IControllable
    {
        void MoveTo(UnityEngine.Vector3 position, bool clearTarget = true);
    }
}