using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace RedditPostbot.Settings
{
    public class SettingsController
    {
        private static SettingsController _instance;
        private static readonly object _instanceLock = new object();

        public static SettingsStore SettingsStore;
        private readonly object _settingsLock = new object();

        private SettingsController()
        {
            SettingsStore = LoadSettings();
        }

        public static SettingsController GetInstance()
        {
            if (_instance == null)
                lock (_instanceLock)
                    if (_instance == null)
                        _instance = new SettingsController();
            return _instance;
        }

        private SettingsStore LoadSettings()
        {
            try
            {
                var rawJson = File.ReadAllText("Settings.json");
                var settings = new JavaScriptSerializer().Deserialize<SettingsStore>(rawJson);
                return settings;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveSettings()
        {
            lock (_settingsLock)
            {
                var settingsJson = new JavaScriptSerializer().Serialize(SettingsStore);
                var stream = new StreamWriter(File.Create("Settings.json"));
                stream.Write(settingsJson);
                stream.Close();
            }
        }
    }
}
