using Game;
using Game.Citizens;
using Game.Common;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;

namespace MagicalHearse;
public sealed partial class MagicalHearseSystem : GameSystemBase
{
    private EntityQuery _deadCitizenQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        _deadCitizenQuery = GetEntityQuery(new EntityQueryDesc()
        {
            All =
            [
                ComponentType.ReadOnly<Citizen>(),
                ComponentType.ReadOnly<HealthProblem>()
            ],
            None =
            [
                ComponentType.ReadOnly<Deleted>(),
                ComponentType.ReadOnly<Temp>()
            ]
        });
        Mod.Log.Info("Injected MagicalHearseSystem!");
        RequireForUpdate(_deadCitizenQuery);
    }

    protected override void OnUpdate()
    {
        var deadCitizens = _deadCitizenQuery.ToEntityArray(Allocator.Temp);

        foreach (var entity in deadCitizens)
        {
            var healthProblem = EntityManager.GetComponentData<HealthProblem>(entity);
            if ((healthProblem.m_Flags & HealthProblemFlags.Dead) != 0 && (healthProblem.m_Flags & HealthProblemFlags.RequireTransport) != 0)
            {
                try
                {
                    EntityManager.AddComponent<Deleted>(entity);
                }
                catch
                {
                    Mod.Log.Error($"An error occured while trying to delete a dead citizen.");
                }
            }
        }
    }
}
