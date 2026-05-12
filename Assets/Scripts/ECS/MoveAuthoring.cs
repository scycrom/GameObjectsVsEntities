using Unity.Entities;
using UnityEngine;

namespace ECS
{
    public class MoveAuthoring : MonoBehaviour
    {
        private class MoveAuthoringBaker : Baker<MoveAuthoring>
        {
            public override void Bake(MoveAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new MoveComponent());
            }
        }
    }
}