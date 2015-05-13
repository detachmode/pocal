using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;

namespace Pocal.ViewModel
{
   public class SettingsViewModel
    {

        // Our settings
       readonly IsolatedStorageSettings _settings;

        const string LiveTileSettingsSingleKeyname = "LiveTileSettingsSingle";
        const string LiveTileSettingsMultiKeyname = "LiveTileSettingsMulti";
        const string DefaultViewSettingsAgendaKeyName = "DefaultViewSettingsAgenda";
        const string DefaultViewSettingsOverviewKeyName = "DefaultViewSettingsOverview";



        const string SundayRedKeyName = "SundayRed";

        const string HiddenCalendarsKeyName = "HiddenCalendars";
        const string TileDaysInFutureKeyName = "TileDaysInFuture";
       


        const bool LiveTileSettingsSingleDefault = false;
        const bool LiveTileSettingsMultiDefault = true;

        const bool DefaultViewSettingsAgendaDefault = true;
        const bool DefaultViewSettingsOverviewDefault = false;

        const bool SundayRedDefault = false;

       private const int TileDaysInFutureDefault = 2;

       readonly List<string> _hiddenCalendarsDefault = new List<string>();


        public SettingsViewModel()
        {
            // Get the settings for this application.
            _settings = IsolatedStorageSettings.ApplicationSettings;
        }

       public bool AddOrUpdateValue(string key, object value)
       {
           if (value == null) throw new ArgumentNullException("value");

           // If the key exists
           if (_settings.Contains(key))
           {
               if (_settings[key] == value) return false;

               // If the value has changed
               // Store the new value
               _settings[key] = value;
           }
           // Otherwise create the key.
           else
           {
               _settings.Add(key, value);
           }
           return true;
       }


        public T GetValueOrDefault<T>(string key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (_settings.Contains(key))
            {
                value = (T)_settings[key];
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
            _settings.Save();
        }



        public bool LiveTileSingleSettings
        {
            get
            {
                return GetValueOrDefault(LiveTileSettingsSingleKeyname, LiveTileSettingsSingleDefault);
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
                return GetValueOrDefault(LiveTileSettingsMultiKeyname, LiveTileSettingsMultiDefault);
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
                return GetValueOrDefault(DefaultViewSettingsAgendaKeyName, DefaultViewSettingsAgendaDefault);
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
                return GetValueOrDefault(DefaultViewSettingsOverviewKeyName, DefaultViewSettingsOverviewDefault);
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
                return GetValueOrDefault(SundayRedKeyName, SundayRedDefault);
            }
            set
            {
                if (AddOrUpdateValue(SundayRedKeyName, value))
                {
                    Save();
                }
            }
        }

        public int TileDaysInFuture
        {
            get
            {
                return GetValueOrDefault(TileDaysInFutureKeyName, TileDaysInFutureDefault);
            }
            set
            {
                if (AddOrUpdateValue(TileDaysInFutureKeyName, value))
                {
                    Save();
                }
            }
        }

        public List<string> HiddenCalendars
        {
            get
            {
                return GetValueOrDefault(HiddenCalendarsKeyName, _hiddenCalendarsDefault);
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

