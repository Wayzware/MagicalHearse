using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;

namespace MagicalHearse
{

    public class Mod : IMod
    {
        public static ILog Log = LogManager.GetLogger($"{nameof(MagicalHearse)}.{nameof(Mod)}").SetShowsErrorsInUI(
#if !DEBUG
            false
#else
            true
#endif
        );

        public void OnLoad(UpdateSystem updateSystem)
        {
            Log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                Log.Info($"Current mod asset at {asset.path}");

            updateSystem.UpdateAt<MagicalHearseSystem>(SystemUpdatePhase.GameSimulation);
        }

        public void OnDispose()
        {
            Log.Info(nameof(OnDispose));
        }
    }
}
