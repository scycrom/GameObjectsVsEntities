using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS
{
    public partial class MoveSpawner : SystemBase
    {
        private int _size = 100;
        private bool _isDirty = true;

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<CubeConfigComponent>();
        }

        protected override void OnUpdate()
        {
            if (SpawnerCommon.TryHandleSizeInput(ref _size))
            {
                _isDirty = true;
            }

            if (!_isDirty) return;
            
            
            Entity cubePrefab = SystemAPI.GetSingleton<CubeConfigComponent>().CubePrefab;
            
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
            
            var clearJob = new ClearCubesJob
            {
                Ecb = ecb
            };
            
            var spawnJob = new SpawnCubesJob
            {
                Ecb = ecb,
                CubePrefab = cubePrefab,
                Size = _size
            };
            
            JobHandle clearHandle = clearJob.Schedule(Dependency);
            JobHandle spawnHandle = spawnJob.Schedule(clearHandle);
            spawnHandle.Complete();
            
            ecb.Playback(EntityManager);
            ecb.Dispose();
            
            _isDirty = false;
            SpawnerEventChannel.OnSpawn?.Invoke(_size * _size);
        }

        
        [BurstCompile]
        [WithAll(typeof(MoveComponent))]
        private partial struct ClearCubesJob : IJobEntity
        {
            public EntityCommandBuffer Ecb;

            public void Execute(Entity entity)
            {
                Ecb.DestroyEntity(entity);
            }
        }
        
        [BurstCompile]
        private struct SpawnCubesJob : IJob
        {
            public EntityCommandBuffer Ecb;
            public Entity CubePrefab;
            public int Size;

            public void Execute()
            {
                for (int x = 0; x < Size; x++)
                {
                    for (int z = 0; z < Size; z++)
                    {
                        Entity cube = Ecb.Instantiate(CubePrefab);

                        Ecb.RemoveComponent<LocalTransform>(cube);
                        Ecb.SetComponent(cube, new LocalToWorld
                        {
                            Value = float4x4.Translate(SpawnerCommon.GridPosition(x, z, Size))
                        });

                        Ecb.SetComponent(cube,
                            new MoveComponent
                            {
                                Time = SpawnerCommon.ComputeRipplePhase(x, z, Size)
                            });
                    }
                }
            }
            
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            _isDirty = true;

        }
    }
}