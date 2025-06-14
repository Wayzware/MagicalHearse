using Game;
using Game.Citizens;
using Game.Common;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace MagicalHearse
{
    public sealed partial class MagicalHearseSystem : GameSystemBase
    {
        private EntityQuery _deadCitizenQuery;
        private EndFrameBarrier _endFrameBarrier;
        public override int GetUpdateInterval(SystemUpdatePhase phase) => 64;

        protected override void OnCreate()
        {
            base.OnCreate();

            _deadCitizenQuery = SystemAPI.QueryBuilder()
                .WithAll<Citizen, HealthProblem>()
                .WithNone<Deleted, Temp>()
                .Build();
            _endFrameBarrier = World.GetOrCreateSystemManaged<EndFrameBarrier>();

            Mod.Log.Info("Injected MagicalHearseSystem! (Burst compiled)");
            RequireForUpdate(_deadCitizenQuery);
        }

        protected override void OnUpdate()
        {
            JobHandle handle = new MagicalHearseJob()
            {
                m_entityTypeHandle = SystemAPI.GetEntityTypeHandle(),
                m_healthProblemType = SystemAPI.GetComponentTypeHandle<HealthProblem>(true),
                m_entityCommandBuffer = _endFrameBarrier.CreateCommandBuffer().AsParallelWriter(),
            }.ScheduleParallel(_deadCitizenQuery, Dependency);
            _endFrameBarrier.AddJobHandleForProducer(handle);
            Dependency = handle;
        }

        [BurstCompile]
        private struct MagicalHearseJob : IJobChunk
        {
            [ReadOnly] public EntityTypeHandle m_entityTypeHandle;
            [ReadOnly] public ComponentTypeHandle<HealthProblem> m_healthProblemType;
            public EntityCommandBuffer.ParallelWriter m_entityCommandBuffer;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                NativeArray<Entity> citizenArray = chunk.GetNativeArray(m_entityTypeHandle);
                NativeArray<HealthProblem> healthProblems = chunk.GetNativeArray(ref m_healthProblemType);
                for (int index = 0; index < citizenArray.Length; index++)
                {
                    if ((healthProblems[index].m_Flags & (HealthProblemFlags.Dead | HealthProblemFlags.RequireTransport)) == (HealthProblemFlags.Dead | HealthProblemFlags.RequireTransport))
                    {
                        m_entityCommandBuffer.AddComponent<Deleted>(unfilteredChunkIndex, citizenArray[index]);
                    }
                }
            }
        }
    }
}
