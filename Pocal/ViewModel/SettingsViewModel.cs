using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;

namespace Pocal.ViewModel
{
    public class SettingsViewModel
    {
        private const string LiveTileSettingsSingleKeyname = "LiveTileSettingsSingle";
        private const string LiveTileSettingsMultiKeyname = "LiveTileSettingsMulti";
        private const string SundayRedKeyName = "SundayRed";
        private const string HiddenCalendarsKeyName = "HiddenCalendars";
        private const string TileDaysInFutureKeyName = "TileDaysInFuture";
        private const bool LiveTileSettingsSingleDefault = false;
        private const bool LiveTileSettingsMultiDefault = true;
        private const bool SundayRedDefault = false;
        private const int TileDaysInFutureDefault = 2;
        private readonly List<string> _hiddenCalendarsDefault = new List<string>();
        private readonly IsolatedStorageSettings _settings;
        private readonly string ChoosenTimeStyleKeyName = "ChoosenTimeStyle";
        public int ChoosenTimeStyleDefault = 0;
        public bool RestartNeeded;


        public SettingsViewModel()
        {
            // Get the settings for this application.
            _settings = IsolatedStorageSettings.ApplicationSettings;
        }

        public List<string> TimeStyles
        {
            get { return new List<string> {"24 Hours", "AM / PM"}; }
        }

        public bool LiveTileSingleSettings
        {
            get { return GetValueOrDefault(LiveTileSettingsSingleKeyname, LiveTileSettingsSingleDefault); }
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
            get { return GetValueOrDefault(LiveTileSettingsMultiKeyname, LiveTileSettingsMultiDefault); }
            set
            {
                if (AddOrUpdateValue(LiveTileSettingsMultiKeyname, value))
                {
                    Save();
                }
            }
        }

        public bool SundayRed
        {
            get { return GetValueOrDefault(SundayRedKeyName, SundayRedDefault); }
            set
            {
                if (AddOrUpdateValue(SundayRedKeyName, value))
                {
                    Save();
                }
            }
        }

        public int ChoosenTimeStyle
        {
            get { return GetValueOrDefault(ChoosenTimeStyleKeyName, ChoosenTimeStyleDefault); }
            set
            {
                if (AddOrUpdateValue(ChoosenTimeStyleKeyName, value))
                {
                    RestartNeeded = true;
                    Save();
                }
            }
        }

        public int TileDaysInFuture
        {
            get { return GetValueOrDefault(TileDaysInFutureKeyName, TileDaysInFutureDefault); }
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
            get { return GetValueOrDefault(HiddenCalendarsKeyName, _hiddenCalendarsDefault); }
            set
            {
                if (AddOrUpdateValue(HiddenCalendarsKeyName, value))
                {
                    Save();
                }
            }
        }

        internal bool IsTimeStyleAMPM()
        {
            return ChoosenTimeStyle == 1;
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
                value = (T) _settings[key];
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
    }
}