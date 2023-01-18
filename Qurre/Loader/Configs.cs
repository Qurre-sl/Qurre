using System;
using Newtonsoft.Json.Linq;
using Qurre.API;
using Qurre.API.Addons;

namespace Qurre.Loader
{
    internal static class Configs
    {
        internal static JsonConfig Config { get; private set; }

        internal static bool AllUnits { get; private set; } = true;

        internal static bool OnlyTutorialUnit { get; private set; }

        internal static bool SpawnBlood { get; private set; }

        internal static bool Better268 { get; private set; }

        internal static bool BetterHints { get; private set; }

        internal static bool PrintLogo { get; private set; }

        internal static string[] ReloadAccess { get; private set; } = Array.Empty<string>();

        internal static string Banned { get; private set; } = "You have been banned. Reason: ";

        internal static string Kicked { get; private set; } = "You have been kicked. Reason: ";

        internal static void Setup()
        {
            JsonConfig.Init();
            Config = new ("Qurre");

            Log.Debugging = Config.TrySafeGetValue("Debug", false, "Are Debug logs enabled?");
            Log.Logging = Config.TrySafeGetValue("Logging", true, "Are errors saved to the log file?");
            Log.AllLogging = Config.TrySafeGetValue("AllLogging", false, "Are all console output being saved to a log file?");

            AllUnits = Config.TrySafeGetValue("AllUnit", true, "Should I show the Qurre version on Units for all roles?");
            OnlyTutorialUnit = Config.TrySafeGetValue("OnlyTutorialsUnit", false, "Should I show the Qurre version on Units only for the Tutorial role?");
            SpawnBlood = Config.TrySafeGetValue("SpawnBlood", true, "Allow the appearance of blood?");
            Better268 = Config.TrySafeGetValue("Better268", false, "SCP 079 & SCP 096 will not see the wearer of SCP 268");
            BetterHints = Config.TrySafeGetValue("BetterHints", false, "Enable Addon [BetterHints]?");
            PrintLogo = Config.TrySafeGetValue("PrintLogo", true, "Print Qurre Logo?");
            ReloadAccess = Config.TrySafeGetValue("ReloadAccess", new[] { "owner", "UserId64@steam", "UserDiscordId@discord" }, "Those who can use the \"reload\" command");

            SetupTranslations();

            JsonConfig.Save();
        }

        internal static void SetupTranslations()
        {
            JToken parent = Config.JsonArray["Translations"];

            if (parent is null)
            {
                parent = JObject.Parse("{ }");
                Config.JsonArray["Translations"] = parent;
            }

            Banned = Config.SafeGetValue("Banned", Banned, source: parent);
            Kicked = Config.SafeGetValue("Kicked", Kicked, source: parent);
        }
    }
}