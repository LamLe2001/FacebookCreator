namespace FacebookCreator.Helper
{
    public class SettingsTool
    {
        internal static Dictionary<string, JsonHelper> settings = new Dictionary<string, JsonHelper>();
        public static JsonHelper GetSettings(string settingName, bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                RefreshSettings(settingName);
            }
            else if (!settings.ContainsKey(settingName))
            {
                settings.Add(settingName, new JsonHelper(settingName));
            }
            return settings[settingName];
        }

        private static void RefreshSettings(string settingName)
        {
            if (settings.ContainsKey(settingName))
            {
                settings[settingName] = new JsonHelper(settingName);
            }
            else
            {
                settings.Add(settingName, new JsonHelper(settingName));
            }
        }
        public static void UpdateSetting(string settingName)
        {
            if (settings.ContainsKey(settingName))
            {
                settings[settingName].SaveJsonToFile();
            }
            RefreshSettings(settingName);
        }


    }
}
