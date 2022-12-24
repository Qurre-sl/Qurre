using Qurre.API;
using Qurre.API.Addons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qurre.Loader
{
    static internal class Configs
    {
        static internal JsonConfig Config { get; private set; }
        static internal bool AllUnits { get; private set; } = true;
        static internal bool OnlyTutorialUnit { get; private set; } = false;
        static internal bool SpawnBlood { get; private set; } = false;
        static internal bool Better268 { get; private set; } = false;
        static internal bool BetterHints { get; private set; } = false;
        static internal string[] ReloadAccess { get; private set; } = new string[] { };

        static internal void Setup()
        {
            JsonConfig.Init();
            Config = new("Qurre");

            Log.Debugging = Config.SafeGetValue("Debug", false, "Are Debug logs enabled?");
            Log.Logging = Config.SafeGetValue("Logging", true, "Are errors saved to the log file?");
            Log.AllLogging = Config.SafeGetValue("AllLogging", false, "Are all console output being saved to a log file?");

            AllUnits = Config.SafeGetValue("AllUnit", true, "Should I show the Qurre version on Units for all roles?");
            OnlyTutorialUnit = Config.SafeGetValue("OnlyTutorialsUnit", false, "Should I show the Qurre version on Units only for the Tutorial role?");
            SpawnBlood = Config.SafeGetValue("SpawnBlood", true, "Allow the appearance of blood?");
            Better268 = Config.SafeGetValue("Better268", false, "SCP 079 & SCP 096 will not see the wearer of SCP 268");
            BetterHints = Config.SafeGetValue("BetterHints", false, "Enable Addon [BetterHints]?");
            ReloadAccess = Config.SafeGetValue("ReloadAccess", new string[] { "owner", "UserId64@steam", "UserDiscordId@discord" }, "Those who can use the \"reload\" command");

            try { Patches.Events.Player.BanAndKick.SetUpConfigs(); } catch { }

            JsonConfig.UpdateFile();
        }
    }
}