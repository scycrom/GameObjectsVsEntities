using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS
{
    public partial struct MoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MoveComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var job = new MoveJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            };
            
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        private partial struct MoveJob : IJobEntity
        {
            public float DeltaTime;

            public void Execute(ref MoveComponent move, ref LocalToWorld localToWorld)
            {
                move.Time += DeltaTime;
                localToWorld.Value.c3.y = 2 * math.sin(move.Time);
            }
        }
    }
}