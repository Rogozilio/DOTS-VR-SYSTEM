using Enums;
using Unity.Entities;
using Unity.Physics.Authoring;

namespace Components
{
    public struct Raycast : IComponentData
    {
        public DirectionType directionType;
        public float lenght;
        public Entity hitEntity;
    }
}