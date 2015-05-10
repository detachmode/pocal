using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pocal
{
   public class SettingsViewModel
    {

        // Our settings
        IsolatedStorageSettings settings;

        const string LiveTileSettingsSingleKeyname = "LiveTileSettingsSingle";
        const string LiveTileSettingsMultiKeyname = "LiveTileSettingsMulti";
        const string DefaultViewSettingsAgendaKeyName = "DefaultViewSettingsAgenda";
        const string DefaultViewSettingsOverviewKeyName = "DefaultViewSettingsOverview";

        const string SundayRedKeyName = "SundayRed";

        const string HiddenCalendarsKeyName = "HiddenCalendars";

       


        const bool LiveTileSettingsSingleDefault = false;
        const bool LiveTileSettingsMultiDefault = true;

        const bool DefaultViewSettingsAgendaDefault = true;
        const bool DefaultViewSettingsOverviewDefault = false;

        const bool SundayRedDefault = false;

        List<string> HiddenCalendarsDefault = new List<string>();


        public SettingsViewModel()
        {
            // Get the settings for this application.
            settings = IsolatedStorageSettings.ApplicationSettings;
        }

        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (settings.Contains(Key))
            {
                // If the value has changed
                if (settings[Key] != value)
                {
                    // Store the new value
                    settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                settings.Add(Key, value);
                valueChanged = true;
            }
            return valueChanged;
        }


        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (settings.Contains(Key))
            {
                value = (T)settings[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }


        public void Save()
        {
            settings.Save();
        }



        public bool LiveTileSingleSettings
        {
            get
            {
                return GetValueOrDefault<bool>(LiveTileSettingsSingleKeyname, LiveTileSettingsSingleDefault);
            }
            set
            {
                if (AddOrUpdateValue(LiveTileSettingsSingleKeyname, value))
                {
                    Save();
                }
            }
        }


        public bool LiveTileMulitSettings
        {
            get
            {
                return GetValueOrDefault<bool>(LiveTileSettingsMultiKeyname, LiveTileSettingsMultiDefault);
            }
            set
            {
                if (AddOrUpdateValue(LiveTileSettingsMultiKeyname, value))
                {
                    Save();
                }
            }
        }


        public bool DefaultViewSettingsAgenda
        {
            get
            {
                return GetValueOrDefault<bool>(DefaultViewSettingsAgendaKeyName, DefaultViewSettingsAgendaDefault);
            }
            set
            {
                if (AddOrUpdateValue(DefaultViewSettingsAgendaKeyName, value))
                {
                    Save();
                }
            }
        }

        public bool DefaultViewSettingsOverview
        {
            get
            {
                return GetValueOrDefault<bool>(DefaultViewSettingsOverviewKeyName, DefaultViewSettingsOverviewDefault);
            }
            set
            {
                if (AddOrUpdateValue(DefaultViewSettingsOverviewKeyName, value))
                {
                    Save();
                }
            }
        }

        public bool SundayRed
        {
            get
            {
                return GetValueOrDefault<bool>(SundayRedKeyName, SundayRedDefault);
            }
            set
            {
                if (AddOrUpdateValue(SundayRedKeyName, value))
                {
                    Save();
                }
            }
        }

        public List<string> HiddenCalendars
        {
            get
            {
                return GetValueOrDefault<List<string>>(HiddenCalendarsKeyName, HiddenCalendarsDefault);
            }
            set
            {
                if (AddOrUpdateValue(HiddenCalendarsKeyName, value))
                {
                    Save();
                }
            }
        }

    }
}

