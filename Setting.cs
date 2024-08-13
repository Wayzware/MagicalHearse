using System;
using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using System.Collections.Generic;
using Colossal.Logging;
using Game.UI.Menu;

namespace MagicalHearse
{
    [FileLocation($"ModsSettings/{nameof(MagicalHearse)}/{nameof(MagicalHearse)}")]
    public class Setting : ModSetting
    {

        public const string kMainSection = "Settings";
        public static Setting Instance;

        public Setting(IMod mod) : base(mod)
        {

        }

        [SettingsUISection(kMainSection)] public bool EnableMagicalHearse { get; set; } = true;

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
                {m_Setting.GetSettingsLocaleID(), "Asset Packs Manager"},
                { m_Setting.GetOptionTabLocaleID(Setting.kMainSection), "Settings" },

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