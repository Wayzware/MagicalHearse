using Game;
using Game.Citizens;
using Game.Common;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;

namespace Wayz.CS2.MagicalHearse;
public class MagicalHearseSystem : GameSystemBase
{
    private EntityCommandBuffer _commandBuffer;

    private EntityQuery _deadCitizenQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        _commandBuffer = base.World.GetOrCreateSystemManaged<EndFrameBarrier>().CreateCommandBuffer();

        _deadCitizenQuery = GetEntityQuery(new EntityQueryDesc()
        {
            All = new []
            {
                ComponentType.ReadOnly<Citizen>(),
                ComponentType.ReadOnly<HealthProblem>()
            },
            None = new []
            {
                ComponentType.ReadOnly<Deleted>(),
                ComponentType.ReadOnly<Temp>()
            }
        });
        MagicalHearseMod.GameLogger.LogInfo("Injected MagicalHearseSystem!");
        RequireForUpdate(_deadCitizenQuery);
    }

    protected override void OnUpdate()
    {
        var deadCitizens = _deadCitizenQuery.ToEntityArray(Allocator.Temp);

        foreach (var entity in deadCitizens)
        {
            var healthProblem = EntityManager.GetComponentData<HealthProblem>(entity);
            if((healthProblem.m_Flags & HealthProblemFlags.Dead) != 0 && (healthProblem.m_Flags & HealthProblemFlags.RequireTransport) != 0)
            {
                _commandBuffer.AddComponent<Deleted>(entity);
#if DEBUG
                MagicalHearseMod.GameLogger.LogInfo($"Deleted citizen {entity.Index}");
#endif
            }
        }
    }
}
