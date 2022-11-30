using Unity.Entities;

namespace Components
{
    public struct Hand : IComponentData
    {
        public Entity inHand;
    }
}