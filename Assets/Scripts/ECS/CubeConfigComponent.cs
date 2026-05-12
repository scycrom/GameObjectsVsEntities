using Unity.Entities;

namespace ECS
{
    public struct CubeConfigComponent : IComponentData
    {
        public Entity CubePrefab;
    }
}