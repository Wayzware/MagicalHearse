using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;

namespace MagicalHearse;

public class Mod : IMod
{
    private Setting m_Setting;
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

        m_Setting = new Setting(this);
        m_Setting.RegisterInOptionsUI();
        GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));
        AssetDatabase.global.LoadSettings(nameof(MagicalHearse), m_Setting, new Setting(this));
        Setting.Instance = m_Setting;

        updateSystem.UpdateAt<MagicalHearseSystem>(SystemUpdatePhase.GameSimulation);
    }

    public void OnDispose()
    {
        Log.Info(nameof(OnDispose));
    }
}
