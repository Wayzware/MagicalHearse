using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using System.Collections.Generic;
using Unity.Entities;

namespace MagicalHearse
{
    [FileLocation("ModsSettings/MagicalHearse/MagicalHearse")]
    public class Setting : ModSetting
    {
        public static Setting Instance;

        public Setting(IMod mod) : base(mod) 
        { }

        public bool EnableMagicalHearse { get; set; } = true;

        public override void Apply()
        {
            base.Apply();

            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<MagicalHearseSystem>().Enabled = EnableMagicalHearse;
        }

        public override void SetDefaults()
        {
            EnableMagicalHearse = true;
        }
    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                {m_Setting.GetSettingsLocaleID(), "Magical Hearse"},
                {m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableMagicalHearse)), "Enable Magical Hearse"},
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableMagicalHearse)),
                    $"Enables the magical hearse. Dead citizens will disappear automatically."
                },
            };
        }

        public void Unload()
        {
        }
    }
}