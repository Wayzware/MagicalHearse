using Game;
using Game.Citizens;
using Game.Common;
using Game.Tools;
using Unity.Burst;
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

            _deadCitizenQuery = GetEntityQuery(new EntityQueryDesc()
            {
                All = new[] {
                    ComponentType.ReadOnly<Citizen>(),
                    ComponentType.ReadOnly<HealthProblem>()
                },
                None = new[] {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Temp>()
                }
            });
            _endFrameBarrier = base.World.GetOrCreateSystemManaged<EndFrameBarrier>();

            Mod.Log.Info("Injected MagicalHearseSystem! (Burst compiled)");
            RequireForUpdate(_deadCitizenQuery);
        }

        protected override void OnUpdate()
        {
            MagicalHearseJob job = default;
            job.m_entityTypeHandle = SystemAPI.GetEntityTypeHandle();
            job.m_entityCommandBuffer = _endFrameBarrier.CreateCommandBuffer();
            job.m_citizensChunks = _deadCitizenQuery.ToArchetypeChunkListAsync(World.UpdateAllocator.ToAllocator, out _);
            job.m_healthProblemLookup = GetComponentLookup<HealthProblem>(true);
            JobHandle handle = job.Schedule(Dependency);
            _endFrameBarrier.AddJobHandleForProducer(handle);
            Dependency = handle;
        }

        [BurstCompile]
        private struct MagicalHearseJob : IJob
        {
            public EntityCommandBuffer m_entityCommandBuffer;
            public NativeList<ArchetypeChunk> m_citizensChunks;
            public EntityTypeHandle m_entityTypeHandle;
            public ComponentLookup<HealthProblem> m_healthProblemLookup;

            public void Execute()
            {
                for (int i = 0; i < m_citizensChunks.Length; i++)
                {
                    var citizenArray = m_citizensChunks[i].GetNativeArray(m_entityTypeHandle);
                    for (int j = 0; j < citizenArray.Length; j++)
                    {
                        if (m_healthProblemLookup.TryGetComponent(citizenArray[j], out var healthProblem) &&
                            (healthProblem.m_Flags & HealthProblemFlags.Dead) != 0 &&
                            (healthProblem.m_Flags & HealthProblemFlags.RequireTransport) != 0)
                        {
                            m_entityCommandBuffer.AddComponent<Deleted>(citizenArray[j]);
                        }
                    }
                }
            }
        }
    }
}
