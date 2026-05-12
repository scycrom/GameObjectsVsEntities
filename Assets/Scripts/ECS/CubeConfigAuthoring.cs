using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace ECS
{
    public class CubeConfigAuthoring : MonoBehaviour
    {
        [FormerlySerializedAs("cubePrefab")] [SerializeField] private GameObject _cubePrefab;
        private class CubeConfigAuthoringBaker : Baker<CubeConfigAuthoring>
        {
            public override void Bake(CubeConfigAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new CubeConfigComponent
                {
                    CubePrefab = GetEntity(authoring._cubePrefab, TransformUsageFlags.None)
                });
            }
        }
    }
}